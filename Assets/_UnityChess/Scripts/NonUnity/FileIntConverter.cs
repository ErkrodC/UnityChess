using System.Collections.Generic;
using System.Net;
using UnityChess;
using UnityEngine;

public static class FileIntConverter {

	private static readonly Dictionary<string, int> FileMap = new Dictionary<string, int> {
		{"a", 1},
		{"b", 2},
		{"c", 3},
		{"d", 4},
		{"e", 5},
		{"f", 6},
		{"g", 7},
		{"h", 8}
	};
	
	private static readonly Dictionary<int, string> ReverseFileMap = new Dictionary<int, string> {
		{1, "a"},
		{2, "b"},
		{3, "c"},
		{4, "d"},
		{5, "e"},
		{6, "f"},
		{7, "g"},
		{8, "h"}
	};

	public static string SquareToString(Square square) {
		string toString = "";

		toString += IntToFileChar(square.File);
		toString += square.Rank.ToString();

		return toString;
	}

	public static Square StringToSquare(string squareText) {
	
		int file = FileCharToInt(squareText.Substring(0, 1));
		int rank = int.Parse(squareText.Substring(1, 1));

		return new Square(file, rank);
	}
	
	public static string IntToFileChar(int number) {
		return ReverseFileMap[number];
	}
	
	private static int FileCharToInt(string file) {
		return FileMap[file];
	}
}