using UnityChess.Application;
using UnityChess.DependencyInjection;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityChess.Presentation {
	[RequireComponent(typeof(UIDocument))]
	public partial class Bootstrapper : MonoBehaviour {
		[SerializeField] private SceneComposition composition;

		partial void InstallComposition(string compositionName, ServiceRegistry registry, GameObject viewRoot, UIDocument uiDocument);

		private void Awake() {
			if (composition == null) {
				Debug.LogError("Bootstrapper requires a SceneComposition reference.");
				return;
			}

			ServiceRegistry registry = new();
			InstallComposition(composition.name, registry, gameObject, GetComponent<UIDocument>());

			// ER TODO: remove this, to be start view in-game menu
			// Start managers (if they have a Start method)
			GameManager gameManager = registry.Resolve<GameManager>();
			gameManager?.Start();
		}

		// ER TODO here be a good spot to pass calls application layer from Unity Update, say for timers?
	}
}