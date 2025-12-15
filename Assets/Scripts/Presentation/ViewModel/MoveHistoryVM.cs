using System;
using System.Collections.Generic;
using UnityChess.DependencyInjection;

namespace UnityChess.Presentation.ViewModel {
	public class MoveHistoryVM : IViewModel {
		public event Action EntriesChanged;

		public List<MoveHistoryEntryVM> moveEntries = new();
		public int currentHalfMoveIndex = -1;

		public Action onToBeginningClicked { get; set; }
		public Action onBackClicked { get; set; }
		public Action onForwardClicked { get; set; }
		public Action onToEndClicked { get; set; }
		public Action<int> onMoveClicked { get; set; }

		public void NotifyEntriesChanged() => EntriesChanged?.Invoke();
	}
}