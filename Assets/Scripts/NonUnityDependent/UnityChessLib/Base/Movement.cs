namespace UnityChess {
	// TODO make this struct to avoid confusing behavior
	/// <summary>Representation of a move, namely a piece and its end square.</summary>
	public class Movement {
		public readonly Square Start;
		public readonly Square End;

		/// <summary>Creates a new Movement.</summary>
		/// <param name="piece">Piece being moved.</param>
		/// <param name="end">Square which the piece will land on.</param>
		public Movement(Piece piece, Square end) {
			Start = piece.Position;
			End = end;
		}

		//Used to improve readability
		internal Movement(Piece piece, int file, int rank) {
			Start = piece.Position;
			End = new Square(file, rank);
		}

		/// <summary>Copy constructor.</summary>
		internal Movement(Movement move) {
			Start = move.Start;
			End = move.End;
		}

		// TODO method may be wrong if .Contains uses ref equality. If so, need to use .Exists w/ lambda exp to check if a move with the fields of the passed move exists in the list
		/// <summary>Checks whether a move is legal on a given board/turn.</summary>
		/// <param name="currentTurnSide">Side of the player whose turn it currently is.</param>
		/// <param name="movingPiece">Piece that is being moved.</param>
		public bool IsLegal(Side currentTurnSide, Piece movingPiece) => movingPiece.PieceOwner == currentTurnSide && movingPiece.ValidMoves.Contains(this);

		public static bool operator ==(Movement lhs, Movement rhs) => lhs.Start == rhs.Start && lhs.End == rhs.End;
		public static bool operator !=(Movement lhs, Movement rhs) => !(lhs == rhs);

		public override string ToString() => $"{Start}->{End}";
	}
}