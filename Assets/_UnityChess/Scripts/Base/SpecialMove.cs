namespace UnityChess {
	public abstract class SpecialMove : Movement {

		public SpecialMove(Square end, Piece piece, Piece associatedPiece) : base(end, piece) {
			AssociatedPiece = associatedPiece;
		}

		public Piece AssociatedPiece { get; set; }

		public abstract void HandleAssociatedPiece(Board board);
	}
}