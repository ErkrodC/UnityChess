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

			_entriesListView.makeItem = () => {
				TemplateContainer element = entryTemplate.Instantiate();

				RegisterHalfMoveButtonClickHandler(element.Q<Button>("white-move-button"));
				RegisterHalfMoveButtonClickHandler(element.Q<Button>("black-move-button"));

				return element;
			};

			_entriesListView.bindItem = (element, moveIndex) => {
				MoveHistoryEntryVM entry = vm.moveEntries[moveIndex];
				element.Q<Label>("move-number-label").text = entry.moveNumber.ToString();

				UpdateHalfMoveButton(
					button: element.Q<Button>("white-move-button"),
					halfMoveIndex: entry.whiteHalfMoveIndex,
					buttonText: entry.whiteMoveString
				);

				UpdateHalfMoveButton(
					button: element.Q<Button>("black-move-button"),
					halfMoveIndex: entry.blackHalfMoveIndex,
					buttonText: entry.blackMoveString
				);
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

		private void RegisterHalfMoveButtonClickHandler(Button button) {
			button.clicked += () => vm.onMoveClicked?.Invoke((int)button.userData);
		}

		private static void UpdateHalfMoveButton(Button button, int halfMoveIndex, string buttonText) {
			button.text = buttonText;
			button.userData = halfMoveIndex;

			// ER TODO: if index == vm.currentHalfMoveIndex, show button highlight or whatever
		}
	}
}