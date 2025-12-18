using UnityChess.Presentation.View;
using UnityChess.Presentation.ViewModel;
using UnityEngine.UIElements;

namespace UnityChess.Presentation.Presentation.View {
	public class TitleScreenView : BaseView<TitleScreenVM> {
		public override void Initialize(TitleScreenVM vm) {
			_root.Q<Button>("continue-button").clicked += vm.onContinueClicked;
		}
	}
}