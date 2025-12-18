namespace UnityChess.Core {
	public interface IGameSerializer {
		string Serialize(Game game);

		Game Deserialize(string gameString);
	}
}