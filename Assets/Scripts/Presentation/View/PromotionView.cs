using Unity.Properties;
using UnityChess.Core;
using UnityChess.Presentation.ViewModel;
using UnityChess.Util;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityChess.Presentation.View {
	public class PromotionView : MonoBehaviour {
		[SerializeField] private UIDocument uiDocument;
		[SerializeField] private PromotionVM vm;

		private VisualElement _root;

		private void Awake() {
			_root = uiDocument.rootVisualElement;
		}

		private void OnEnable() {
			SetupButtonBinding("knight-election-button", ElectedPiece.Knight);
			SetupButtonBinding("bishop-election-button", ElectedPiece.Bishop);
			SetupButtonBinding("rook-election-button", ElectedPiece.Rook);
			SetupButtonBinding("queen-election-button", ElectedPiece.Queen);
			SetupCancelButtonBinding();
			SetupPanelBinding();
		}

		private void SetupButtonBinding(string buttonName, ElectedPiece piece) {
			Button button = _root.Q<Button>(buttonName);

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

		private void SetupCancelButtonBinding() {
			Button cancelButton = _root.Q<Button>("promotion-cancel-button");
			cancelButton.clicked += () => vm.OnCancelled?.Invoke();
		}

		private void SetupPanelBinding() {
			DataBinding binding = new() {
				dataSource = vm,
				dataSourcePath = new PropertyPath(nameof(PromotionVM.isRequesting)),
				bindingMode = BindingMode.ToTarget
			};

			_root.Q<VisualElement>("promotion-panel")
				.SetBinding(nameof(VisualElement.visible), binding);
		}
	}
}