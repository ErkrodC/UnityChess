namespace UnityChess {
	public struct Turn {
		public readonly Piece Piece;
		public readonly Movement Movement;

		public Turn(Piece piece, Movement movement) {
			Piece = piece;
			Movement = movement;
		}
	}
}
