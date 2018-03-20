using System.Collections.Generic;
using System.Net;
using UnityEngine;

public static class FileConverter {

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
	
	public static int FileToInt(string file) {
		return FileMap[file];
	}
	
	public static string IntToFile(int number) {
		return ReverseFileMap[number];
	}
}