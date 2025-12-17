using System.Threading.Tasks;
using UnityChess.Application;
using UnityChess.Core;
using UnityChess.DependencyInjection;
using UnityChess.Presentation.ViewModel;
using UnityChess.Util;

namespace UnityChess.Presentation {
	public class BoardMediator : IMediator {
		private readonly BoardVM _boardVM;
		private readonly GameManager _gameManager;

		public BoardMediator(GameManager gameManager, BoardVM boardVM) {
			_gameManager = gameManager;
			_boardVM = boardVM;

			// To Presentation (subscriptions to application events)
			_gameManager.newGameStarted += OnNewGameStarted;
			_gameManager.moveExecuted += OnMoveExecuted;
			_gameManager.gameResetToHalfMove += OnGameResetToHalfMove;

			// To Application (assignments to VM commands)
			_boardVM.onPieceDropped = OnPieceDroppedAsync;
		}

		#region To Presentation Layer

		private void OnNewGameStarted(Board board) {
			ConvertBoardToPieceTypes(board, _boardVM.currentBoard);
		}

		private void OnMoveExecuted(Board board, Timeline<HalfMove> _) {
			ConvertBoardToPieceTypes(board, _boardVM.currentBoard);
		}

		private void OnGameResetToHalfMove(Board board, Timeline<HalfMove> _) {
			ConvertBoardToPieceTypes(board, _boardVM.currentBoard);
		}

		#endregion

		#region To Application Layer

		private async Task<bool> OnPieceDroppedAsync(string fromSquare, string toSquare) {
			return await _gameManager.TryExecuteMoveAsync(
				SquareUtil.StringToSquare(fromSquare),
				SquareUtil.StringToSquare(toSquare)
			);
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
