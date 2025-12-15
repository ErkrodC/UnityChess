using System.Collections.Generic;
using UnityChess.Core;
using UnityChess.DependencyInjection;
using UnityChess.Presentation.ViewModel;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityChess.Presentation.View {
	public class BoardView : MonoBehaviour, IView<BoardVM> {
		private List<DragAndDropManipulator> _dragAndDropManipulators = new();

		public void Initialize(BoardVM vm, UIDocument uiDocument) {
			VisualElement root = uiDocument.rootVisualElement;

			// Bind all 64 squares (a1-h8) to display pieces
			for (int file = 0; file < 8; file++) {
				for (int rank = 0; rank < 8; rank++) {
					BindSquare(vm, root, file, rank);
				}
			}
		}

		private void BindSquare(BoardVM vm, VisualElement root, int file, int rank) {
			string squareName = SquareUtil.SquareToString(file, rank); // e.g., "a1", "e4", etc.
			Label squareLabel = root.Q<VisualElement>(squareName).Q<Label>();
			_dragAndDropManipulators.Add(new DragAndDropManipulator(squareLabel, root, vm));

			if (squareLabel == null) { return; }

			DataBinding binding = new() {
				dataSource = vm.currentBoard[file, rank],
				bindingMode = BindingMode.ToTarget,
			};

			// Create converter that extracts the piece at this specific square
			ConverterGroup converters = new($"{nameof(BoardView)}-{squareName}");

			converters.AddConverter<PieceVM, string>(Converters.PieceVMToTextArt);
			binding.ApplyConverterGroupToUI(converters);

			squareLabel.SetBinding(nameof(Label.text), binding);
		}
	}
}