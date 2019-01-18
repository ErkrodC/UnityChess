namespace UnityChess {
	/// <summary>Representation of a move, namely a piece and its end square.</summary>
	public class Movement {
		public readonly Square Start;
		public readonly Square End;

		/// <summary>Creates a new Movement.</summary>
		/// <param name="piecePosition">Position of piece being moved.</param>
		/// <param name="end">Square which the piece will land on.</param>
		public Movement(Square piecePosition, Square end) {
			Start = piecePosition;
			End = end;
		}

		/// <summary>Copy constructor.</summary>
		internal Movement(Movement move) {
			Start = move.Start;
			End = move.End;
		}

		public static bool operator ==(Movement lhs, Movement rhs) => lhs.Start == rhs.Start && lhs.End == rhs.End;
		public static bool operator !=(Movement lhs, Movement rhs) => !(lhs == rhs);

		public override string ToString() => $"{Start}->{End}";
	}
}