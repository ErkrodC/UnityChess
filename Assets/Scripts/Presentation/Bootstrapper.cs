using UnityChess.Application;
using UnityChess.DependencyInjection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static UnityChess.DependencyInjection.DependencyRegistry;

namespace UnityChess.Presentation {
	[RequireComponent(typeof(UIDocument))]
	public partial class Bootstrapper : MonoBehaviour {
		[SerializeField] private SceneComposition _composition;
		private DependencyRegistry _registry;

		partial void InstallComposition(string compositionName, DependencyRegistry registry);

		private void Awake() {
			if (_composition == null) {
				Debug.LogError($"Bootstrapper requires a {nameof(SceneComposition)} reference.");
				return;
			}

			_registry = new DependencyRegistry();
			SceneManager.sceneLoaded += OnSceneLoaded;
			SceneManager.sceneUnloaded += OnSceneUnloaded;
			DontDestroyOnLoad(gameObject);

			OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
		}

		private void OnDestroy() {
			SceneManager.sceneLoaded -= OnSceneLoaded;
			SceneManager.sceneUnloaded -= OnSceneUnloaded;
		}

		private void OnSceneLoaded(Scene loadedScene, LoadSceneMode loadSceneMode) {
			InstallComposition(_composition.name, _registry);

			// ER TODO remove
			_registry.Resolve<GameManager>().StartNewGame();
		}

		private void OnSceneUnloaded(Scene unloadedScene) {
			_registry.EndScope(Scope.Scene);
		}

		// ER TODO here be a good spot to pass calls application layer from Unity Update, say for timers?
	}
}