using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityChess.Core;
using UnityChess.Util;
using UnityEngine;

namespace UnityChess.Presentation.ViewModel {
	[CreateAssetMenu(fileName = "BoardVM", menuName = "ScriptableObjects/BoardVM", order = 1)]
	public class BoardVM : ScriptableObject {
		public PieceVM[,] currentBoard { get; set; }
		public Square? selectedSquare { get; set; }
		public List<Square> highlightedSquares { get; set; }
		public Side currentSideToMove { get; set; }
		public float whiteTimeRemaining { get; set; }
		public float blackTimeRemaining { get; set; }

		public Func<Square, Square, Task<bool>> onPieceDropped { get; set; }
	}
}