using System;
using System.Threading;
using System.Threading.Tasks;
using UnityChess.Core;

namespace UnityChess.Application.Service {
	public sealed class MoveInteraction : IDisposable {
		public Task<(Square start, Square end)> getMoveTask => _getMoveTcs.Task;
		public Task<bool> isMoveValidTask => _isMoveValidTcs.Task;

		private readonly TaskCompletionSource<(Square start, Square end)> _getMoveTcs = new (TaskCreationOptions.RunContinuationsAsynchronously);
		private readonly TaskCompletionSource<bool> _isMoveValidTcs = new (TaskCreationOptions.RunContinuationsAsynchronously);
		private CancellationTokenSource _cts = new();

		// ER TODO DragAndDropManipulator should probably be the one to register this
		public void RegisterCancelCallback(Action action) => _cts.Token.Register(action);

		public bool TryCompleteGetMove(Square start, Square end) => _getMoveTcs.TrySetResult((start, end));

		public bool TryCompleteIsValidMove(bool isValid) => _isMoveValidTcs.TrySetResult(isValid);

		public bool TryCancel() {
			if (_cts == null) { return false; }

			_cts.Cancel();
			_isMoveValidTcs.TrySetCanceled();
			return _getMoveTcs.TrySetCanceled();
		}

		public void Dispose() {
			_cts?.Dispose();
			_cts = null;
			_isMoveValidTcs.TrySetCanceled();
		}
	}
}