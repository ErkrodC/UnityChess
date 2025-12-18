using System.Collections.Generic;

namespace UnityChess.Core {
	public static class SquareUtil {
		public static readonly Dictionary<string, int> FileCharToIndexMap = new() {
			{"a", 0},
			{"b", 1},
			{"c", 2},
			{"d", 3},
			{"e", 4},
			{"f", 5},
			{"g", 6},
			{"h", 7}
		};

		public static readonly Dictionary<int, string> FileIndexToCharMap = new() {
			{0, "a"},
			{1, "b"},
			{2, "c"},
			{3, "d"},
			{4, "e"},
			{5, "f"},
			{6, "g"},
			{7, "h"}
		};

		public static readonly Square[] KnightOffsets = {
			new(-2, -1),
			new(-2, 1),
			new(2, -1),
			new(2, 1),
			new(-1, -2),
			new(-1, 2),
			new(1, -2),
			new(1, 2),
		};

		public static readonly Square[] SurroundingOffsets = {
			new(-1, 0),
			new(1, 0),
			new(0, -1),
			new(0, 1),
			new(-1, 1),
			new(-1, -1),
			new(1, -1),
			new(1, 1),
		};

		public static readonly Square[] DiagonalOffsets = {
			new(-1, 1),
			new(-1, -1),
			new(1, -1),
			new(1, 1)
		};

		public static readonly Square[] CardinalOffsets = {
			new(-1, 0),
			new(1, 0),
			new(0, -1),
			new(0, 1),
		};



		public static string SquareToString(Square square) => SquareToString(square.File, square.Rank);
		public static string SquareToString(int file, int rank) {
			if (FileIndexToCharMap.TryGetValue(file, out string fileChar)) {
				return $"{fileChar}{rank + 1}";
			}

			return "Invalid";
		}

		public static Square StringToSquare(string squareText) {
			char fileChar = squareText[0];
			char rankChar = squareText[1];
			int file = FileCharToIndexMap[fileChar.ToString()];
			int rank = int.Parse(rankChar.ToString()) - 1;

			return new Square(file, rank);
		}
	}
}