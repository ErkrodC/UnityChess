using System;
using System.Collections.Generic;

namespace UnityChess.Presentation.ViewModel {
	[Serializable]
	public class MoveHistoryVM {
		public List<MoveHistoryEntryVM> moveEntries { get; set; } = new List<MoveHistoryEntryVM>();
		public int currentHalfMoveIndex { get; set; } = -1;

		public Action onToBeginningClicked { get; set; }
		public Action onBackClicked { get; set; }
		public Action onForwardClicked { get; set; }
		public Action onToEndClicked { get; set; }
		public Action<int> onMoveClicked { get; set; }
	}
}