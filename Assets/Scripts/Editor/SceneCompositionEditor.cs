using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityChess.DependencyInjection;
using UnityChess.Presentation;
using UnityEditor;
using UnityEngine;

namespace UnityChess.Editor {
	[CustomEditor(typeof(SceneComposition))]
	public class SceneCompositionEditor : UnityEditor.Editor {
		private List<(string guid, Type type)> _allMediatorTypes;
		private List<(string guid, Type type)> _allViewTypes;

		public override void OnInspectorGUI() {
			SceneComposition composition = (SceneComposition)target;
			DiscoverAllTypes();

			serializedObject.Update();

			EditorGUILayout.HelpBox(
				"Check mediators and views to include in this composition. " +
				"Code generation will trigger automatically when you save this asset (Ctrl/Cmd+S).",
				MessageType.Info);
			EditorGUILayout.Space();

			DrawTypeIncludeSection("Mediators", composition, composition.includedMediatorGUIDs, _allMediatorTypes, ShowDiscoveredMediatorDependencies);
			DrawTypeIncludeSection("Views", composition, composition.includedViewGUIDs, _allViewTypes, ShowDiscoveredViewDependencies);

			serializedObject.ApplyModifiedProperties();
		}

		private void DiscoverAllTypes() {
			_allMediatorTypes = new List<(string, Type)>();
			_allViewTypes = new List<(string, Type)>();

			// Find all MonoScript assets in the project
			string[] guids = AssetDatabase.FindAssets("t:MonoScript");

			foreach (string guid in guids) {
				if (!EditorReflectionUtil.TryGetTypeByMonoScriptGuid(guid, out Type type, out string path)) {
					Debug.LogError($"Failed to get type for MonoScript GUID: {guid}, Path: {path}");
					continue;
				}

				if (type == null || type.IsAbstract) continue;

				// Check if it's a mediator
				if (typeof(IMediator).IsAssignableFrom(type)) {
					_allMediatorTypes.Add((guid, type));
				}

				// Check if it's a view (implements IView<T>)
				if (type.GetInterfaces().Any(i =>
					    i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IView<>))) {
					_allViewTypes.Add((guid, type));
				}
			}

			_allMediatorTypes.Sort((a, b) => string.Compare(a.type.Name, b.type.Name, StringComparison.Ordinal));
			_allViewTypes.Sort((a, b) => string.Compare(a.type.Name, b.type.Name, StringComparison.Ordinal));
		}

		private static void DrawTypeIncludeSection(string label, SceneComposition composition, List<string> compositionList,
			List<(string guid, Type type)> allRelevantTypes, Action<Type> showDiscoveredDependencies) {
			EditorGUILayout.Space();
			EditorGUILayout.LabelField(label, EditorStyles.boldLabel);

			if (allRelevantTypes.Count == 0) {
				EditorGUILayout.HelpBox("No mediator types found implementing IMediator", MessageType.Info);
				return;
			}

			foreach ((string guid, Type type) in allRelevantTypes) {
				// Check if this GUID is in the included list
				bool isIncluded = compositionList.Contains(guid);

				// Draw checkbox with type name
				EditorGUI.BeginChangeCheck();
				bool newIncluded = EditorGUILayout.Toggle(type.Name, isIncluded);
				if (EditorGUI.EndChangeCheck()) {
					Undo.RecordObject(composition, "Toggle Mediator");

					if (newIncluded && !isIncluded) {
						// Add to list
						compositionList.Add(guid);
						compositionList.Sort(StringComparer.Ordinal);
					} else if (!newIncluded && isIncluded) {
						// Remove from list
						compositionList.Remove(guid);
					}

					EditorUtility.SetDirty(composition);
				}

				// Show discovered dependencies indented
				if (newIncluded) {
					showDiscoveredDependencies(type);
				}
			}
		}

		private static void ShowDiscoveredMediatorDependencies(Type type) {
			ConstructorInfo ctor = type.GetConstructors().FirstOrDefault();
			if (ctor != null) {
				EditorGUI.indentLevel++;
				foreach (ParameterInfo param in ctor.GetParameters()) {
					EditorGUILayout.LabelField($"→ {param.ParameterType.Name}", EditorStyles.miniLabel);
				}
				EditorGUI.indentLevel--;
			}
		}

		private static void ShowDiscoveredViewDependencies(Type type) {
			Type viewInterface = type.GetInterfaces()
				.FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IView<>));
			if (viewInterface != null) {
				Type vmType = viewInterface.GetGenericArguments()[0];
				EditorGUI.indentLevel++;
				EditorGUILayout.LabelField($"→ {vmType.Name}", EditorStyles.miniLabel);
				EditorGUI.indentLevel--;
			}
		}
	}
}
