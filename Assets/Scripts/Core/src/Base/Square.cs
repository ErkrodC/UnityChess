using System;

namespace UnityChess.Core {
	/// <summary>Representation of a square on a chessboard.</summary>
	[Serializable]
	public readonly struct Square {
		public static readonly Square Invalid = new Square(-1, -1);
		public readonly int File;
		public readonly int Rank;

		/// <summary>Creates a new Square instance.</summary>
		/// <param name="file">Column of the square.</param>
		/// <param name="rank">Row of the square.</param>
		public Square(int file, int rank) {
			File = file;
			Rank = rank;
		}

		public Square(string squareString) {
			this = string.IsNullOrEmpty(squareString)
				? Invalid
				: SquareUtil.StringToSquare(squareString);
		}

		internal Square(Square startPosition, int fileOffset, int rankOffset) {
			File = startPosition.File + fileOffset;
			Rank = startPosition.Rank + rankOffset;
		}

		internal readonly bool IsValid() {
			return File is >= 0 and <= 7
			       && Rank is >= 0 and <= 7;
		}

		public static bool operator ==(Square lhs, Square rhs) => lhs.File == rhs.File && lhs.Rank == rhs.Rank;
		public static bool operator !=(Square lhs, Square rhs) => !(lhs == rhs);
		public static Square operator +(Square lhs, Square rhs) => new Square(lhs.File + rhs.File, lhs.Rank + rhs.Rank);

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