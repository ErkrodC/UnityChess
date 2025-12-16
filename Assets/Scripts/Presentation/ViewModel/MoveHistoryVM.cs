using System;
using System.Collections.Generic;
using UnityChess.DependencyInjection;

namespace UnityChess.Presentation.ViewModel {
	public class MoveHistoryVM : IViewModel {
		public event Action EntriesChanged;

		public readonly List<MoveHistoryEntryVM> moveEntries = new();
		public int currentHalfMoveIndex = -1;

		public Action onToBeginningClicked;
		public Action onBackClicked;
		public Action onForwardClicked;
		public Action onToEndClicked;
		public Action<int> onMoveClicked;

		public void NotifyEntriesChanged() => EntriesChanged?.Invoke();
	}
}