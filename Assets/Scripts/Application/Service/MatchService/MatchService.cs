using System;
using UnityChess.Core;

namespace UnityChess.Application.Service {
	public class MatchService {
		private readonly GameManager _gameManager;
		private readonly HumanPlayerService _humanPlayerService;
		private readonly AIPlayerService _aiPlayerService;
		private readonly GameSerializationService _gameSerializationService;

		public MatchService(GameManager gameManager,
			HumanPlayerService humanPlayerService,
			AIPlayerService aiPlayerService,
			GameSerializationService gameSerializationService
		) {
			_gameManager = gameManager;
			_humanPlayerService = humanPlayerService;
			_aiPlayerService = aiPlayerService;
			_gameSerializationService = gameSerializationService;
		}

		public void StartMatch(MatchOptions options, Game game = null) {
			IPlayerService whitePlayer = GetPlayerService(options.whitePlayerType);
			IPlayerService blackPlayer = GetPlayerService(options.blackPlayerType);

			_gameManager.StartNewGame(whitePlayer, blackPlayer, game);
		}

		public void LoadGame(string gameString, GameSerializationType serializationType, MatchOptions matchOptions) {
			StartMatch(
				matchOptions,
				_gameSerializationService.DeserializeGame(gameString, serializationType)
			);
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