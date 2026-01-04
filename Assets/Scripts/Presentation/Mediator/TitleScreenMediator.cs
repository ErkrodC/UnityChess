using System;
using UnityChess.Application;
using UnityChess.DependencyInjection;
using UnityChess.Presentation.ViewModel;

namespace UnityChess.Presentation {
	public class TitleScreenMediator : IMediator, IDisposable {
		private readonly SessionManager _sessionManager;
		private readonly TitleScreenVM _vm;

		public TitleScreenMediator(SessionManager sessionManager, TitleScreenVM vm) {
			_sessionManager = sessionManager;
			_vm = vm;

			// Subscriptions to application events

			// Assignments of VM commands
			_vm.onContinueClicked = OnContinueClicked;
		}

		public void Dispose() {
			throw new NotImplementedException();
		}

		#region Called From Application Layer

		#endregion

		#region Called From View Layer

		private void OnContinueClicked() {
			_sessionManager.OpenScene();
		}

		#endregion

		#region Helpers

		#endregion
	}
}