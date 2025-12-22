using System;
using UnityChess.Application;
using UnityChess.Application.Service;
using UnityChess.Core;
using UnityChess.DependencyInjection;
using UnityChess.Presentation.ViewModel;

namespace UnityChess.Presentation {
	public class MenuMediator : IMediator, IDisposable {
		private readonly GameManager _gameManager;
		private readonly MatchService _matchService;
		private readonly MenuVM _menuVM;

		public MenuMediator(GameManager gameManager, MatchService matchService, MenuVM menuVM) {
			_gameManager = gameManager;
			_matchService = matchService;
			_menuVM = menuVM;

			// To Presentation (subscriptions to application events)
			_gameManager.newGameStarted += OnNewGameStarted;
			_gameManager.gameEnded += OnGameEnded;

			// To Application (assignments to VM commands)
			_menuVM.onStartNewGameClicked = OnStartNewGameClicked;
			_menuVM.onLoadFENClicked = OnLoadFenClicked;
		}

		public void Dispose() {
			_gameManager.newGameStarted -= OnNewGameStarted;
			_gameManager.gameEnded -= OnGameEnded;
			_menuVM.onStartNewGameClicked = null;
			_menuVM.onLoadFENClicked = null;
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
			_matchService.StartMatch(new MatchOptions {
				whitePlayerType = MatchOptions.PlayerType.Human,
				blackPlayerType = MatchOptions.PlayerType.Human,
			});
		}

		private void OnLoadFenClicked(string fen) {
			// ER TODO: Load FEN string
		}

		#endregion
	}
}
