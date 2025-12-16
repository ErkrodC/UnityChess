using UnityEditor;
using UnityEditor.SceneTemplate;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using UnityChess.DependencyInjection;
using UnityChess.Presentation;
using UnityEditor.SceneManagement;

namespace UnityChess.Editor {
	public class GameSceneTemplatePipeline : ISceneTemplatePipeline {
		public virtual bool IsValidTemplateForInstantiation(SceneTemplateAsset sceneTemplateAsset) {
			return true;
		}

		public virtual void BeforeTemplateInstantiation(SceneTemplateAsset sceneTemplateAsset, bool isAdditive, string sceneName) {

		}

		public virtual void AfterTemplateInstantiation(SceneTemplateAsset sceneTemplateAsset, Scene scene, bool isAdditive, string sceneName) {
			EditorSceneManager.sceneSaved += OnSceneSaved;
		}

		private void OnSceneSaved(Scene scene) {
			// Get the scene path and directory
			string scenePath = scene.path;
			string sceneDirectory = Path.GetDirectoryName(scenePath) ?? throw new System.Exception("Scene directory is null");
			string sceneNameWithoutExtension = Path.GetFileNameWithoutExtension(scenePath);

			// Create the SceneComposition asset path
			string compositionPath = Path.Combine(sceneDirectory, sceneNameWithoutExtension + ".asset");

			// Check if SceneComposition asset already exists
			SceneComposition composition = AssetDatabase.LoadAssetAtPath<SceneComposition>(compositionPath);

			// Create the asset if it doesn't exist
			if (composition == null) {
				composition = ScriptableObject.CreateInstance<SceneComposition>();
				AssetDatabase.CreateAsset(composition, compositionPath);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}

			// Find Bootstrapper in the scene and assign the composition
			GameObject[] rootObjects = scene.GetRootGameObjects();
			foreach (GameObject rootObject in rootObjects) {
				Bootstrapper bootstrapper = rootObject.GetComponentInChildren<Bootstrapper>(true);
				if (bootstrapper != null) {
					SerializedObject serializedBootstrapper = new(bootstrapper);
					SerializedProperty compositionProperty = serializedBootstrapper.FindProperty("composition");
					compositionProperty.objectReferenceValue = composition;
					serializedBootstrapper.ApplyModifiedProperties();
					break;
				}
			}

			EditorSceneManager.sceneSaved -= OnSceneSaved;
		}
	}
}