using UnityChess.DependencyInjection;
using UnityChess.DependencyInjection.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static UnityChess.DependencyInjection.ServiceRegistry;

namespace UnityChess.Presentation {
	[RequireComponent(typeof(UIDocument))]
	public partial class Bootstrapper : MonoBehaviour {
		[SerializeField] private SceneComposition _composition;
		private ServiceRegistry _registry;

		partial void InstallComposition(string compositionName, ServiceRegistry registry);

		private void Awake() {
			if (_composition == null) {
				Debug.LogError($"Bootstrapper requires a {nameof(SceneComposition)} reference.");
				return;
			}

			_registry = new ServiceRegistry();
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
		}

		private void OnSceneUnloaded(Scene unloadedScene) {
			_registry.EndScope(Scope.Scene);
		}
	}
}