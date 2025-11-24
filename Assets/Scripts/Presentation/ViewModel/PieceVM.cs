using System;
using UnityChess.Util;

namespace UnityChess.Presentation.ViewModel {
	[Serializable]
	public class PieceVM {
		public PieceType type;
		public Side side;
	}
}