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
			if (!IsSorted(includedMediatorGUIDs)) {
				includedMediatorGUIDs.Sort(StringComparer.Ordinal);
				EditorUtility.SetDirty(this);
			}

			if (!IsSorted(includedViewGUIDs)) {
				includedViewGUIDs.Sort(StringComparer.Ordinal);
				EditorUtility.SetDirty(this);
			}
		}

		private static bool IsSorted(List<string> list) {
			for (int i = 1; i < list.Count; i++) {
				if (StringComparer.Ordinal.Compare(list[i - 1], list[i]) > 0) {
					return false;
				}
			}
			return true;
		}
#endif
	}
}