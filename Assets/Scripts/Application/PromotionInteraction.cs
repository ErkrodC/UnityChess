using System;
using System.Threading;
using System.Threading.Tasks;
using UnityChess.Core;
using UnityChess.Util;

namespace UnityChess.Application {
	public sealed class PromotionInteraction : IDisposable {
		public Side requestingSide { get; private set; }
		public Task<ElectedPiece> task => _tcs.Task;

		private readonly TaskCompletionSource<ElectedPiece> _tcs = new (TaskCreationOptions.RunContinuationsAsynchronously);
		private CancellationTokenSource _cts = new();

		public PromotionInteraction(Side requestingSide) {
			this.requestingSide = requestingSide;
		}

		// ER TODO DragAndDropManipulator should probably be the one to register this
		public void RegisterCancelCallback(Action action) => _cts.Token.Register(action);

		public bool TryComplete(ElectedPiece electedPiece) => _tcs.TrySetResult(electedPiece);

		public bool TryCancel() {
			if (_cts == null) { return false; }

			_cts.Cancel();
			return _tcs.TrySetCanceled();
		}

		public void Dispose() {
			_cts?.Dispose();
			_cts = null;
		}
	}
}