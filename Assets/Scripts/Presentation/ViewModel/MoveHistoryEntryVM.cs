using System;

namespace UnityChess.Presentation.ViewModel {
	[Serializable]
	public class MoveHistoryEntryVM {
		public int moveNumber;
		public string whiteMoveString;
		public string blackMoveString;

		public int whiteHalfMoveIndex => moveIndex * 2;
		public int blackHalfMoveIndex => moveIndex * 2 + 1;
		private int moveIndex => moveNumber - 1;
	}
}