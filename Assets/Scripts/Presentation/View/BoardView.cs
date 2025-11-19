using UnityChess.Core;
using UnityEngine;
using UnityEngine.UIElements;
using UnityChess.Presentation.ViewModel;

namespace UnityChess.Presentation.View {
	public class BoardView : MonoBehaviour {
		[SerializeField] private UIDocument uiDocument;
		[SerializeField] private GameViewModel viewModel; // Assign in Inspector or via Initialize

		private void OnEnable() {
			SetupBindings();
		}

		private void SetupBindings() {
			var root = uiDocument.rootVisualElement;

			// Bind all 64 squares (a1-h8) to display pieces
			for (int file = 1; file <= 8; file++) {
				for (int rank = 1; rank <= 8; rank++) {
					BindSquare(root, file, rank);
				}
			}
		}

		private void BindSquare(VisualElement root, int file, int rank) {
			Square square = new Square(file, rank);
			string squareName = square.ToString(); // e.g., "a1", "e4", etc.
			var squareElement = root.Q<Label>(squareName);

			if (squareElement == null) { return; }

			DataBinding binding = new DataBinding {
				dataSource = viewModel.boardVM.currentBoard[square],
				bindingMode = BindingMode.ToTarget
			};

			// Create converter that extracts the piece at this specific square
			ConverterGroup converters = new ConverterGroup($"{nameof(BoardView)}-{squareName}");

			converters.AddConverter<Piece, string>(Converters.PieceToTextArt);
			binding.ApplyConverterGroupToUI(converters);

			squareElement.SetBinding(nameof(Label.text), binding);
		}
	}
}