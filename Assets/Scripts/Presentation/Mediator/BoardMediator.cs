using System;
using System.Threading.Tasks;
using UnityChess.Application;
using UnityChess.Application.Service;
using UnityChess.Core;
using UnityChess.DependencyInjection;
using UnityChess.Presentation.ViewModel;
using UnityChess.Core.Util;

namespace UnityChess.Presentation {
	public class BoardMediator : IMediator, IDisposable {
		private readonly GameManager _gameManager;
		private readonly HumanPlayerService _player;
		private readonly BoardVM _vm;
		private MoveInteraction _moveInteraction;

		public BoardMediator(GameManager gameManager, HumanPlayerService player, BoardVM vm) {
			_gameManager = gameManager;
			_player = player;
			_vm = vm;

			// To Presentation (subscriptions to application events)
			_gameManager.newGameStarted += OnNewGameStarted;
			_gameManager.moveExecuted += OnMoveExecuted;
			_gameManager.gameResetToHalfMove += OnGameResetToHalfMove;
			_player.moveRequested += OnMoveRequested;

			// To Application (assignments to VM commands)
			_vm.onPieceDropped = OnPieceDropped;
		}

		public void Dispose() {
			_gameManager.newGameStarted -= OnNewGameStarted;
			_gameManager.moveExecuted -= OnMoveExecuted;
			_gameManager.gameResetToHalfMove -= OnGameResetToHalfMove;
			_player.moveRequested -= OnMoveRequested;
			_vm.onPieceDropped = null;
			_moveInteraction?.TryCancel();
			_moveInteraction = null;
		}

		#region To Presentation Layer

		private void OnNewGameStarted(Board board) {
			ConvertBoardToPieceTypes(board, _vm.currentBoard);
		}

		private void OnMoveExecuted(Board board, Timeline<HalfMove> _) {
			ConvertBoardToPieceTypes(board, _vm.currentBoard);
		}

		private void OnGameResetToHalfMove(Board board, Timeline<HalfMove> _) {
			_moveInteraction?.TryCancel();
			_moveInteraction = null;
			ConvertBoardToPieceTypes(board, _vm.currentBoard);
		}

		private void OnMoveRequested(MoveInteraction moveInteraction) {
			_moveInteraction?.TryCancel();
			_moveInteraction = moveInteraction;
		}

		#endregion

		#region To Application Layer

		private async Task<bool> OnPieceDropped(string fromSquare, string toSquare) {
			if (_moveInteraction == null) { return false; }

			MoveInteraction moveInteraction = _moveInteraction;
			_moveInteraction = null;

			bool received = moveInteraction.TryCompleteGetMove(
				SquareUtil.StringToSquare(fromSquare),
				SquareUtil.StringToSquare(toSquare)
			);

			if (!received) {
				return false;
			}

			bool moveIsValid;
			try {
				moveIsValid = await moveInteraction.isMoveValidTask;
			} catch (OperationCanceledException) {
				moveIsValid = false;
			}

			return moveIsValid;
		}

		#endregion

		#region Helpers

		private static void ConvertBoardToPieceTypes(Board board, PieceVM[,] pieceVMs) {
			for (int file = 0; file < 8; file++)
			for (int rank = 0; rank < 8; rank++) {
				Piece piece = board[file, rank];
				PieceVM pieceVM = pieceVMs[file, rank];

				pieceVM.side = piece?.Owner ?? Side.None;
				pieceVM.type = piece switch {
					Pawn => PieceType.Pawn,
					Rook => PieceType.Rook,
					Knight => PieceType.Knight,
					Bishop => PieceType.Bishop,
					Queen => PieceType.Queen,
					King => PieceType.King,
					_ => PieceType.None
				};
			}
		}

		#endregion
	}
}
