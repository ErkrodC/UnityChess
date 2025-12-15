using Unity.Properties;
using UnityChess.Core;
using UnityChess.DependencyInjection;
using UnityChess.Presentation.ViewModel;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityChess.Presentation.View {
	public class TurnView : MonoBehaviour, IView<BoardVM> {
		//[SerializeField] private UIDocument uiDocument;

		public void Initialize(BoardVM vm, UIDocument uiDocument) {
			SetupBindings();
		}

		private void SetupBindings() {
		/*
			var root = uiDocument.rootVisualElement;

			{ // turn label binding and formatting
				var turnLabel = root.Q<Label>("turn-label");

				DataBinding binding = new DataBinding {
					dataSource = gameVM.boardVM,
					dataSourcePath = new PropertyPath(nameof(BoardVM.currentSideToMove)),
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
					dataSource = gameVM.boardVM,
					dataSourcePath = new PropertyPath(nameof(BoardVM.currentSideToMove)),
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
					dataSource = gameVM.boardVM,
					dataSourcePath = new PropertyPath(nameof(BoardVM.currentSideToMove)),
					bindingMode = BindingMode.ToTarget
				};

				ConverterGroup converters = new ConverterGroup(nameof(TurnView));
				converters.AddConverter<Side, DisplayStyle>(Converters.BlackActiveToDisplayStyle);
				binding.ApplyConverterGroupToUI(converters);

				blackIndicator.SetBinding("style.display", binding);
			}
		*/
		}
	}
}