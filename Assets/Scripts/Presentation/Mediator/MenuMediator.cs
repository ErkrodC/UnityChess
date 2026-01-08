using System;
using UnityChess.Application;
using UnityChess.Application.Service;
using UnityChess.Core;
using UnityChess.Core.Util;
using UnityChess.DependencyInjection;
using UnityChess.Presentation.View;
using UnityChess.Presentation.ViewModel;
using UnityChess.Resource;

namespace UnityChess.Presentation {
	public class MenuMediator : IMediator, IDisposable {
		private const string _PIECE_SET_MANIFEST_KEY = "PieceSetManifest";
		private readonly GameManager _gameManager;
		private readonly PersistenceManager _persistenceManager;
		private readonly AssetManager _assetManager;
		private readonly MatchService _matchService;
		private readonly MenuVM _menuVM;
		private readonly BoardVM _boardVM;
		private IAssetRef<PieceSetDefinition> _activePieceSetRef;
		private IAssetRef<PieceSetManifest> _manifestRef;
		private MenuPreferences _menuPreferences;

		public MenuMediator(
			GameManager gameManager,
			PersistenceManager persistenceManager,
			AssetManager assetManager,
			MatchService matchService,
			MenuVM menuVM,
			BoardVM boardVM
		) {
			_gameManager = gameManager;
			_persistenceManager = persistenceManager;
			_assetManager = assetManager;
			_matchService = matchService;
			_menuVM = menuVM;
			_boardVM = boardVM;
			LoadPieceSetManifest();
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
			_activePieceSetRef?.Dispose();
			_manifestRef?.Dispose();
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

		private async void OnActivePieceSetKeyChanged(string pieceSetKey) {
			if (_boardVM.activePieceSet?.key == pieceSetKey) { return; }

			try {
				_activePieceSetRef?.Dispose();
				_activePieceSetRef = await _assetManager.LoadAsync<PieceSetDefinition>(pieceSetKey);
				_boardVM.activePieceSet = _activePieceSetRef.asset;

				_menuPreferences.activePieceSetKey = pieceSetKey;
				_persistenceManager.Save(_menuPreferences);
			} catch {
				_menuPreferences.activePieceSetKey = null;
				_persistenceManager.Save(_menuPreferences);
				throw;
			}
		}

		#endregion

		#region Helpers

		private async void LoadPieceSetManifest() {
			try {
				_manifestRef?.Dispose();
				_manifestRef = await _assetManager.LoadAsync<PieceSetManifest>(_PIECE_SET_MANIFEST_KEY);
				_menuVM.pieceSetManifest = _manifestRef.asset;
			} catch {
				_menuVM.pieceSetManifest = null;
			} finally {
				_menuVM.NotifyPieceSetManifestReady();
			}
		}

		private async void LoadPieceSetPreference() {
			if (!_persistenceManager.TryLoad(out _menuPreferences)) { return; }

			try {
				_activePieceSetRef?.Dispose();
				_activePieceSetRef = await _assetManager.LoadAsync<PieceSetDefinition>(_menuPreferences.activePieceSetKey);
				_boardVM.activePieceSet = _activePieceSetRef.asset;
			}
			catch { _boardVM.activePieceSet = null; }
		}

		#endregion

		private struct MenuPreferences {
			public string activePieceSetKey;
		}
	}
}
