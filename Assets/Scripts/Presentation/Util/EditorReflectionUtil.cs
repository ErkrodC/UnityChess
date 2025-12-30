using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityChess.Presentation.Util {
	public static class EditorReflectionUtil {
		public static bool TryGetTypeByMonoScriptGuid(string guid, out Type type, out string path) {
#if UNITY_EDITOR
			path = AssetDatabase.GUIDToAssetPath(guid);
			type = AssetDatabase.LoadAssetAtPath<MonoScript>(path)?.GetClass();
			return type != null;
#else
			type = null;
			path = null;
			return false;
#endif
		}
	}
}