using System;
using UnityChess.Application;
using UnityChess.Application.Service;
using UnityChess.Core;
using UnityChess.Core.Util;
using UnityChess.DependencyInjection;
using UnityChess.Presentation.ViewModel;

namespace UnityChess.Presentation {
	public class MenuMediator : IMediator, IDisposable {
		private readonly GameManager _gameManager;
		private readonly MatchService _matchService;
		private readonly MenuVM _vm;

		public MenuMediator(GameManager gameManager, MatchService matchService, MenuVM vm) {
			_gameManager = gameManager;
			_matchService = matchService;
			_vm = vm;

			// To Presentation (subscriptions to application events)
			_gameManager.newGameStarted += OnNewGameStarted;
			_gameManager.gameEnded += OnGameEnded;

			// To Application (assignments to VM commands)
			_vm.onStartNewGameClicked = OnStartNewGameClicked;
			_vm.onLoadFENClicked = OnLoadFenClicked;
		}

		public void Dispose() {
			_gameManager.newGameStarted -= OnNewGameStarted;
			_gameManager.gameEnded -= OnGameEnded;
			_vm.onStartNewGameClicked = null;
			_vm.onLoadFENClicked = null;
		}

		#region To Presentation Layer

		private void OnNewGameStarted(Board board) {
			_vm.gameResult = string.Empty;
			// ER TODO: Convert board to FEN string
			//_vm.fenString = currentFEN;
		}

		private void OnGameEnded(Board board) {
			// ER TODO: Convert board to FEN string and set game result
			//_vm.fenString = currentFEN;
			//_vm.gameResult = result;
		}

		#endregion

		#region To Application Layer

		private void OnStartNewGameClicked() {
			MatchOptions matchOptions = new();

			ref MatchOptions.PlayerType playerType = ref _vm.playAsSide == Side.White
				? ref matchOptions.whitePlayerType
				: ref matchOptions.blackPlayerType;
			ref MatchOptions.PlayerType opponentType = ref _vm.playAsSide == Side.White
				? ref matchOptions.blackPlayerType
				: ref matchOptions.whitePlayerType;

			playerType = MatchOptions.PlayerType.Human;
			opponentType = _vm.opponentType;
			_matchService.StartMatch(matchOptions);
		}

		private void OnLoadFenClicked(string fen) {
			// ER TODO: Load FEN string
		}

		#endregion
	}
}
