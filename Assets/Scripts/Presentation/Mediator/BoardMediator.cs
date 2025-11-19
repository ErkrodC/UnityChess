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
			_gameManager.newGameStarted += OnNewGameStarted;
			_gameManager.boardChanged += OnBoardChanged;
			_gameManager.turnChanged += OnTurnChanged;

			// To Application
			_boardVM.onSquareClicked = OnSquareClicked;
		}

		#region To Presentation Layer

		private void OnNewGameStarted(Board board) {
			PieceVM[,] pieceVMs = new PieceVM[8, 8];
			ConvertBoardToPieceTypes(board, pieceVMs);
			_boardVM.currentBoard = pieceVMs;
		}

		private void OnBoardChanged(Board board) {
			ConvertBoardToPieceTypes(board, _boardVM.currentBoard);
		}

		private void OnTurnChanged(Side sideToMove) {
			_boardVM.currentSideToMove = sideToMove;
		}

		#endregion

		#region To Application Layer

		private void OnSquareClicked(Square square) {
			// ER TODO: Implement square click logic - select piece, move piece, etc.
		}

		#endregion

		#region Helpers

		private static void ConvertBoardToPieceTypes(Board board, PieceVM[,] boardPieceTypes) {
			for (int file = 1; file <= 8; file++)
			for (int rank = 1; rank <= 8; rank++) {
				Piece piece = board[file, rank];

				if (piece == null) {
					boardPieceTypes[file - 1, rank - 1] = null;
					continue;
				}

				PieceType pieceType = piece switch {
					Pawn => PieceType.Pawn,
					Rook => PieceType.Rook,
					Knight => PieceType.Knight,
					Bishop => PieceType.Bishop,
					Queen => PieceType.Queen,
					King => PieceType.King,
					_ => throw new System.ArgumentException($"Unknown piece type: {piece.GetType().Name}")
				};

				boardPieceTypes[file - 1, rank - 1] = new PieceVM {
					type = pieceType,
					side = piece.Owner
				};
			}
		}

		#endregion
	}
}
