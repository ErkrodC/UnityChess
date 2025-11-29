using UnityChess.Application;
using UnityChess.Core;
using UnityChess.Presentation.ViewModel;

namespace UnityChess.Presentation {
	public class MoveHistoryMediator {
		private readonly MoveHistoryVM _vm;
		private readonly GameManager _gameManager;

		public MoveHistoryMediator(GameManager gameManager, MoveHistoryVM vm) {
			_gameManager = gameManager;
			_vm = vm;

			// To Presentation
			_gameManager.NewGameStarted += OnNewGameStarted;
			_gameManager.MoveExecuted += OnMoveExecuted;
			_gameManager.GameResetToHalfMove += OnGameResetToHalfMove;

			// To Application
			_vm.onToBeginningClicked = OnToBeginningClicked;
			_vm.onBackClicked = OnBackClicked;
			_vm.onForwardClicked = OnForwardClicked;
			_vm.onToEndClicked = OnToEndClicked;
			_vm.onMoveClicked = OnMoveClicked;
		}

		#region To Presentation Layer

		private void OnNewGameStarted(Board board) {
			_vm.moveEntries.Clear();
			_vm.currentHalfMoveIndex = -1;
			_vm.NotifyEntriesChanged();
		}

		private void OnMoveExecuted(Board _, HalfMove halfMove) {
			int halfMoveIndex = _vm.currentHalfMoveIndex + 1;
			_vm.currentHalfMoveIndex = halfMoveIndex;

			// White's Move (even half-Move index)
			if (halfMoveIndex % 2 == 0) {
				_vm.moveEntries.Add(new MoveHistoryEntryVM {
					moveNumber = halfMoveIndex / 2 + 1,
					whiteMove = halfMove.ToAlgebraicNotation(),
					blackMove = null
				});
			} else { // Black's Move (odd half-Move index)
				MoveHistoryEntryVM lastEntry = _vm.moveEntries[^1];
				lastEntry.blackMove = halfMove.ToAlgebraicNotation();
			}

			_vm.NotifyEntriesChanged();
		}

		private void OnGameResetToHalfMove(Timeline<HalfMove> halfMoveTimeline) {
			_vm.NotifyEntriesChanged();
		}

		#endregion

		#region To Application Layer

		private void OnToBeginningClicked() {
			// ER TODO: Reset to beginning of game
			_vm.NotifyEntriesChanged();
		}

		private void OnBackClicked() {
			// ER TODO: Go back one Move
			_vm.NotifyEntriesChanged();
		}

		private void OnForwardClicked() {
			// ER TODO: Go forward one Move
			_vm.NotifyEntriesChanged();
		}

		private void OnToEndClicked() {
			// ER TODO: Go to end of game
			_vm.NotifyEntriesChanged();
		}

		private void OnMoveClicked(int halfMoveIndex) {
			// ER TODO: Jump to specific Move
			_vm.NotifyEntriesChanged();
		}

		#endregion
	}
}
