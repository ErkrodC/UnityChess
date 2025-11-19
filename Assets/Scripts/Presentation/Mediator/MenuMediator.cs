using UnityChess.Application;
using UnityChess.Core;
using UnityChess.Presentation.ViewModel;

namespace UnityChess.Presentation {
	public class MenuMediator {
		private readonly MenuVM _menuVM;
		private readonly GameManager _gameManager;

		public MenuMediator(GameManager gameManager, MenuVM menuVM) {
			_gameManager = gameManager;
			_menuVM = menuVM;

			// To Presentation
			_gameManager.newGameStarted += OnNewGameStarted;
			_gameManager.gameEnded += OnGameEnded;

			// To Application
			_menuVM.onStartNewGameClicked = OnStartNewGameClicked;
			_menuVM.onLoadFenClicked = OnLoadFenClicked;
		}

		#region To Presentation Layer

		private void OnNewGameStarted(Board board) {
			_menuVM.gameResult = string.Empty;
			// ER TODO: Convert board to FEN string
			//_menuVM.fenString = currentFEN;
		}

		private void OnGameEnded(Board board) {
			// ER TODO: Convert board to FEN string and set game result
			//_menuVM.fenString = currentFEN;
			//_menuVM.gameResult = result;
		}

		#endregion

		#region To Application Layer

		private void OnStartNewGameClicked() {
			_gameManager.StartNewGame();
		}

		private void OnLoadFenClicked(string fen) {
			// ER TODO: Load FEN string
		}

		#endregion
	}
}
