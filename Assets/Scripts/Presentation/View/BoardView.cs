using System.Collections.Generic;
using UnityChess.Core;
using UnityChess.Presentation.ViewModel;
using UnityEngine.UIElements;

namespace UnityChess.Presentation.View {
	public class BoardView : BaseView<BoardVM> {
		private BoardVM _vm;
		private readonly List<DragAndDropManipulator> _dragAndDropManipulators = new();

		public override void Initialize(BoardVM vm) {
			_vm = vm;
			_vm.pieceSetChanged += PopulateBoardWithPieceSet;

			if (_vm.activePieceSet == null) {
				// ER TODO do some loading spinner or something
			} else {
				PopulateBoardWithPieceSet(_vm.activePieceSet);
			}
		}

		private void PopulateBoardWithPieceSet(PieceSetDefinition activePieceSet) {
			// Bind all 64 squares (a1-h8) to display pieces
			for (int file = 0; file < 8; file++) {
				for (int rank = 0; rank < 8; rank++) {
					BindSquare(file, rank);
				}
			}
		}

		private void BindSquare(int file, int rank) {
			string squareName = SquareUtil.SquareToString(file, rank); // e.g., "a1", "e4", etc.
			Image squareImage = _root.Q<VisualElement>(squareName).Q<Image>();
			_dragAndDropManipulators.Add(new DragAndDropManipulator(squareImage, _root, _vm));

			if (squareImage == null) { return; }

			DataBinding binding = new() {
				dataSource = _vm.currentBoard[file, rank],
				bindingMode = BindingMode.ToTarget,
			};

			// Create converter that extracts the piece at this specific square
			ConverterGroup converters = new($"{nameof(BoardView)}-{squareName}");

			//converters.AddConverter<PieceVM, string>(Converters.PieceVMToTextArt);
			converters.AddConverter((ref PieceVM piece) => _vm.activePieceSet.GetSprite(piece.type, piece.side));
			binding.ApplyConverterGroupToUI(converters);

			squareImage.SetBinding(nameof(Image.sprite), binding);
		}
	}
}