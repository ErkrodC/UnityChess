using System;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityChess.Presentation.Util;
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

			RemoveInvalidGUIDs(includedMediatorGUIDs, IsValidMediatorGUID, guid => $"For {nameof(SceneComposition)} asset \"{name}\", removed orphaned {nameof(IMediator)} GUID: {guid}");
			RemoveInvalidGUIDs(includedViewGUIDs, IsValidViewGUID, guid => $"For {nameof(SceneComposition)} asset \"{name}\", removed orphaned {typeof(IView<>).Name} GUID: {guid}");
		}

		private static bool IsSorted(List<string> list) {
			for (int i = 1; i < list.Count; i++) {
				if (StringComparer.Ordinal.Compare(list[i - 1], list[i]) > 0) {
					return false;
				}
			}
			return true;
		}

		private void RemoveInvalidGUIDs(List<string> guids, Func<string, bool> isValidGUID, Func<string, string> warningMessageFormatter) {
			bool removedGUIDs = false;
			for (int i = guids.Count - 1; i >= 0; i--) {
				string guid = guids[i];
				if (!isValidGUID(guid)) {
					Debug.LogWarning(warningMessageFormatter);
					guids.RemoveAt(i);
					removedGUIDs = true;
				}
			}

			if (removedGUIDs) {
				EditorUtility.SetDirty(this);
			}
		}

		private static bool IsValidMediatorGUID(string guid) {
			if (!EditorReflectionUtil.TryGetTypeByMonoScriptGuid(guid, out Type scriptType, out _)) {
				return false;
			}

			return typeof(IMediator).IsAssignableFrom(scriptType);
		}

		private static bool IsValidViewGUID(string guid) {
			if (!EditorReflectionUtil.TryGetTypeByMonoScriptGuid(guid, out Type scriptType, out _)) {
				return false;
			}

			// Check if type implements IView<> generic interface
			foreach (Type interfaceType in scriptType.GetInterfaces()) {
				if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition().Name == "IView`1") {
					return true;
				}
			}

			return false;
		}
#endif
	}
}