using UnityEngine;

public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviourSingleton<T> {
    public static T Instance { get; protected set; }

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
            throw new System.Exception("An instance of this singleton already exists.");
        }

        Instance = (T) this;
    }
}
