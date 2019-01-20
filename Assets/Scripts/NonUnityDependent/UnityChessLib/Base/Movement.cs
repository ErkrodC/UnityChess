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
		
		protected bool Equals(Movement other) => Start == other.Start && End == other.End;
		
		public static bool operator ==(Movement lhs, Movement rhs) => rhs != null && lhs != null && lhs.Start == rhs.Start && lhs.End == rhs.End;
		public static bool operator !=(Movement lhs, Movement rhs) => !(lhs == rhs);

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			return GetType() == obj.GetType() && Equals((Movement) obj);
		}

		public override int GetHashCode() {
			unchecked {
				return (Start.GetHashCode() * 397) ^ End.GetHashCode();
			}
		}

		public override string ToString() => $"{Start}->{End}";
	}
}