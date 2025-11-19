using UnityChess.Application;
using UnityChess.Core;
using UnityChess.Presentation.ViewModel;

namespace UnityChess.Presentation {
	public class MoveHistoryMediator {
		private readonly MoveHistoryVM _moveHistoryVM;
		private readonly GameManager _gameManager;

		public MoveHistoryMediator(GameManager gameManager, MoveHistoryVM moveHistoryVM) {
			_gameManager = gameManager;
			_moveHistoryVM = moveHistoryVM;

			// To Presentation
			_gameManager.newGameStarted += OnNewGameStarted;
			_gameManager.moveExecuted += OnMoveExecuted;
			_gameManager.gameResetToHalfMove += OnGameResetToHalfMove;

			// To Application
			_moveHistoryVM.onToBeginningClicked = OnToBeginningClicked;
			_moveHistoryVM.onBackClicked = OnBackClicked;
			_moveHistoryVM.onForwardClicked = OnForwardClicked;
			_moveHistoryVM.onToEndClicked = OnToEndClicked;
			_moveHistoryVM.onMoveClicked = OnMoveClicked;
		}

		#region To Presentation Layer

		private void OnNewGameStarted(Board board) {
			_moveHistoryVM.moveEntries.Clear();
			_moveHistoryVM.currentHalfMoveIndex = -1;
		}

		private void OnMoveExecuted(HalfMove halfMove) {
			int halfMoveIndex = _moveHistoryVM.currentHalfMoveIndex + 1;
			_moveHistoryVM.currentHalfMoveIndex = halfMoveIndex;

			// White's move (even half-move index)
			if (halfMoveIndex % 2 == 0) {
				_moveHistoryVM.moveEntries.Add(new MoveHistoryEntryVM {
					moveNumber = halfMoveIndex / 2 + 1,
					whiteMove = halfMove.ToAlgebraicNotation(),
					blackMove = null
				});
			} else { // Black's move (odd half-move index)
				var lastEntry = _moveHistoryVM.moveEntries[^1];
				lastEntry.blackMove = halfMove.ToAlgebraicNotation();
			}
		}

		private void OnGameResetToHalfMove(Timeline<HalfMove> halfMoveTimeline) {
			// ER TODO: Update move history view model with new half move timeline
		}

		#endregion

		#region To Application Layer

		private void OnToBeginningClicked() {
			// ER TODO: Reset to beginning of game
		}

		private void OnBackClicked() {
			// ER TODO: Go back one move
		}

		private void OnForwardClicked() {
			// ER TODO: Go forward one move
		}

		private void OnToEndClicked() {
			// ER TODO: Go to end of game
		}

		private void OnMoveClicked(int halfMoveIndex) {
			// ER TODO: Jump to specific move
		}

		#endregion
	}
}
