using UnityChess.Application;
using UnityChess.Core;
using UnityChess.Presentation.ViewModel;

namespace UnityChess.Presentation {
	public class GamePresenter {
		private GameViewModel _viewModel;
		private GameManager _gameManager;

		public GamePresenter(GameManager gameManager, GameViewModel viewModel) {
			_gameManager = gameManager;
			_viewModel = viewModel;

			_gameManager.newGameStarted += (board) => {
				PieceVM[,] pieceVMs = new PieceVM[8, 8];
				ConvertBoardToPieceTypes(board, pieceVMs);

				_viewModel.boardVM.currentBoard = pieceVMs;
			};

			_gameManager.boardChanged += (board) => {
				ConvertBoardToPieceTypes(board, _viewModel.boardVM.currentBoard);
			};

			_gameManager.turnChanged += (sideToMove) => {
				_viewModel.turnVM.currentSideToMove = sideToMove;
			};

			_gameManager.moveExecuted += (halfMove) => {
				_viewModel.moveHistoryVM.halfMoves.Add(halfMove);
			};

			_gameManager.gameResetToHalfMove += (Timeline<HalfMove> halfMoveTimeline) => {
				// ER TODO update relevant view models with new half move timeline
				/*_viewModel.boardVM.currentBoard = board;
				_viewModel.turnVM.currentSideToMove = sideToMove;*/

				// ER TODO convert board to fen string
				//_viewModel.fenVM.fen = currentFEN;
			};

			_gameManager.gameEnded += (board) => {
				// ER TODO convert board to fen string
				//_viewModel.fenVM.fen = currentFEN;
			};
		}

		public void OnNewGameButtonClicked() => _gameManager.StartNewGame();

		private static void ConvertBoardToPieceTypes(Board board, PieceVM[,] boardPieceTypes) {
			for (int file = 1; file <= 8; file++) {
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
		}
	}
}