using System.Collections.Generic;
using UnityChess.Core;
using UnityChess.Presentation.ViewModel;
using UnityEngine.UIElements;

namespace UnityChess.Presentation.View {
	public class BoardView : BaseView<BoardVM> {
		private readonly List<DragAndDropManipulator> _dragAndDropManipulators = new();

		public override void Initialize(BoardVM vm) {
			// Bind all 64 squares (a1-h8) to display pieces
			for (int file = 0; file < 8; file++) {
				for (int rank = 0; rank < 8; rank++) {
					BindSquare(vm, file, rank);
				}
			}
		}

		private void BindSquare(BoardVM vm, int file, int rank) {
			string squareName = SquareUtil.SquareToString(file, rank); // e.g., "a1", "e4", etc.
			Image squareImage = _root.Q<VisualElement>(squareName).Q<Image>();
			_dragAndDropManipulators.Add(new DragAndDropManipulator(squareImage, _root, vm));

			if (squareImage == null) { return; }

			DataBinding binding = new() {
				dataSource = vm.currentBoard[file, rank],
				bindingMode = BindingMode.ToTarget,
			};

			// Create converter that extracts the piece at this specific square
			ConverterGroup converters = new($"{nameof(BoardView)}-{squareName}");

			//converters.AddConverter<PieceVM, string>(Converters.PieceVMToTextArt);
			converters.AddConverter((ref PieceVM piece) => vm.activePieceSet.GetSprite(piece.type, piece.side));
			binding.ApplyConverterGroupToUI(converters);

			squareImage.SetBinding(nameof(Image.sprite), binding);
		}
	}
}