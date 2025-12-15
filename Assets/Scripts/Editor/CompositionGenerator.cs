using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityChess.DependencyInjection;
using UnityEditor;
using UnityEngine;

namespace UnityChess.Editor {
	[InitializeOnLoad]
	public static class CompositionGenerator {
		static CompositionGenerator() {
			// Trigger generation after scripts compile
			EditorApplication.delayCall += GenerateAllCompositions;
		}

		[MenuItem("Tools/Regenerate DI Compositions")]
		public static void GenerateAllCompositions() {
			string[] guids = AssetDatabase.FindAssets("t:SceneComposition");
			List<string> compositionNames = new();

			foreach (string guid in guids) {
				string path = AssetDatabase.GUIDToAssetPath(guid);
				SceneComposition composition = AssetDatabase.LoadAssetAtPath<SceneComposition>(path);
				if (composition != null) {
					GenerateComposition(composition);
					compositionNames.Add(composition.name);
				}
			}

			// Generate the registry file with method dictionary
			GenerateRegistry(compositionNames);

			AssetDatabase.Refresh();
		}

		private static void GenerateComposition(SceneComposition composition) {
			string compositionName = composition.name;

			// Discover types
			List<Type> mediatorTypes = new();
			List<Type> viewTypes = new();
			HashSet<Type> managerTypes = new();
			HashSet<Type> viewModelTypes = new();

			// Resolve included mediator GUIDs to types
			foreach (string guid in composition.includedMediatorGUIDs) {
				if (!EditorReflectionUtil.TryGetTypeByMonoScriptGuid(guid, out Type type, out string path)) {
					Debug.LogError($"Failed to get type for MonoScript GUID: {guid}, Path: {path}");
					continue;
				}

				if (!typeof(IMediator).IsAssignableFrom(type)) {
					Debug.LogError($"Script at {path} does not implement IMediator");
					continue;
				}

				mediatorTypes.Add(type);

				// Inspect constructor to find managers and view models
				ConstructorInfo ctor = type.GetConstructors().FirstOrDefault();
				if (ctor != null) {
					foreach (ParameterInfo param in ctor.GetParameters()) {
						if (typeof(IManager).IsAssignableFrom(param.ParameterType)) {
							managerTypes.Add(param.ParameterType);
						} else if (typeof(IViewModel).IsAssignableFrom(param.ParameterType)) {
							viewModelTypes.Add(param.ParameterType);
						}
					}
				}
			}

			// Resolve included view GUIDs to types
			foreach (string guid in composition.includedViewGUIDs) {
				if (!EditorReflectionUtil.TryGetTypeByMonoScriptGuid(guid, out Type type, out string path)) {
					Debug.LogError($"Failed to get type for MonoScript GUID: {guid}, Path: {path}");
					continue;
				}

				Type viewInterface = type.GetInterfaces()
					.FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IView<>));

				if (viewInterface == null) {
					Debug.LogError($"Script at {path} does not implement IView<T>");
					continue;
				}

				viewTypes.Add(type);
				viewModelTypes.Add(viewInterface.GetGenericArguments()[0]);
			}

			// Generate code
			StringBuilder sb = new();
			sb.AppendLine("// This file is auto-generated. Do not modify manually.");
			sb.AppendLine("using UnityChess.DependencyInjection;");
			sb.AppendLine("using UnityEngine;");
			sb.AppendLine("using UnityEngine.UIElements;");

			// Add necessary using statements
			HashSet<string> namespaces = new();
			foreach (Type t in managerTypes.Concat(viewModelTypes).Concat(mediatorTypes).Concat(viewTypes)) {
				if (!string.IsNullOrEmpty(t.Namespace)) {
					namespaces.Add(t.Namespace);
				}
			}
			foreach (string ns in namespaces.OrderBy(x => x)) {
				sb.AppendLine($"using {ns};");
			}

			sb.AppendLine();
			sb.AppendLine("namespace UnityChess.Presentation {");
			sb.AppendLine("\tpublic partial class Bootstrapper {");
			sb.AppendLine($"\t\tprivate void Install{compositionName}(ServiceRegistry registry, GameObject viewRoot, UIDocument uiDocument) {{");

			// Register managers
			sb.AppendLine("\t\t\t// Register Managers");
			foreach (Type managerType in managerTypes.OrderBy(x => x.Name)) {
				sb.AppendLine($"\t\t\tregistry.RegisterSingleton<{managerType.Name}>(() => new {managerType.Name}());");
			}

			sb.AppendLine();

			// Register view models
			sb.AppendLine("\t\t\t// Register ViewModels");
			foreach (Type vmType in viewModelTypes.OrderBy(x => x.Name)) {
				sb.AppendLine($"\t\t\tregistry.RegisterSingleton<{vmType.Name}>(() => new {vmType.Name}());");
			}

			sb.AppendLine();

