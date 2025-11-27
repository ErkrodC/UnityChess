using UnityChess.Application;
using UnityChess.Core;
using UnityChess.Presentation.ViewModel;
using UnityChess.Util;

namespace UnityChess.Presentation {
	public class PromotionMediator {
		private readonly GameManager _gameManager;
		private PromotionInteraction _interaction;
		private PromotionVM _vm;

		public PromotionMediator(GameManager gameManager, PromotionVM vm) {
			_gameManager = gameManager;
			_vm = vm;

			// To Presentation
			_gameManager.ElectionRequested += OnElectionRequested;

			// To Application
			_vm.OnPieceElected = OnPieceElected;
			_vm.OnCancelled = OnCancelled;

			_vm.requestingSide = Side.None;
			_vm.isRequesting = false;
		}

		#region To Presentation Layer

		private void OnElectionRequested(PromotionInteraction interaction) {
			_interaction = interaction;
			_vm.requestingSide = _interaction.RequestingSide;
			_vm.isRequesting = true;
		}

		#endregion

		#region To Application Layer

		private void OnPieceElected(ElectedPiece electedPiece) {
			_interaction.TryComplete(electedPiece);
			_interaction = null;
			_vm.requestingSide = Side.None;
			_vm.isRequesting = false;
		}

		private void OnCancelled() {
			_interaction.TryCancel();
			_interaction = null;
			_vm.isRequesting = false;
		}

		#endregion

		#region Helpers

		#endregion
	}
}