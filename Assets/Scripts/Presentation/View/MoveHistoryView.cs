using UnityChess.Presentation.ViewModel;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityChess.Presentation.View {
	public class MoveHistoryView : MonoBehaviour {
		[SerializeField] private UIDocument uiDocument;
		[SerializeField] private MoveHistoryVM vm;
		[SerializeField] private VisualTreeAsset entryTemplate;

		private VisualElement _root;
		private ListView _entriesListView;
		private void Awake() {
			_root = uiDocument.rootVisualElement;
			_entriesListView = _root.Q<ListView>("move-entries-list");

			_entriesListView.makeItem = entryTemplate.Instantiate;
			_entriesListView.bindItem = (element, index) => {
				MoveHistoryEntryVM entry = vm.moveEntries[index];
				element.Q<Label>("move-number-label").text = entry.moveNumber.ToString();
				element.Q<Button>("white-move-button").text = entry.whiteMove;
				element.Q<Button>("black-move-button").text = entry.blackMove;

				// ER TODO alpha on button to show that it's the current move, and disable clickability if it is
			};
			_entriesListView.itemsSource = vm.moveEntries;
		}

		private void OnEnable() {
			vm.EntriesChanged += OnEntriesChanged;
		}

		private void OnDisable() {
			vm.EntriesChanged -= OnEntriesChanged;
		}

		private void OnEntriesChanged() {
			_entriesListView.RefreshItems();
		}
	}
}