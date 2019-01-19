namespace UnityChess {
	public abstract class SpecialMove : Movement {
		protected internal Piece AssociatedPiece;

		protected SpecialMove(Square piecePosition, Square end, Piece associatedPiece) : base(piecePosition, end) {
			AssociatedPiece = associatedPiece;
		}

		public abstract void HandleAssociatedPiece(Board board);
	}
}