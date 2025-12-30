using System.Collections.Generic;
using UnityChess.Core;

namespace UnityChess.Application.Service {
	public class GameSerializationService {
		public GameSerializationType selectedType;

		private readonly Dictionary<GameSerializationType, IGameSerializer> _serializers = new() {
			[GameSerializationType.FEN] = new FENSerializer(),
			[GameSerializationType.PGN] = new PGNSerializer()
		};

		public GameSerializationService() {
			selectedType = GameSerializationType.FEN;
		}

		public string SerializeGame(Game game, GameSerializationType? serializationTypeOverride = null) {
			GameSerializationType type = serializationTypeOverride ?? selectedType;
			return _serializers[type].Serialize(game);
		}

		public Game DeserializeGame(string serializedGame, GameSerializationType? serializationTypeOverride = null) {
			GameSerializationType type = serializationTypeOverride ?? selectedType;
			return _serializers[type].Deserialize(serializedGame);
		}
	}
}