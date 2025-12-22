using System;
using System.Threading.Tasks;
using UnityChess.Core;
using UnityChess.Core.Resource;
using UnityChess.Core.Util;

namespace UnityChess.Application.Service {
	public class AIPlayerService : IPlayerService {
		private readonly IUCIEngine _engine;

		public async Task<(Square start, Square end)> GetMoveAsync(string fen) {
			return await _engine.GetBestMove(fen, 10_000);
		}

		public Task<ElectedPiece> ElectPieceAsync(Side side) {
			throw new NotImplementedException();
		}

		public void ReportMoveValidity(bool isValid) {
			throw new NotImplementedException();
		}
	}
}