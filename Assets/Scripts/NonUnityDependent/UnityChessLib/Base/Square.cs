namespace UnityChess {
	/// <summary>Representation of a square on a chessboard.</summary>
	public struct Square {
		public readonly int File;
		public readonly int Rank;

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

		/// <summary>Checks if this Square is on the 8x8 center of a 120-length board array.</summary>
		internal bool IsValid() => 1 <= File && File <= 8 && 1 <= Rank && Rank <= 8;

		internal bool IsOccupied(Board board) => board[this] != null;

		/// <summary>Determines whether the square is occupied by a piece belonging to the given side.</summary>
		internal bool IsOccupiedBySide(Board board, Side side) {
			Piece piece = board[this];
			return piece != null && piece.Color == side;
		}

		//public static int FileRankAsIndex(int file, int rank) => (rank + 1) * 10 + file;

		public static bool operator ==(Square lhs, Square rhs) => lhs.File == rhs.File && lhs.Rank == rhs.Rank;
		public static bool operator !=(Square lhs, Square rhs) => !(lhs == rhs);
		
		public bool Equals(Square other) => File == other.File && Rank == other.Rank;

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