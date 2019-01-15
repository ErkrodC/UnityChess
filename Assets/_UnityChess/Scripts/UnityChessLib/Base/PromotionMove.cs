namespace UnityChess {
	/// <summary>Representation of a promotion move; inherits from SpecialMove.</summary>
	public class PromotionMove : SpecialMove {
		
		/// <summary>Creates a new PromotionMove instance; inherits from SpecialMove.</summary>
		/// <param name="end">Square which the promoting pawn is landing on.</param>
		/// <param name="pawn">The promoting pawn.</param>
		/// <param name="election">The piece to promote the promoting pawn to.</param>
		public PromotionMove(Square end, Pawn pawn, ElectedPiece election) : base(end, pawn, ParseElection(election, end, pawn.Side)) { }

		private static Piece ParseElection(ElectedPiece election, Square end, Side side) {
			Piece piece = PieceUtil.PromotionMoveGeneratorMap[election](end, side);
			piece.HasMoved = true;
			piece.Position = end;
			return piece;
		}

		/// <summary>Handles replacing the promoting pawn with the elected promotion piece.</summary>
		/// <param name="board">Board on which the move is being made.</param>
		public override void HandleAssociatedPiece(Board board) => board.PlacePiece(AssociatedPiece, End);
	}
}