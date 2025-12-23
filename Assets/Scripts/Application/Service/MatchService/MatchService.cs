using System;

namespace UnityChess.Application.Service {
	public class MatchService {
		private readonly GameManager _gameManager;
		private readonly HumanPlayerService _humanPlayerService;
		private readonly AIPlayerService _aiPlayerService;

		public MatchService(GameManager gameManager, HumanPlayerService humanPlayerService, AIPlayerService aiPlayerService) {
			_gameManager = gameManager;
			_humanPlayerService = humanPlayerService;
			_aiPlayerService = aiPlayerService;
		}

		public void StartMatch(MatchOptions options) {
			IPlayerService whitePlayer = GetPlayerService(options.whitePlayerType);
			IPlayerService blackPlayer = GetPlayerService(options.blackPlayerType);

			_gameManager.StartNewGame(whitePlayer, blackPlayer);
		}

		private IPlayerService GetPlayerService(MatchOptions.PlayerType playerType) {
			return playerType switch {
				MatchOptions.PlayerType.Human => _humanPlayerService,
				MatchOptions.PlayerType.AI => _aiPlayerService,
				MatchOptions.PlayerType.Networked => throw new NotImplementedException("Networked player type is not yet implemented."),
				_ => throw new ArgumentException($"Invalid player type: {playerType}", nameof(playerType))
			};
		}
	}
}