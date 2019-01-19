namespace UnityChess {
	/// <summary>Representation of a castling move; inherits from SpecialMove.</summary>
	public class CastlingMove : SpecialMove {
		/// <summary>Creates a new CastlingMove instance.</summary>
		/// <param name="kingPosition">Position of the king to be castled.</param>
		/// <param name="end">Square on which the king will land on.</param>
		/// <param name="rook">The rook associated with the castling move.</param>
		public CastlingMove(Square kingPosition, Square end, Rook rook) : base(kingPosition, end, rook) { }

		/// <summary>Handles moving the associated rook to the correct position on the board.</summary>
		/// <param name="board">Board on which the move is being made.</param>
		public override void HandleAssociatedPiece(Board board) {
			switch (AssociatedPiece.Position.File) {
				case 1: //queenside castling move
					board.MovePiece(new Movement(AssociatedPiece.Position, new Square(AssociatedPiece.Position, 3, 0)));
					break;
				case 8: //kingside castling move
					board.MovePiece(new Movement(AssociatedPiece.Position, new Square(AssociatedPiece.Position,-2, 0)));
					break;
			}
		}
	}
}