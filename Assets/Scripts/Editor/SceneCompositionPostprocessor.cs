using System.Linq;
using UnityChess.DependencyInjection;
using UnityEditor;
using UnityEngine;

namespace UnityChess.Editor {
	// Asset postprocessor to detect SceneComposition asset changes and trigger regeneration
	public class SceneCompositionPostprocessor : AssetPostprocessor {
		private static void OnPostprocessAllAssets(
			string[] importedAssets,
			string[] deletedAssets,
			string[] movedAssets,
			string[] movedFromAssetPaths) {

			bool compositionChanged =
				(from path in importedAssets
					where path.EndsWith(".asset")
					select AssetDatabase.LoadAssetAtPath<SceneComposition>(path))
				.Any(composition => composition != null);

			// Regenerate if any composition changed
			if (compositionChanged) {
				Debug.Log("SceneComposition asset changed, regenerating installers...");
				CompositionGenerator.GenerateAllCompositions();
			}
		}
	}
}