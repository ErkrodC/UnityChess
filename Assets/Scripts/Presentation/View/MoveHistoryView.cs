using UnityChess.DependencyInjection;
using UnityChess.Presentation.ViewModel;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityChess.Presentation.View {
	public class MoveHistoryView : MonoBehaviour, IView<MoveHistoryVM> {
		[SerializeField] private VisualTreeAsset _entryTemplate;

		public void Initialize(MoveHistoryVM vm, UIDocument uiDocument) {
			VisualElement root = uiDocument.rootVisualElement;

			root.Q<Button>("to-beginning-button").clicked += () => { vm.onToBeginningClicked?.Invoke(); };
			root.Q<Button>("back-button").clicked += () => { vm.onBackClicked?.Invoke(); };
			root.Q<Button>("forward-button").clicked += () => { vm.onForwardClicked?.Invoke(); };
			root.Q<Button>("to-end-button").clicked += () => { vm.onToEndClicked?.Invoke(); };

			SetupListViewBinding(vm, root.Q<ListView>("move-entries-list"));
		}

		private void SetupListViewBinding(MoveHistoryVM vm, ListView entriesListView) {
			vm.EntriesChanged += entriesListView.RefreshItems;

			entriesListView.makeItem = () => {
				TemplateContainer element = _entryTemplate.Instantiate();

				RegisterHalfMoveButtonClickHandler(vm, element.Q<Button>("white-move-button"));
				RegisterHalfMoveButtonClickHandler(vm, element.Q<Button>("black-move-button"));

				return element;
			};

			entriesListView.bindItem = (element, moveIndex) => {
				MoveHistoryEntryVM entry = vm.moveEntries[moveIndex];
				element.Q<Label>("move-number-label").text = $"{entry.moveNumber}.";

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

			entriesListView.itemsSource = vm.moveEntries;
		}

		private void RegisterHalfMoveButtonClickHandler(MoveHistoryVM vm, Button button) {
			button.clicked += () => vm.onMoveClicked?.Invoke((int)button.userData);
		}

		private static void UpdateHalfMoveButton(Button button, int halfMoveIndex, string buttonText) {
			button.text = buttonText;
			button.userData = halfMoveIndex;

			// ER TODO: if index == vm.currentHalfMoveIndex, show button highlight or whatever
		}
	}
}