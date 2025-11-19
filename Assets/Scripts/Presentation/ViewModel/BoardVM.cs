using System;
using System.Collections.Generic;
using UnityChess.Core;

namespace UnityChess.Presentation.ViewModel {
	[Serializable]
	public class BoardVM {
		public PieceVM[,] currentBoard { get; set; }
		public Square? selectedSquare { get; set; }
		public List<Square> highlightedSquares { get; set; }
		public Side currentSideToMove { get; set; }
		public float whiteTimeRemaining { get; set; }
		public float blackTimeRemaining { get; set; }

		public Action<Square> onSquareClicked { get; set; }
	}
}