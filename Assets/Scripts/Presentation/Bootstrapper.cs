using UnityChess.Application;
using UnityChess.DependencyInjection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace UnityChess.Presentation {
	[RequireComponent(typeof(UIDocument))]
	public partial class Bootstrapper : MonoBehaviour {
		[SerializeField] private SceneComposition composition;
		private ServiceRegistry _registry;

		partial void InstallComposition(string compositionName, ServiceRegistry registry);

		private void Awake() {
			_registry = new ServiceRegistry();
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

			// ER TODO: remove this, to be started via in-game menu
			// ER TODO: once thats done, also move registry object into InstallComposition call
			GameManager gameManager = _registry.Resolve<GameManager>();
			gameManager?.StartNewGame();
		}

		// ER TODO here be a good spot to pass calls application layer from Unity Update, say for timers?
	}
}