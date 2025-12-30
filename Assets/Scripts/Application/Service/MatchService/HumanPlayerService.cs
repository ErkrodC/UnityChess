using System;
using System.Threading.Tasks;
using UnityChess.Core;
using UnityChess.Core.Util;

namespace UnityChess.Application.Service {
	public class HumanPlayerService : IPlayerService {
		public event Action<MoveInteraction> moveRequested;
		public event Action<PromotionInteraction> electionRequested;

		private MoveInteraction _activeInteraction;

		public async Task<(Square start, Square end)> GetMoveAsync(string _) {
			MoveInteraction moveInteraction = new();
			_activeInteraction = moveInteraction;
			moveInteraction.RegisterCancelCallback(() => {
				if (ReferenceEquals(_activeInteraction, moveInteraction)) {
					_activeInteraction = null;
				}
			});
			moveRequested?.Invoke(moveInteraction);
			return await moveInteraction.getMoveTask;
		}

		public void ReportMoveValidity(bool isValid) {
			if (_activeInteraction == null) { return; }

			_activeInteraction.TryCompleteIsValidMove(isValid);
			_activeInteraction.Dispose();
			_activeInteraction = null;
		}

		public async Task<ElectedPiece> ElectPieceAsync(Side side) {
			using PromotionInteraction promotionInteraction = new(side);
			electionRequested?.Invoke(promotionInteraction);
			return await promotionInteraction.task;
		}
	}
}