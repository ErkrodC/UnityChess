using UnityEngine;

public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviourSingleton<T> {
	public static T Instance {
		get {
			if (!instance) {
				instance = FindAnyObjectByType<T>()
				           ?? new GameObject().AddComponent<T>();
			}

			return instance;
		}
	} private static T instance;
}
