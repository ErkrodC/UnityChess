using UnityChess.Application;
using UnityChess.Core;
using UnityChess.DependencyInjection;
using UnityChess.Presentation.ViewModel;
using UnityChess.Core.Util;

namespace UnityChess.Presentation {
	public class PromotionMediator : IMediator {
		private readonly GameManager _gameManager;
		private readonly PromotionVM _vm;
		private PromotionInteraction _interaction;

		public PromotionMediator(GameManager gameManager, PromotionVM vm) {
			_gameManager = gameManager;
			_vm = vm;
			_vm.requestingSide = Side.None;
			_vm.isRequesting = false;

			// To Presentation (subscriptions to application events)
			_gameManager.electionRequested += OnElectionRequested;

			// To Application (assignments to VM commands)
			_vm.onPieceElected = OnPieceElected;
			_vm.onCancelled = OnCancelled;
		}

		#region To Presentation Layer

		private void OnElectionRequested(PromotionInteraction interaction) {
			_interaction = interaction;
			_vm.requestingSide = _interaction.requestingSide;
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