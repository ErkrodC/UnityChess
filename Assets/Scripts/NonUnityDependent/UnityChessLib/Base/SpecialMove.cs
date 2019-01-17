namespace UnityChess {
	public abstract class SpecialMove : Movement {
		protected Piece AssociatedPiece { get; }

		protected SpecialMove(Square end, Piece piece, Piece associatedPiece) : base(end, piece) {
			AssociatedPiece = associatedPiece;
		}

		public abstract void HandleAssociatedPiece(Board board);
	}
}