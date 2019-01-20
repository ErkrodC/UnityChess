namespace UnityChess {
	/// <summary>Representation of a promotion move; inherits from SpecialMove.</summary>
	public class PromotionMove : SpecialMove {

		/// <summary>Creates a new PromotionMove instance; inherits from SpecialMove.</summary>
		/// <param name="pawnPosition">Position of the promoting pawn.</param>
		/// <param name="end">Square which the promoting pawn is landing on.</param>
		public PromotionMove(Square pawnPosition, Square end) : base(pawnPosition, end, null) { }

		/// <summary>Handles replacing the promoting pawn with the elected promotion piece.</summary>
		/// <param name="board">Board on which the move is being made.</param>
		public override void HandleAssociatedPiece(Board board) => board[End] = AssociatedPiece;
	}
}