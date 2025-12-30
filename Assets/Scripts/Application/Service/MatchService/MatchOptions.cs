namespace UnityChess.Application.Service {
	public struct MatchOptions {
		public enum PlayerType { Human, AI, Networked }

		public PlayerType whitePlayerType;
		public PlayerType blackPlayerType;
		// ER TODO time controls, w/e else
	}
}