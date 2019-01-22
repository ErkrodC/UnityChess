namespace UnityChess {
	public interface IGameStringInterchanger {
		string Export(Game game);
		Game Import(string gameString);
	}
}