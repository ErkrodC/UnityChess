using System;
using System.Collections.Generic;
using UnityChess.Core;

namespace UnityChess.Presentation.ViewModel {
	[Serializable]
	public class BoardViewModel {
		public PieceVM[,] currentBoard { get; set; }
		public Square? selectedSquare { get; set; }
		public List<Square> highlightedSquares { get; set; }
	}
}