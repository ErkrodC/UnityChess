namespace UnityChess {
	/// <summary>Representation of a castling move; inherits from SpecialMove.</summary>
	public class CastlingMove : SpecialMove {
		/// <summary>Creates a new CastlingMove instance.</summary>
		/// <param name="end">Square on which the king will land on.</param>
		/// <param name="king"></param>
		/// <param name="rook"></param>
		public CastlingMove(Square end, King king, Rook rook) : base(end, king, rook) { }

		/// <summary>Handles moving the associated rook to the correct position on the board.</summary>
		/// <param name="board">Board on which the move is being made.</param>
		/// <param name="piece">Piece that was first moved.</param>
		public override void HandleAssociatedPiece(Board board, Piece piece) {
			switch (AssociatedPiece.Position.File) {
				case 1: //queenside castling move
					board.MovePiece(new Movement(AssociatedPiece, AssociatedPiece.Position.File + 3, AssociatedPiece.Position.Rank));
					break;
				case 8: //kingside castling move
					board.MovePiece(new Movement(AssociatedPiece, AssociatedPiece.Position.File - 2, AssociatedPiece.Position.Rank));
					break;
			}
		}
	}
}