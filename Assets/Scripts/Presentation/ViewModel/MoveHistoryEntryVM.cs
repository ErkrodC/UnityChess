using System;

namespace UnityChess.Presentation.ViewModel {
	[Serializable]
	public class MoveHistoryEntryVM {
		public int moveNumber;
		public string whiteMove;
		public string blackMove;
	}
}