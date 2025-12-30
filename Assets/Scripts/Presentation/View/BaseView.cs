using UnityChess.DependencyInjection;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityChess.Presentation.View {
	[RequireComponent(typeof(UIDocument))]
	public abstract class BaseView<T> : MonoBehaviour, IView<T> where T : class, IViewModel {
		protected VisualElement _root;

		private void Awake() {
			_root = GetComponent<UIDocument>().rootVisualElement;
		}

		public abstract void Initialize(T vm);
	}
}