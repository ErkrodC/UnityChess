using System.Collections.Generic;
using UnityChess.Presentation;
using UnityChess.Presentation.View;
using UnityEditor;
using UnityEngine;

namespace UnityChess.Editor {
	public static class PieceSetManifestGenerator {
		private const string _MANIFEST_PATH = "Assets/Manifests/PieceSetManifest.asset";

		[MenuItem("Tools/Piece Sets/Regenerate Piece Set Manifest")]
		public static void Generate() {
			string[] guids = AssetDatabase.FindAssets($"t:{nameof(PieceSetDefinition)}");
			List<PieceSetManifest.Entry> entries = new();

			foreach (string guid in guids) {
				string path = AssetDatabase.GUIDToAssetPath(guid);
				PieceSetDefinition definition = AssetDatabase.LoadAssetAtPath<PieceSetDefinition>(path);

				entries.Add(new PieceSetManifest.Entry {
					displayName = definition.displayName,
					previewIcon = definition.previewIcon,
					key = definition.key
				});
			}

			PieceSetManifest manifest = ScriptableObject.CreateInstance<PieceSetManifest>();
			typeof(PieceSetManifest).GetProperty(
				nameof(manifest.entries),
				System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public
			)?.SetValue(manifest, entries);

			AssetDatabase.CreateAsset(manifest, _MANIFEST_PATH);
			AssetDatabase.SaveAssets();
		}
	}
}