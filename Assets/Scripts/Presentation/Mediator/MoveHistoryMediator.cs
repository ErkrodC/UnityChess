using System;
using UnityChess.Application;
using UnityChess.Core;
using UnityChess.DependencyInjection;
using UnityChess.Presentation.ViewModel;

namespace UnityChess.Presentation {
	public class MoveHistoryMediator : IMediator, IDisposable {
		private readonly MoveHistoryVM _vm;
		private readonly GameManager _gameManager;

		public MoveHistoryMediator(GameManager gameManager, MoveHistoryVM vm) {
			_gameManager = gameManager;
			_vm = vm;

			// Subscriptions to application events
			_gameManager.newGameStarted += OnNewGameStarted;
			_gameManager.moveExecuted += OnMoveExecuted;
			_gameManager.gameResetToHalfMove += OnGameResetToHalfMove;

			// Assignments of VM commands
			_vm.onToBeginningClicked = OnToBeginningClicked;
			_vm.onBackClicked = OnBackClicked;
			_vm.onForwardClicked = OnForwardClicked;
			_vm.onToEndClicked = OnToEndClicked;
			_vm.onMoveClicked = OnMoveClicked;
		}

		public void Dispose() {
			_gameManager.newGameStarted -= OnNewGameStarted;
			_gameManager.moveExecuted -= OnMoveExecuted;
			_gameManager.gameResetToHalfMove -= OnGameResetToHalfMove;
			_vm.onToBeginningClicked = null;
			_vm.onBackClicked = null;
			_vm.onForwardClicked = null;
			_vm.onToEndClicked = null;
			_vm.onMoveClicked = null;
		}

		#region Called From Application Layer

		private void OnNewGameStarted(Board board) {
			_vm.moveEntries.Clear();
			_vm.currentHalfMoveIndex = -1;
			_vm.NotifyEntriesChanged();
		}

		private void OnMoveExecuted(Board _, Timeline<HalfMove> halfMoveTimeline) {
			PopulateMoveEntries(halfMoveTimeline);
			_vm.NotifyEntriesChanged();
		}

		private void OnGameResetToHalfMove(Board _, Timeline<HalfMove> halfMoveTimeline) {
			PopulateMoveEntries(halfMoveTimeline);
			_vm.NotifyEntriesChanged();
		}

		#endregion

		#region Called From View Layer

		private void OnToBeginningClicked() {
			_gameManager.ResetGameToHalfMoveIndex(0);
		}

		private void OnBackClicked() {
			int targetIndex = Math.Max(_gameManager.GetCurrentHalfMoveIndex() - 1, 0);
			_gameManager.ResetGameToHalfMoveIndex(targetIndex);
		}

		private void OnForwardClicked() {
			int targetIndex = Math.Min(_gameManager.GetCurrentHalfMoveIndex() + 1, _gameManager.GetHalfMoveTimelineCount() - 1);
			_gameManager.ResetGameToHalfMoveIndex(targetIndex);
		}

		private void OnToEndClicked() {
			int targetIndex = Math.Max(_gameManager.GetHalfMoveTimelineCount() - 1, 0);
			_gameManager.ResetGameToHalfMoveIndex(targetIndex);
		}

		private void OnMoveClicked(int halfMoveIndex) {
			_gameManager.ResetGameToHalfMoveIndex(halfMoveIndex);
		}

		#endregion

		#region Helpers

		private void PopulateMoveEntries(Timeline<HalfMove> halfMoveTimeline) {
			_vm.moveEntries.Clear();

			for (int halfMoveIndex = 0; halfMoveIndex < halfMoveTimeline.Count; halfMoveIndex++) {
				HalfMove halfMove = halfMoveTimeline[halfMoveIndex];

				// White's Move (even half-move index)
				if (halfMoveIndex % 2 == 0) {
					_vm.moveEntries.Add(new MoveHistoryEntryVM {
						moveNumber = halfMoveIndex / 2 + 1,
						whiteMoveString = halfMove.ToAlgebraicNotation(),
						blackMoveString = null
					});
				} else { // Black's Move (odd half-move index)
					MoveHistoryEntryVM lastEntry = _vm.moveEntries[^1];
					lastEntry.blackMoveString = halfMove.ToAlgebraicNotation();
				}
			}

			_vm.currentHalfMoveIndex = halfMoveTimeline.HeadIndex;
		}

		#endregion
	}
}
