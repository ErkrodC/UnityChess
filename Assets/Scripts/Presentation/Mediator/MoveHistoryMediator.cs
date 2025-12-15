using System;
using UnityChess.Application;
using UnityChess.Core;
using UnityChess.DependencyInjection;
using UnityChess.Presentation.ViewModel;

namespace UnityChess.Presentation {
	public class MoveHistoryMediator : IMediator {
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

		private void OnMoveExecuted(Board _, Timeline<HalfMove> halfMoveTimeline) {
			PopulateMoveEntries(halfMoveTimeline);
			_vm.NotifyEntriesChanged();
		}

		private void OnGameResetToHalfMove(Board _, Timeline<HalfMove> halfMoveTimeline) {
			PopulateMoveEntries(halfMoveTimeline);
			_vm.NotifyEntriesChanged();
		}

		#endregion

		#region To Application Layer

		private void OnToBeginningClicked() {
			_gameManager.ResetGameToHalfMoveIndex(0);
		}

		private void OnBackClicked() {
			int targetIndex = Math.Max(_gameManager.CurrentHalfMoveIndex - 1, 0);
			_gameManager.ResetGameToHalfMoveIndex(targetIndex);
		}

		private void OnForwardClicked() {
			int targetIndex = Math.Min(_gameManager.CurrentHalfMoveIndex + 1, _gameManager.HalfMoveTimelineCount - 1);
			_gameManager.ResetGameToHalfMoveIndex(targetIndex);
		}

		private void OnToEndClicked() {
			int targetIndex = Math.Max(_gameManager.HalfMoveTimelineCount - 1, 0);
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
