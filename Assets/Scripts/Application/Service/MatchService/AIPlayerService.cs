using System;
using System.Threading.Tasks;
using UnityChess.Core;
using UnityChess.Core.Util;
using UnityChess.Resource;
using UnityChess.ResourceAccess;

namespace UnityChess.Application.Service {
	public class AIPlayerService : IPlayerService, IDisposable {
		private readonly GameManager _gameManager;
		private readonly IUCIEngine _engine;

		public AIPlayerService(GameManager gameManager, ILogger logger, IResourcePathProvider resourcePathProvider) {
			_gameManager = gameManager;
			// ER TODO should be selectable
			_engine = new PigeonUCIEngine(logger, resourcePathProvider);

			_gameManager.newGameStarted += OnNewGameStarted;
		}

		public void Dispose() {
			if (_engine is IDisposable disposable) { disposable.Dispose(); }

			_gameManager.newGameStarted -= OnNewGameStarted;
		}

		public async Task<(Square start, Square end)> GetMoveAsync(string fen) {
			return await _engine.GetBestMove(fen, 10_000);
		}

		public Task<ElectedPiece> ElectPieceAsync(Side side) {
			return Task.FromResult(_engine.promotionElection);
		}

		public void ReportMoveValidity(bool isValid) { /*no-op*/ }

		private async void OnNewGameStarted(Board board) {
			await _engine.StartNewGameAsync();
		}
	}
}