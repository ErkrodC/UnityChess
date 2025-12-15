using Unity.Properties;
using UnityChess.Core;
using UnityChess.Presentation.ViewModel;
using UnityChess.Util;
using UnityEngine.UIElements;

namespace UnityChess.Presentation.View {
	public class PromotionView : BaseView<PromotionVM> {
		public override void Initialize(PromotionVM vm) {
			SetupButtonBinding(vm, _root.Q<Button>("knight-election-button"), ElectedPiece.Knight);
			SetupButtonBinding(vm, _root.Q<Button>("bishop-election-button"), ElectedPiece.Bishop);
			SetupButtonBinding(vm, _root.Q<Button>("rook-election-button"), ElectedPiece.Rook);
			SetupButtonBinding(vm, _root.Q<Button>("queen-election-button"), ElectedPiece.Queen);
			SetupPanelBinding(vm, _root.Q<VisualElement>("promotion-panel"));
			_root.Q<Button>("promotion-cancel-button").clicked += () => vm.OnCancelled?.Invoke();
		}

		private void SetupButtonBinding(PromotionVM vm, Button button, ElectedPiece piece) {
			DataBinding binding = new() {
				dataSource = vm,
				dataSourcePath = new PropertyPath(nameof(PromotionVM.requestingSide)),
				bindingMode = BindingMode.ToTarget
			};

			ConverterGroup converters = new($"{nameof(PromotionView)}-{piece.ToString()}-button");
			converters.AddConverter((ref Side side) => Converters.ElectedPieceToTextArt(piece, side));
			binding.ApplyConverterGroupToUI(converters);

			button.SetBinding(nameof(Button.text), binding);
			button.clicked += () => vm.OnPieceElected?.Invoke(piece);
		}

		private void SetupPanelBinding(PromotionVM vm, VisualElement promotionPanel) {
			DataBinding binding = new() {
				dataSource = vm,
				dataSourcePath = new PropertyPath(nameof(PromotionVM.isRequesting)),
				bindingMode = BindingMode.ToTarget
			};

			promotionPanel.SetBinding(nameof(VisualElement.visible), binding);
		}
	}
}