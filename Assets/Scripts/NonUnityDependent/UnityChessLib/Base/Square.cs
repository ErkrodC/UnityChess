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

		/// <summary>Copy constructor.</summary>
		internal Square(Square square) {
			File = square.File;
			Rank = square.Rank;
		}

		internal Square(Square startPosition, int fileOffset, int rankOffset) {
			File = startPosition.File + fileOffset;
			Rank = startPosition.Rank + rankOffset;
		}
		
		public Square(int oneDimensionalIndex) {
			File = oneDimensionalIndex % 10;
			Rank = (oneDimensionalIndex - File) / 10 - 1;
		}

		/// <summary>Checks if this Square is on the 8x8 center of a 120-length board array.</summary>
		internal bool IsValid() => 1 <= File && File <= 8 && 1 <= Rank && Rank <= 8;

		internal bool IsOccupied(Board board) => board.GetBasePiece(this) is Piece;

		/// <summary>Determines whether the square is occupied by a piece belonging to the given side.</summary>
		internal bool IsOccupiedBySide(Board board, Side side) => board.GetBasePiece(this) is Piece piece && piece.PieceOwner == side;

		/// <summary>Returns the 1-D analog of the Square, with regard to a 120-length board array.</summary>
		public int AsIndex() => FileRankAsIndex(File, Rank);

		public static int FileRankAsIndex(int file, int rank) => (rank + 1) * 10 + file;

		public static bool operator ==(Square lhs, Square rhs) => lhs.File == rhs.File && lhs.Rank == rhs.Rank;
		public static bool operator !=(Square lhs, Square rhs) => !(lhs == rhs);

		public override string ToString() => SquareUtil.FileRankToSquareString(File, Rank);
	}
}