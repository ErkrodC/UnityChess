using System.Threading.Tasks;
using UnityChess.Application;
using UnityChess.Core;
using UnityChess.Presentation.ViewModel;

namespace UnityChess.Presentation {
	public class BoardMediator {
		private readonly BoardVM _boardVM;
		private readonly GameManager _gameManager;

		public BoardMediator(GameManager gameManager, BoardVM boardVM) {
			_gameManager = gameManager;
			_boardVM = boardVM;

			// To Presentation
			_gameManager.NewGameStarted += OnNewGameStarted;
			_gameManager.MoveExecuted += OnMoveExecuted;

			// To Application
			_boardVM.onPieceDropped = OnPieceDroppedAsync;
		}

		#region To Presentation Layer

		private void OnNewGameStarted(Board board) {
			PieceVM[,] pieceVMs = new PieceVM[8, 8];
			ConvertBoardToPieceTypes(board, pieceVMs);
			_boardVM.currentBoard = pieceVMs;
		}

		private void OnMoveExecuted(Board board, HalfMove _) {
			ConvertBoardToPieceTypes(board, _boardVM.currentBoard);
		}

		#endregion

		#region To Application Layer

		private async Task<bool> OnPieceDroppedAsync(Square fromSquare, Square toSquare) {
			return await _gameManager.TryExecuteMoveAsync(fromSquare, toSquare);
		}

		#endregion

		#region Helpers

		private static void ConvertBoardToPieceTypes(Board board, PieceVM[,] pieceVMs) {
			for (int file = 0; file < 8; file++)
			for (int rank = 0; rank < 8; rank++) {
				Piece piece = board[file, rank];

				PieceVM pieceVM = null;
				if (piece != null) {
					PieceType pieceType = piece switch {
						Pawn => PieceType.Pawn,
						Rook => PieceType.Rook,
						Knight => PieceType.Knight,
						Bishop => PieceType.Bishop,
						Queen => PieceType.Queen,
						King => PieceType.King,
						_ => throw new System.ArgumentException($"Unknown piece type: {piece.GetType().Name}")
					};

					pieceVM = new PieceVM {
						type = pieceType,
						side = piece.Owner
					};
				}

				pieceVMs[file, rank] = pieceVM;
			}
		}

		#endregion
	}
}
