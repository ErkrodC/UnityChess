using UnityEngine.UIElements;

namespace UnityChess.DependencyInjection {
	public interface IView<TVm> where TVm : class, IViewModel {
		public void Initialize(TVm vm, UIDocument uiDocument);
	}
}