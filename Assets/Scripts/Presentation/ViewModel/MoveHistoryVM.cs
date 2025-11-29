using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityChess.Presentation.ViewModel {
	[CreateAssetMenu(fileName = "MoveHistoryVM", menuName = "ScriptableObjects/MoveHistoryVM", order = 1)]
	public class MoveHistoryVM : ScriptableObject {
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