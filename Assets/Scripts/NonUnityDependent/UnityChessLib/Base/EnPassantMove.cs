namespace UnityChess {
	/// <summary>Representation of an en passant move; inherits from SpecialMove.</summary>
	public class EnPassantMove : SpecialMove {
		/// <summary>Creates a new EnPassantMove instance; inherits from SpecialMove.</summary>
		/// <param name="attackingPawnPosition">Position of the attacking pawn.</param>
		/// <param name="end">Square on which the attacking pawn will land on.</param>
		/// <param name="capturedPawn">Pawn that is being captured via en passant.</param>
		public EnPassantMove(Square attackingPawnPosition, Square end, Pawn capturedPawn) : base(attackingPawnPosition, end, capturedPawn) { }

		/// <summary>Handles removing the captured pawn from the board.</summary>
		/// <param name="board">Board on which the move is being made.</param>
		public override void HandleAssociatedPiece(Board board) {
			board[AssociatedPiece.Position] = null;
		}
	}
}