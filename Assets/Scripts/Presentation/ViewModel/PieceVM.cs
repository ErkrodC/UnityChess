using System;
using UnityChess.Core;

namespace UnityChess.Presentation.ViewModel {
	[Serializable]
	public class PieceVM {
		public PieceType type;
		public Side side;
	}
}