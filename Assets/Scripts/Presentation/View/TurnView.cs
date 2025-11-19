using Unity.Properties;
using UnityChess.Core;
using UnityEngine;
using UnityEngine.UIElements;
using UnityChess.Presentation.ViewModel;

namespace UnityChess.Presentation.View {
	public class TurnView : MonoBehaviour {
		[SerializeField] private UIDocument uiDocument;
		[SerializeField] private GameViewModel viewModel; // Assign in Inspector or via Initialize

		private void OnEnable() {
			SetupBindings();
		}

		private void SetupBindings() {
			var root = uiDocument.rootVisualElement;

			{ // turn label binding and formatting
				var turnLabel = root.Q<Label>("turn-label");

				DataBinding binding = new DataBinding {
					dataSource = viewModel.turnVM,
					dataSourcePath = new PropertyPath(nameof(TurnViewModel.currentSideToMove)),
					bindingMode = BindingMode.ToTarget
				};

				ConverterGroup converters = new ConverterGroup(nameof(TurnView));
				converters.AddConverter<Side, string>(Converters.BoolToTurnText);
				binding.ApplyConverterGroupToUI(converters);

				turnLabel.SetBinding(nameof(Label.text), binding);
			}

			{ // white turn indicator visibility binding
				var whiteIndicator = root.Q<VisualElement>("white-turn-indicator");

				DataBinding binding = new DataBinding {
					dataSource = viewModel.turnVM,
					dataSourcePath = new PropertyPath(nameof(TurnViewModel.currentSideToMove)),
					bindingMode = BindingMode.ToTarget
				};

				ConverterGroup converters = new ConverterGroup(nameof(TurnView));
				converters.AddConverter<Side, DisplayStyle>(Converters.WhiteActiveToDisplayStyle);
				binding.ApplyConverterGroupToUI(converters);

				whiteIndicator.SetBinding("style.display", binding);
			}

			{ // black turn indicator visibility binding
				var blackIndicator = root.Q<VisualElement>("black-turn-indicator");

				DataBinding binding = new DataBinding {
					dataSource = viewModel.turnVM,
					dataSourcePath = new PropertyPath(nameof(TurnViewModel.currentSideToMove)),
					bindingMode = BindingMode.ToTarget
				};

				ConverterGroup converters = new ConverterGroup(nameof(TurnView));
				converters.AddConverter<Side, DisplayStyle>(Converters.BlackActiveToDisplayStyle);
				binding.ApplyConverterGroupToUI(converters);

				blackIndicator.SetBinding("style.display", binding);
			}
		}
	}
}