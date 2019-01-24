namespace UnityChess {
	/// <summary>Representation of a square on a chessboard.</summary>
	public struct Square {
		public static readonly Square Invalid = new Square(-1, -1);
		public readonly int File;
		public readonly int Rank;
		internal bool IsValid => 1 <= File && File <= 8 && 1 <= Rank && Rank <= 8;

		/// <summary>Creates a new Square instance.</summary>
		/// <param name="file">Column of the square.</param>
		/// <param name="rank">Row of the square.</param>
		public Square(int file, int rank) {
			File = file;
			Rank = rank;
		}

		internal Square(Square startPosition, int fileOffset, int rankOffset) {
			File = startPosition.File + fileOffset;
			Rank = startPosition.Rank + rankOffset;
		}

		//public static int FileRankAsIndex(int file, int rank) => (rank + 1) * 10 + file;

		public static bool operator ==(Square lhs, Square rhs) => lhs.File == rhs.File && lhs.Rank == rhs.Rank;
		public static bool operator !=(Square lhs, Square rhs) => !(lhs == rhs);
		
		public bool Equals(Square other) => File == other.File && Rank == other.Rank;

		public bool Equals(int file, int rank) => File == file && Rank == rank;

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;

			return obj is Square other && Equals(other);
		}

		public override int GetHashCode() {
			unchecked {
				return (File * 397) ^ Rank;
			}
		}

		public override string ToString() => SquareUtil.SquareToString(this);
	}
}