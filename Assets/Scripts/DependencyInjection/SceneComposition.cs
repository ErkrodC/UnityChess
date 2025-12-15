using System;
using System.Collections.Generic;
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
			// Sort both list to ensure consistent serialization order
			includedMediatorGUIDs.Sort(StringComparer.Ordinal);
			includedViewGUIDs.Sort(StringComparer.Ordinal);
			EditorUtility.SetDirty(this);
		}
#endif
	}
}