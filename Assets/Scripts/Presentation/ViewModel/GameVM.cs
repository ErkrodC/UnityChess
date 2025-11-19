using System;

namespace UnityChess.Presentation.ViewModel {
	[Serializable]
	public class GameVM {
		public BoardVM boardVM { get; set; }
		public MoveHistoryVM moveHistoryVM { get; set; }
		public MenuVM menuVM { get; set; }
	}
}