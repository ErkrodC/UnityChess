using System;

namespace UnityChess.Application.Service {
	public class MatchService {
		private readonly GameManager _gameManager;

		public MatchService(GameManager gameManager) {
			_gameManager = gameManager;
		}

		public void StartMatch(MatchOptions options) {
			IPlayerService whitePlayer = CreatePlayer(options.whitePlayerType);
			IPlayerService blackPlayer = CreatePlayer(options.blackPlayerType);

			_gameManager.StartNewGame(whitePlayer, blackPlayer);
		}

		private static IPlayerService CreatePlayer(MatchOptions.PlayerType playerType) {
			return playerType switch {
				MatchOptions.PlayerType.Human => new HumanPlayerService(),
				MatchOptions.PlayerType.AI => new AIPlayerService(),
				MatchOptions.PlayerType.Networked => throw new NotImplementedException("Networked player type is not yet implemented."),
				_ => throw new ArgumentException($"Invalid player type: {playerType}", nameof(playerType))
			};
		}
	}
}