using System;
using System.Collections.Generic;
using System.Linq;
using UnityChess.DependencyInjection;
using UnityEditor;
using UnityEngine;

namespace UnityChess.Editor {
	[CustomEditor(typeof(SceneComposition))]
	public class SceneCompositionEditor : UnityEditor.Editor {
		private List<(string guid, Type type)> _allMediatorTypes;
		private List<(string guid, Type type)> _allViewTypes;

		public override void OnInspectorGUI() {
			SceneComposition composition = (SceneComposition)target;

			// Discover all types on every inspector draw
			DiscoverAllTypes();

			serializedObject.Update();

			// Info box about code generation
			EditorGUILayout.HelpBox(
				"Check mediators and views to include in this composition. " +
				"Code generation will trigger automatically when you save this asset (Ctrl+S).",
				MessageType.Info);
			EditorGUILayout.Space();

			DrawMediatorsSection(composition);
			DrawViewsSection(composition);

			serializedObject.ApplyModifiedProperties();
		}

		private void DiscoverAllTypes() {
			_allMediatorTypes = new List<(string, Type)>();
			_allViewTypes = new List<(string, Type)>();

			// Find all MonoScript assets in the project
			string[] guids = AssetDatabase.FindAssets("t:MonoScript");

			foreach (string guid in guids) {
				string path = AssetDatabase.GUIDToAssetPath(guid);
				MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
				Type type = script?.GetClass();

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

		private void DrawMediatorsSection(SceneComposition composition) {
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Mediators", EditorStyles.boldLabel);

			if (_allMediatorTypes.Count == 0) {
				EditorGUILayout.HelpBox("No mediator types found implementing IMediator", MessageType.Info);
				return;
			}

			foreach (var (guid, type) in _allMediatorTypes) {
				// Check if this GUID is in the included list
				bool isIncluded = composition.includedMediatorGUIDs.Contains(guid);

				// Draw checkbox with type name
				EditorGUI.BeginChangeCheck();
				bool newIncluded = EditorGUILayout.Toggle(type.Name, isIncluded);
				if (EditorGUI.EndChangeCheck()) {
					Undo.RecordObject(composition, "Toggle Mediator");

					if (newIncluded && !isIncluded) {
						// Add to list
						composition.includedMediatorGUIDs.Add(guid);
					} else if (!newIncluded && isIncluded) {
						// Remove from list
						composition.includedMediatorGUIDs.Remove(guid);
					}

					EditorUtility.SetDirty(composition);
				}

				// Show discovered dependencies indented
				if (newIncluded) {
					var ctor = type.GetConstructors().FirstOrDefault();
					if (ctor != null) {
						EditorGUI.indentLevel++;
						foreach (var param in ctor.GetParameters()) {
							EditorGUILayout.LabelField($"→ {param.ParameterType.Name}", EditorStyles.miniLabel);
						}
						EditorGUI.indentLevel--;
					}
				}
			}
		}

		private void DrawViewsSection(SceneComposition composition) {
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Views", EditorStyles.boldLabel);

			if (_allViewTypes.Count == 0) {
				EditorGUILayout.HelpBox("No view types found implementing IView<T>", MessageType.Info);
				return;
			}

			foreach (var (guid, type) in _allViewTypes) {
				// Check if this GUID is in the included list
				bool isIncluded = composition.includedViewGUIDs.Contains(guid);

				// Draw checkbox with type name
				EditorGUI.BeginChangeCheck();
				bool newIncluded = EditorGUILayout.Toggle(type.Name, isIncluded);
				if (EditorGUI.EndChangeCheck()) {
					Undo.RecordObject(composition, "Toggle View");

					if (newIncluded && !isIncluded) {
						// Add to list
						composition.includedViewGUIDs.Add(guid);
					} else if (!newIncluded && isIncluded) {
						// Remove from list
						composition.includedViewGUIDs.Remove(guid);
					}

					EditorUtility.SetDirty(composition);
				}

				// Show discovered ViewModel indented
				if (newIncluded) {
					var viewInterface = type.GetInterfaces()
						.FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IView<>));
					if (viewInterface != null) {
						var vmType = viewInterface.GetGenericArguments()[0];
						EditorGUI.indentLevel++;
						EditorGUILayout.LabelField($"→ {vmType.Name}", EditorStyles.miniLabel);
						EditorGUI.indentLevel--;
					}
				}
			}
		}
	}
}
