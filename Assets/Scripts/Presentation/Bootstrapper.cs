using UnityChess.DependencyInjection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace UnityChess.Presentation {
	[RequireComponent(typeof(UIDocument))]
	public partial class Bootstrapper : MonoBehaviour {
		[SerializeField] private SceneComposition composition;
		private DependencyRegistry _registry;

		partial void InstallComposition(string compositionName, DependencyRegistry registry);

		private void Awake() {
			_registry = new DependencyRegistry();
			DontDestroyOnLoad(gameObject);
			SceneManager.sceneLoaded += OnSceneLoaded;
			OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
		}

		private void OnSceneLoaded(Scene loadedScene, LoadSceneMode loadSceneMode) {
			if (composition == null) {
				Debug.LogError("Bootstrapper requires a SceneComposition reference.");
				return;
			}

			InstallComposition(composition.name, _registry);
		}

		// ER TODO here be a good spot to pass calls application layer from Unity Update, say for timers?
	}
}