			// Register mediators
			sb.AppendLine("\t\t\t// Register Mediators");
			foreach (Type mediatorType in mediatorTypes.OrderBy(x => x.Name)) {
				ConstructorInfo ctor = mediatorType.GetConstructors()[0];
				ParameterInfo[] parameters = ctor.GetParameters();

				if (parameters.Length == 0) {
					sb.AppendLine($"\t\t\tregistry.RegisterSingleton<{mediatorType.Name}>(() => new {mediatorType.Name}());");
				} else {
					string resolveParams = string.Join(", ", parameters.Select(p => $"registry.Resolve<{p.ParameterType.Name}>()"));
					sb.AppendLine($"\t\t\tregistry.RegisterSingleton<{mediatorType.Name}>(() => new {mediatorType.Name}({resolveParams}));");
				}
			}

			sb.AppendLine();

			// Instantiate mediators (force resolution to run constructors)
			sb.AppendLine("\t\t\t// Instantiate Mediators");
			foreach (Type mediatorType in mediatorTypes.OrderBy(x => x.Name)) {
				sb.AppendLine($"\t\t\tregistry.Resolve<{mediatorType.Name}>();");
			}

			sb.AppendLine();

			// Initialize views
			sb.AppendLine("\t\t\t// Initialize Views");
			foreach (Type viewType in viewTypes.OrderBy(x => x.Name)) {
				Type viewInterface = viewType.GetInterfaces()
					.First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IView<>));
				Type vmType = viewInterface.GetGenericArguments()[0];

				sb.AppendLine($"\t\t\t{{");
				sb.AppendLine($"\t\t\t\t{viewType.Name} view = viewRoot.GetComponent<{viewType.Name}>();");
				sb.AppendLine($"\t\t\t\tif (view == null) {{ view = viewRoot.AddComponent<{viewType.Name}>(); }}");
				sb.AppendLine($"\t\t\t\t{vmType.Name} vm = registry.Resolve<{vmType.Name}>();");
				sb.AppendLine($"\t\t\t\tview.Initialize(vm, uiDocument);");
				sb.AppendLine($"\t\t\t}}");
			}

			sb.AppendLine("\t\t}");
			sb.AppendLine("\t}");
			sb.AppendLine("}");

			// Ensure directory exists
			string outputDir = "Assets/Scripts/Presentation/Generated";
			if (!Directory.Exists(outputDir)) {
				Directory.CreateDirectory(outputDir);
			}

			// Write to file
			string outputPath = $"{outputDir}/Bootstrapper.{compositionName}.Generated.cs";
			File.WriteAllText(outputPath, sb.ToString());

			Debug.Log($"Generated composition installer for {compositionName} at {outputPath}");
		}

		private static void GenerateRegistry(List<string> compositionNames) {
			StringBuilder sb = new();
			sb.AppendLine("// This file is auto-generated. Do not modify manually.");
			sb.AppendLine("using System;");
			sb.AppendLine("using System.Collections.Generic;");
			sb.AppendLine("using UnityChess.DependencyInjection;");
			sb.AppendLine("using UnityEngine;");
			sb.AppendLine("using UnityEngine.UIElements;");
			sb.AppendLine();
			sb.AppendLine("namespace UnityChess.Presentation {");
			sb.AppendLine("\tpublic partial class Bootstrapper {");
			sb.AppendLine("\t\tprivate static readonly Dictionary<string, Action<Bootstrapper, ServiceRegistry, GameObject, UIDocument>> _installers = new() {");

			foreach (string compositionName in compositionNames) {
				sb.AppendLine($"\t\t\t[\"{compositionName}\"] = (self, registry, viewRoot, uiDocument) => self.Install{compositionName}(registry, viewRoot, uiDocument),");
			}

			sb.AppendLine("\t\t};");
			sb.AppendLine();
			sb.AppendLine("\t\tpartial void InstallComposition(string compositionName, ServiceRegistry registry, GameObject viewRoot, UIDocument uiDocument) {");
			sb.AppendLine("\t\t\tif (_installers.TryGetValue(compositionName, out var installer)) {");
			sb.AppendLine("\t\t\t\tinstaller(this, registry, viewRoot, uiDocument);");
			sb.AppendLine("\t\t\t} else {");
			sb.AppendLine("\t\t\t\tDebug.LogError($\"No installer found for composition '{compositionName}'. Available compositions: {string.Join(\", \", _installers.Keys)}\");");
			sb.AppendLine("\t\t\t}");
			sb.AppendLine("\t\t}");
			sb.AppendLine("\t}");
			sb.AppendLine("}");

			// Ensure directory exists
			string outputDir = "Assets/Scripts/Presentation/Generated";
			if (!Directory.Exists(outputDir)) {
				Directory.CreateDirectory(outputDir);
			}

			// Write to file
			string outputPath = $"{outputDir}/Bootstrapper.Registry.Generated.cs";
			File.WriteAllText(outputPath, sb.ToString());

			Debug.Log($"Generated composition registry at {outputPath}");
		}
	}
}
