using Unity.Properties;
using UnityChess.Core;
using UnityChess.Core.Util;
using UnityChess.Presentation.ViewModel;
using UnityEngine.UIElements;

namespace UnityChess.Presentation.View {
	public class PromotionView : BaseView<PromotionVM> {
		public override void Initialize(PromotionVM vm) {
			SetupButtonBinding(vm, _root.Q<Button>("knight-election-button"), ElectedPiece.Knight);
			SetupButtonBinding(vm, _root.Q<Button>("bishop-election-button"), ElectedPiece.Bishop);
			SetupButtonBinding(vm, _root.Q<Button>("rook-election-button"), ElectedPiece.Rook);
			SetupButtonBinding(vm, _root.Q<Button>("queen-election-button"), ElectedPiece.Queen);
			SetupPanelBinding(vm, _root.Q<VisualElement>("promotion-panel"));
			_root.Q<Button>("promotion-cancel-button").clicked += () => vm.onCancelled?.Invoke();
		}

		private static void SetupButtonBinding(PromotionVM vm, Button button, ElectedPiece piece) {
			DataBinding binding = new() {
				dataSource = vm,
				dataSourcePath = new PropertyPath(nameof(PromotionVM.requestingSide)),
				bindingMode = BindingMode.ToTarget
			};

			ConverterGroup converters = new($"{nameof(PromotionView)}-{piece.ToString()}-button");
			converters.AddConverter((ref Side side) => Converters.ElectedPieceToTextArt(piece, side));
			binding.ApplyConverterGroupToUI(converters);

			button.SetBinding(nameof(Button.text), binding);
			button.clicked += () => vm.onPieceElected?.Invoke(piece);
		}

		private static void SetupPanelBinding(PromotionVM vm, VisualElement promotionPanel) {
			DataBinding binding = new() {
				dataSource = vm,
				dataSourcePath = new PropertyPath(nameof(PromotionVM.isRequesting)),
				bindingMode = BindingMode.ToTarget
			};

			promotionPanel.SetBinding(nameof(VisualElement.visible), binding);
		}
	}
}