using System;

namespace UnityChess.Presentation.ViewModel {
	[Serializable]
	public class MoveHistoryEntryVM {
		public int moveNumber { get; set; }
		public string whiteMove { get; set; }
		public string blackMove { get; set; }
	}
}