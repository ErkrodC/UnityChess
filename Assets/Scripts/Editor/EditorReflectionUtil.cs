using System;
using UnityEditor;

namespace UnityChess.Editor {
	public static class EditorReflectionUtil {
		public static bool TryGetTypeByMonoScriptGuid(string guid, out Type type, out string path) {
			path = AssetDatabase.GUIDToAssetPath(guid);
			type = AssetDatabase.LoadAssetAtPath<MonoScript>(path)?.GetClass();
			return type != null;
		}
	}
}