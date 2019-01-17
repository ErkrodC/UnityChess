namespace UnityChess {
	public abstract class SpecialMove : Movement {
		protected readonly Piece AssociatedPiece;

		protected SpecialMove(Square end, Piece piece, Piece associatedPiece) : base(piece, end) {
			AssociatedPiece = associatedPiece;
		}

		public abstract void HandleAssociatedPiece(Board board, Piece piece);
	}
}