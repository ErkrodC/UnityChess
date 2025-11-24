using System.Collections.Generic;
using UnityChess.Core;
using UnityChess.Presentation.Presentation.View;
using UnityEngine;
using UnityEngine.UIElements;
using UnityChess.Presentation.ViewModel;

namespace UnityChess.Presentation.View {
	public class BoardView : MonoBehaviour {
		[SerializeField] private UIDocument uiDocument;
		[SerializeField] private BoardVM vm; // Assign in Inspector or via Initialize

		private List<DragAndDropManipulator> _dragAndDropManipulators;

		private void Awake() {
			_dragAndDropManipulators = new List<DragAndDropManipulator>();
		}

		private void OnEnable() {
			SetupBindings();
		}

		private void SetupBindings() {
			VisualElement root = uiDocument.rootVisualElement;

			// Bind all 64 squares (a1-h8) to display pieces
			for (int file = 0; file < 8; file++) {
				for (int rank = 0; rank < 8; rank++) {
					BindSquare(root, file, rank);
				}
			}
		}

		private void BindSquare(VisualElement root, int file, int rank) {
			string squareName = SquareUtil.SquareToString(file, rank); // e.g., "a1", "e4", etc.
			Label squareLabel = root.Q<VisualElement>(squareName).Q<Label>();
			_dragAndDropManipulators.Add(new DragAndDropManipulator(squareLabel, root));

			if (squareLabel == null) { return; }

			DataBinding binding = new() {
				dataSource = vm.currentBoard[file, rank],
				bindingMode = BindingMode.ToTarget
			};

			// Create converter that extracts the piece at this specific square
			ConverterGroup converters = new($"{nameof(BoardView)}-{squareName}");

			converters.AddConverter<PieceVM, string>(Converters.PieceToTextArt);
			binding.ApplyConverterGroupToUI(converters);

			squareLabel.SetBinding(nameof(Label.text), binding);
		}
	}
}