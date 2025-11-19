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

			_gameManager.newGameStarted += () => {
				_viewModel.boardVM.currentBoard = new Board();
			};

			_gameManager.boardChanged += (board) => {
				_viewModel.boardVM.currentBoard = board;
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
	}
}