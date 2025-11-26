using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityChess.Core;
using UnityChess.Util;
using UnityEngine;

namespace UnityChess.Presentation.ViewModel {
	[CreateAssetMenu(fileName = "BoardVM", menuName = "ScriptableObjects/BoardVM", order = 1)]
	public class BoardVM : ScriptableObject {
		public PieceVM[,] currentBoard { get; set; } = CreateEmptyPieceVMArray();
		public Square? selectedSquare { get; set; }
		public List<Square> highlightedSquares { get; set; } = new();
		public Side currentSideToMove { get; set; }
		public float whiteTimeRemaining { get; set; }
		public float blackTimeRemaining { get; set; }

		public Func<string, string, Task<bool>> onPieceDropped { get; set; }

		private static PieceVM[,] CreateEmptyPieceVMArray() {
			PieceVM[,] result = new PieceVM[8, 8];

			for (int file = 0; file < 8; file++)
			for (int rank = 0; rank < 8; rank++) {
				result[file, rank] = new PieceVM();
			}

			return result;
		}
	}
}