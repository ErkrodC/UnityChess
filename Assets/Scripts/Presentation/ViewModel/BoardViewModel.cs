using System;
using System.Collections.Generic;
using UnityChess.Core;

namespace UnityChess.Presentation.ViewModel {
	[Serializable]
	// ER TODO avoid volatile domain types in view models
	public class BoardViewModel {
		public Board currentBoard { get; set; }
		public Square? selectedSquare { get; set; }
		public List<Square> highlightedSquares { get; set; }
	}
}