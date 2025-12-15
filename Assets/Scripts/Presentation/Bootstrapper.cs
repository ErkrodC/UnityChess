using UnityChess.Application;
using UnityChess.DependencyInjection;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityChess.Presentation {
	[RequireComponent(typeof(UIDocument))]
	public partial class Bootstrapper : MonoBehaviour {
		[SerializeField] private SceneComposition composition;

		partial void InstallComposition(string compositionName, ServiceRegistry registry);

		private void Start() {
			if (composition == null) {
				Debug.LogError("Bootstrapper requires a SceneComposition reference.");
				return;
			}

			ServiceRegistry registry = new();
			InstallComposition(composition.name, registry);

			// ER TODO: remove this, to be started via in-game menu
			// ER TODO: once thats done, also move registry object into InstallComposition call
			GameManager gameManager = registry.Resolve<GameManager>();
			gameManager?.StartNewGame();
		}

		// ER TODO here be a good spot to pass calls application layer from Unity Update, say for timers?
	}
}