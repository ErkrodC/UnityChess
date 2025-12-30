namespace UnityChess.Core {
	public abstract class SpecialMove : Movement {
		protected SpecialMove(Square piecePosition, Square end)
			: base(piecePosition, end) { }

		public abstract void HandleAssociatedPiece(Board board);
	}
}