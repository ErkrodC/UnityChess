using UnityEngine;

namespace UnityChess.Presentation {
	public static class GameObjectUtil {
		public static T GetOrCreateComponent<T>(this GameObject gameObject) where T : Component {
			if (!gameObject.TryGetComponent(out T component)) {
				component = gameObject.AddComponent<T>();
			}

			return component;
		}
	}
}