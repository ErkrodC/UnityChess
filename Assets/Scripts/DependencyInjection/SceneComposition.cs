using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityChess.DependencyInjection {
	[CreateAssetMenu(fileName = "SceneComposition", menuName = "ScriptableObjects/SceneComposition")]
	public class SceneComposition : ScriptableObject {
		public List<string> includedMediatorGUIDs = new();
		public List<string> includedViewGUIDs = new();

#if UNITY_EDITOR
		private void OnValidate() {
			// Sort both HashSets by type name to ensure consistent serialization order
			SortGUIDsByTypeName(includedMediatorGUIDs);
			SortGUIDsByTypeName(includedViewGUIDs);
			EditorUtility.SetDirty(this);
		}

		private static void SortGUIDsByTypeName(List<string> guids) {
			if (guids.Count == 0) return;

			// Convert GUIDs to (guid, typeName) pairs
			List<string> sorted = guids
				.Select(guid => {
					string path = AssetDatabase.GUIDToAssetPath(guid);
					string typeName = path switch {
						{ Length: > 0 } => AssetDatabase.LoadAssetAtPath<MonoScript>(path)?.GetClass()?.Name,
						_ => null
					};
					return (guid, typeName: typeName ?? guid);
				})
				.OrderBy(pair => pair.guid)
				.Select(pair => pair.guid)
				.ToList();

			// Rebuild HashSet in sorted order
			guids.Clear();
			guids.AddRange(sorted);
		}
#endif
	}
}