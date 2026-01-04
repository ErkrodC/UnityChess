using System;
using UnityChess.Application;
using UnityChess.Application.Service;
using UnityChess.Core;
using UnityChess.Core.Util;
using UnityChess.DependencyInjection;
using UnityChess.Presentation.View;
using UnityChess.Presentation.ViewModel;

namespace UnityChess.Presentation {
	public class MenuMediator : IMediator, IDisposable {
		private const string _ACTIVE_PIECE_SET_KEY_KEY = "activePieceSetKey";
		private readonly GameManager _gameManager;
		private readonly PersistenceManager _persistenceManager;
		private readonly DLCManager _dlcManager;
		private readonly MatchService _matchService;
		private readonly MenuVM _menuVM;
		private readonly BoardVM _boardVM;

		public MenuMediator(
			GameManager gameManager,
			PersistenceManager persistenceManager,
			DLCManager dlcManager,
			MatchService matchService,
			MenuVM menuVM,
			BoardVM boardVM
		) {
			_gameManager = gameManager;
			_persistenceManager = persistenceManager;
			_dlcManager = dlcManager;
			_matchService = matchService;
			_menuVM = menuVM;
			_boardVM = boardVM;
			LoadPieceSetPreference();

			// Subscriptions to application events
			_gameManager.newGameStarted += OnNewGameStarted;
			_gameManager.gameEnded += OnGameEnded;

			// Assignments of VM commands
			_menuVM.onStartNewGameClicked = OnStartNewGameClicked;
			_menuVM.onLoadFENClicked = OnLoadFenClicked;
			_menuVM.onActivePieceSetKeyChanged = OnActivePieceSetKeyChanged;
		}

		public void Dispose() {
			_gameManager.newGameStarted -= OnNewGameStarted;
			_gameManager.gameEnded -= OnGameEnded;
			_menuVM.onStartNewGameClicked = null;
			_menuVM.onLoadFENClicked = null;
		}

		#region Called From Application Layer

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

		#region Called From View Layer

		private void OnStartNewGameClicked() {
			MatchOptions matchOptions = new();

			switch (_menuVM.playAsSide) {
				case Side.White: {
					matchOptions.whitePlayerType = MatchOptions.PlayerType.Human;
					matchOptions.blackPlayerType = _menuVM.opponentType;
					break;
				}
				case Side.Black: {
					matchOptions.whitePlayerType = _menuVM.opponentType;
					matchOptions.blackPlayerType = MatchOptions.PlayerType.Human;
					break;
				}
				default:
					throw new ArgumentOutOfRangeException(
						nameof(_menuVM.playAsSide),
						_menuVM.playAsSide,
						"Invalid side."
					);
			}

			_matchService.StartMatch(matchOptions);
		}

		private void OnLoadFenClicked(string fen) {
			// ER TODO: Load FEN string
		}

		private async void OnActivePieceSetKeyChanged(string pieceSetAddressablesKey) {
			if (_boardVM.activePieceSet.key == pieceSetAddressablesKey) { return; }

			try {
				_boardVM.activePieceSet = await _dlcManager.LoadAsync<PieceSetDefinition>(pieceSetAddressablesKey);
				_persistenceManager.Set(_ACTIVE_PIECE_SET_KEY_KEY, pieceSetAddressablesKey);
				_persistenceManager.Save();
			} catch {
				_persistenceManager.Set<string>(_ACTIVE_PIECE_SET_KEY_KEY, null);
				_persistenceManager.Save();
				throw;
			}
		}

		#endregion

		#region Helpers

		private async void LoadPieceSetPreference() {
			if (!_persistenceManager.TryGet(_ACTIVE_PIECE_SET_KEY_KEY, out string pieceSetKey)) { return; }

			try	  { _boardVM.activePieceSet = await _dlcManager.LoadAsync<PieceSetDefinition>(pieceSetKey); }
			catch { _boardVM.activePieceSet = null; }
		}

		#endregion
	}
}
