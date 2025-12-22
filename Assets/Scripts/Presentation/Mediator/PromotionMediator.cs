using System;
using UnityChess.Application.Service;
using UnityChess.Core;
using UnityChess.DependencyInjection;
using UnityChess.Presentation.ViewModel;
using UnityChess.Core.Util;

namespace UnityChess.Presentation {
	public class PromotionMediator : IMediator, IDisposable {
		private readonly PromotionVM _vm;
		private readonly HumanPlayerService _player;
		private PromotionInteraction _interaction;

		public PromotionMediator(HumanPlayerService player, PromotionVM vm) {
			_vm = vm;
			_vm.requestingSide = Side.None;
			_vm.isRequesting = false;
			_player = player;

			// To Presentation (subscriptions to application events)
			_player.electionRequested += OnElectionRequested;

			// To Application (assignments to VM commands)
			_vm.onPieceElected = OnPieceElected;
			_vm.onCancelled = OnCancelled;
		}

		public void Dispose() {
			_player.electionRequested -= OnElectionRequested;
			_vm.onPieceElected = null;
			_vm.onCancelled = null;
			_interaction?.TryCancel();
			_interaction = null;
		}

		#region To Presentation Layer

		private void OnElectionRequested(PromotionInteraction interaction) {
			_interaction?.TryCancel();
			_interaction = interaction;
			_vm.requestingSide = _interaction.requestingSide;
			_vm.isRequesting = true;
		}

		#endregion

		#region To Application Layer

		private void OnPieceElected(ElectedPiece electedPiece) {
			if (_interaction == null) { return; }

			_interaction.TryComplete(electedPiece);
			ClearInteractionState();
		}

		private void OnCancelled() {
			if (_interaction == null) { return; }

			_interaction.TryCancel();
			ClearInteractionState();
		}

		#endregion

		#region Helpers

		private void ClearInteractionState() {
			_interaction = null;
			_vm.requestingSide = Side.None;
			_vm.isRequesting = false;
		}

		#endregion
	}
}