using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityChess.DependencyInjection;
using UnityChess.DependencyInjection.Unity;
using UnityChess.Presentation.Util;
using UnityEditor;
using UnityEngine;

namespace UnityChess.Editor {
	[InitializeOnLoad]
	public static class BootstrapperGenerator {
		private const string OUTPUT_DIR = "Assets/Scripts/Presentation/Generated";
		private const string INSTALL_COMPOSITION_PATH = OUTPUT_DIR + "/Bootstrapper.InstallComposition.Generated.cs";
		private const string DELETED_FILES_KEY = "BootstrapperGenerator_DeletedFiles";
		private const string MISSING_CONCRETE_TYPES_KEY = "BootstrapperGenerator_MissingConcreteTypes";
		private const string BUILD_TYPE = "Unity";

		static BootstrapperGenerator() {
			LogDeletedOrphanedFiles();

			// Trigger generation after scripts compile
			EditorApplication.delayCall += Generate;
		}

		private static void LogDeletedOrphanedFiles() {
			// Check if there are deleted files to log after domain reload
			if (!EditorPrefs.HasKey(DELETED_FILES_KEY)) {
				return;
			}

			string deletedFilesStr = EditorPrefs.GetString(DELETED_FILES_KEY);
			EditorPrefs.DeleteKey(DELETED_FILES_KEY);

			if (!string.IsNullOrEmpty(deletedFilesStr)) {
				string[] deletedFiles = deletedFilesStr.Split('|');
				foreach (string file in deletedFiles) {
					Debug.LogWarning($"Deleted orphaned generated file: {file}");
				}
			}
		}

		private static void LogMissingConcreteTypes() {
			if (!EditorPrefs.HasKey(MISSING_CONCRETE_TYPES_KEY)) {
				return;
			}

			string interfaceTypesMissingConcrete = EditorPrefs.GetString(MISSING_CONCRETE_TYPES_KEY);
			EditorPrefs.DeleteKey(MISSING_CONCRETE_TYPES_KEY);

			if (!string.IsNullOrEmpty(interfaceTypesMissingConcrete)) {
				string[] interfaceTypeNames = interfaceTypesMissingConcrete.Split('|');
				foreach (string interfaceTypeName in interfaceTypeNames) {
					if (interfaceTypeName.Equals("IUpdateable", StringComparison.OrdinalIgnoreCase)) { continue; }

					Debug.LogError($"No matching concrete type '{GetConcreteNameForInterface(interfaceTypeName)}' found for interface '{interfaceTypeName}'");
				}
			}
		}

		[MenuItem("Tools/Bootstrapper Generation/Delete...", priority = 1)]
		public static void DeleteGeneratedFiles() {
			foreach (string filePath in Directory.GetFiles(OUTPUT_DIR, "Bootstrapper.*.Generated.cs*")) {
				File.Delete(filePath);
			}
		}

		[MenuItem("Tools/Bootstrapper Generation/Generate...", priority = 0)]
		public static void Generate() {
			if (File.Exists(INSTALL_COMPOSITION_PATH)) {
				File.Delete(INSTALL_COMPOSITION_PATH);
			}

			string[] guids = AssetDatabase.FindAssets("t:SceneComposition");
			List<SceneComposition> compositions = new();

			foreach (string guid in guids) {
				string path = AssetDatabase.GUIDToAssetPath(guid);
				SceneComposition composition = AssetDatabase.LoadAssetAtPath<SceneComposition>(path);
				if (composition != null) { compositions.Add(composition); }
			}

			// Clean up generated files that don't have corresponding composition assets
			List<string> compositionNames = compositions.Select(composition => composition.name).ToList();
			DeleteOrphanedGeneratedFiles(compositionNames);

			List<string> interfaceTypesMissingConcrete = new();
			foreach (SceneComposition composition in compositions) {
				GenerateInstallSceneMethod(composition, interfaceTypesMissingConcrete);
			}

			// Generate the installer file with method dictionary
			GenerateInstallCompositionMethod(compositionNames);

			// Generate the update function to drive updates in IUpdateable implementations
			GenerateUpdateMethod();


			if (interfaceTypesMissingConcrete.Count > 0) {
				string missingConcreteTypesStr = string.Join("|", interfaceTypesMissingConcrete);
				EditorPrefs.SetString(MISSING_CONCRETE_TYPES_KEY, missingConcreteTypesStr);
				EditorApplication.delayCall += LogMissingConcreteTypes;
			}

			AssetDatabase.Refresh();
		}

		private static void DeleteOrphanedGeneratedFiles(List<string> validCompositionNames) {
			if (!Directory.Exists(OUTPUT_DIR)) {
				return;
			}

			// Find all generated composition files
			string[] generatedFiles = Directory.GetFiles(OUTPUT_DIR, "Bootstrapper.Install*.Generated.cs");

			List<string> deletedFiles = new();
			foreach (string filePath in generatedFiles) {
				// Extract composition name from file name
				// Expected format: Bootstrapper.{CompositionName}.Generated.cs
				string fileName = Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(filePath)); // Remove .cs then .Generated
				string compositionName = fileName["Bootstrapper.Install".Length..];

				// Check if this composition still exists
				if (!validCompositionNames.Contains(compositionName)) {
					File.Delete(filePath);

					// Also delete the .meta file if it exists
					string metaFile = filePath + ".meta";
					if (File.Exists(metaFile)) {
						File.Delete(metaFile);
					}

					deletedFiles.Add(filePath.Replace('\\', '/'));
				}
			}

			// Store deleted files in EditorPrefs to survive domain reload
			if (deletedFiles.Count > 0) {
				EditorPrefs.SetString(DELETED_FILES_KEY, string.Join("|", deletedFiles));
			}
		}

		private static void GenerateInstallSceneMethod(SceneComposition composition, List<string> interfaceTypesMissingConcrete) {
			string compositionName = composition.name;

			List<Type> mediatorTypes = new();
			List<Type> viewTypes = new();
			HashSet<Type> managerTypes = new();
			HashSet<Type> viewModelTypes = new();
			HashSet<Type> concreteTypes = new();
			HashSet<Type> interfaceTypes = new();

			{ // Discover types
				// Resolve included mediator GUIDs to types
				foreach (string guid in composition.includedMediatorGUIDs) {
					if (!EditorReflectionUtil.TryGetTypeByMonoScriptGuid(guid, out Type mediatorType, out string path)) {
						Debug.LogError(
							$"For {nameof(SceneComposition)} asset \"{composition.name}\", failed to get type for {nameof(IMediator)} script GUID: {guid}, Path: {path}");
						continue;
					}

					if (!typeof(IMediator).IsAssignableFrom(mediatorType)) {
						Debug.LogError($"Script at {path} does not implement IMediator");
						continue;
					}

					mediatorTypes.Add(mediatorType);
					TrackResolvableInterfaces(mediatorType, interfaceTypes);

					// Inspect constructor to find managers and view models
					ConstructorInfo mediatorCtor = mediatorType.GetConstructors().FirstOrDefault();
					if (mediatorCtor != null) {
						foreach (ParameterInfo mediatorParam in mediatorCtor.GetParameters()) {
							if (typeof(IManager).IsAssignableFrom(mediatorParam.ParameterType)) {
								managerTypes.Add(mediatorParam.ParameterType);
								TrackResolvableInterfaces(mediatorParam.ParameterType, interfaceTypes);

								ConstructorInfo managerCtor = mediatorParam.ParameterType.GetConstructors().FirstOrDefault();
								if (managerCtor != null) {
									foreach (ParameterInfo managerParam in managerCtor.GetParameters()) {
										if (managerParam.ParameterType.IsInterface) {
											interfaceTypes.Add(managerParam.ParameterType);
										} else {
											concreteTypes.Add(managerParam.ParameterType);
											TrackResolvableInterfaces(managerParam.ParameterType, interfaceTypes);
										}
									}
								}
							} else if (typeof(IViewModel).IsAssignableFrom(mediatorParam.ParameterType)) {
								viewModelTypes.Add(mediatorParam.ParameterType);
							} else if (mediatorParam.ParameterType.IsInterface) {
								interfaceTypes.Add(mediatorParam.ParameterType);
							} else {
								concreteTypes.Add(mediatorParam.ParameterType);
							}
						}
					}
				}

				// Resolve included view GUIDs to types
				foreach (string guid in composition.includedViewGUIDs) {
					if (!EditorReflectionUtil.TryGetTypeByMonoScriptGuid(guid, out Type type, out string path)) {
						Debug.LogError(
							$"For ${nameof(SceneComposition)} asset \"{composition.name}\", failed to get type for {typeof(IView<>).Name} script GUID: {guid}, Path: {path}");
						continue;
					}

					Type viewInterface = type.GetInterfaces()
						.FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IView<>));

					if (viewInterface == null) {
						Debug.LogError($"Script at {path} does not implement IView<T>");
						continue;
					}

					viewTypes.Add(type);
					viewModelTypes.Add(viewInterface.GetGenericArguments().FirstOrDefault());
					TrackResolvableInterfaces(type, interfaceTypes);
				}

				foreach (Type interfaceType in interfaceTypes) {
					if (!interfaceType.IsInterface || !interfaceType.Name.StartsWith("I")) {
						continue;
					}

					string concreteName = GetConcreteNameForInterface(interfaceType.Name);
					Type concreteType = Assembly.Load("UnityChess.Presentation").GetTypes()
						.FirstOrDefault(t => t.Name == concreteName && !t.IsInterface && !t.IsAbstract);

					if (concreteType != null) {
						concreteTypes.Add(concreteType);
						TrackResolvableInterfaces(concreteType, interfaceTypes);
					} else {
						interfaceTypesMissingConcrete.Add(interfaceType.Name);
					}
				}
			}

			{ // Generate code
				StringBuilder sb = new();
				sb.AppendLine($"// This file is auto-generated by {nameof(BootstrapperGenerator)}. Do not modify manually.");
				sb.AppendLine("using System.Linq;");
				sb.AppendLine("using UnityChess.DependencyInjection;");
				sb.AppendLine("using UnityEngine;");
				sb.AppendLine("using UnityEngine.UIElements;");
				sb.AppendLine("using UnityChess.Presentation.Util;");

				// Add necessary using statements
				HashSet<string> namespaces = new();
				foreach (Type t in managerTypes.Concat(viewModelTypes).Concat(mediatorTypes).Concat(viewTypes).Concat(concreteTypes)) {
					if (!string.IsNullOrEmpty(t.Namespace)) {
						namespaces.Add(t.Namespace);
					}
				}

				foreach (string ns in namespaces.OrderBy(x => x)) {
					sb.AppendLine($"using {ns};");
				}

				sb.AppendLine("using static UnityChess.DependencyInjection.ServiceRegistry.Scope;");
				sb.AppendLine("using static UnityChess.DependencyInjection.ScopedRegistry.InstantiationTime;");

				sb.AppendLine();
				sb.AppendLine("namespace UnityChess.Presentation {");
				sb.AppendLine("\tpublic partial class Bootstrapper {");
				sb.AppendLine($"\t\tprivate void Install{compositionName}(ServiceRegistry registry) {{");

				// Register dependencies
				if (concreteTypes.Count > 0 || managerTypes.Count > 0) {
					sb.AppendLine("\t\t\t// Register Dependencies");
					HashSet<Type> visitedTypes = new();

					IEnumerable<Type> candidateTypes = concreteTypes.Concat(managerTypes);
					IDictionary<Type, Type> interfaceToConcrete = interfaceTypes
						.Where(interfaceType => candidateTypes.Any(interfaceType.IsAssignableFrom))
						.ToDictionary(interfaceType => candidateTypes.First(interfaceType.IsAssignableFrom)
					);
					Func<Type, IEnumerable<Type>> getInterfaces = t => GetImplementedResolvableInterfaces(t).Where(interfaceTypes.Contains);

					foreach (Type concreteType in concreteTypes.OrderBy(x => x.Name)) {
						EmitRecursiveRegistrations(concreteType, visitedTypes, sb, interfaceToConcrete, getInterfaces);
					}

					foreach (Type managerType in managerTypes.OrderBy(x => x.Name)) {
						EmitRecursiveRegistrations(managerType, visitedTypes, sb, interfaceToConcrete, getInterfaces);
					}

					sb.AppendLine();
				}

				// Begin scene registry scope
				sb.AppendLine("\t\t\t// Begin scene registry scope");
				sb.AppendLine("\t\t\tScopedRegistry sceneRegistry = registry.BeginScope(Scene);");

				// Register view models
				if (viewModelTypes.Count > 0) {
					sb.AppendLine();
					sb.AppendLine("\t\t\t// Register ViewModels");
					foreach (Type vmType in viewModelTypes.OrderBy(x => x.Name)) {
						sb.AppendLine($"\t\t\tsceneRegistry.Register(Lazy, () => new {vmType.Name}());");
					}
				}

				// Register mediators
				if (mediatorTypes.Count > 0) {
					sb.AppendLine();
					sb.AppendLine("\t\t\t// Register Mediators");
					foreach (Type mediatorType in mediatorTypes.OrderBy(x => x.Name)) {
						ConstructorInfo ctor = mediatorType.GetConstructors().FirstOrDefault();
						ParameterInfo[] parameters = ctor.GetParameters();

						if (parameters.Length == 0) {
							sb.AppendLine($"\t\t\tsceneRegistry.Register(Eager, () => new {mediatorType.Name}());");
						} else {
							string resolveParams = string.Join(", ",
								parameters.Select(p => $"registry.Resolve<{p.ParameterType.Name}>()"));
							sb.AppendLine(
								$"\t\t\tsceneRegistry.Register(Eager, () => new {mediatorType.Name}({resolveParams}));");
						}
					}
				}

				// Initialize views
				if (viewTypes.Count > 0) {
					sb.AppendLine();
					sb.AppendLine("\t\t\t// Initialize Views");
					foreach (Type viewType in viewTypes.OrderBy(x => x.Name)) {
						Type viewInterface = viewType.GetInterfaces()
							.First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IView<>));
						Type vmType = viewInterface.GetGenericArguments().FirstOrDefault();

						sb.AppendLine(
							$"\t\t\tgameObject.GetOrCreateComponent<{viewType.Name}>().Initialize(registry.Resolve<{vmType.Name}>());");
					}
				}

				// Initialize updateables
				sb.AppendLine();
				sb.AppendLine("\t\t\t// Initialize Updateables");
				sb.AppendLine("\t\t\t_updateables = _registry.ResolveMany<IUpdateable>().ToList();");
				sb.AppendLine("\t\t}");

				sb.AppendLine("\t}");
				sb.AppendLine("}");

				// Ensure directory exists
				if (!Directory.Exists(OUTPUT_DIR)) {
					Directory.CreateDirectory(OUTPUT_DIR);
				}

				// Write to file
				string outputPath = $"{OUTPUT_DIR}/Bootstrapper.Install{compositionName}.Generated.cs";
				File.WriteAllText(outputPath, sb.ToString());
			}
		}

		private static void GenerateInstallCompositionMethod(List<string> compositionNames) {
			StringBuilder sb = new();
			sb.AppendLine($"// This file is auto-generated by {nameof(BootstrapperGenerator)}. Do not modify manually.");
			sb.AppendLine("using System;");
			sb.AppendLine("using System.Collections.Generic;");
			sb.AppendLine("using UnityChess.DependencyInjection;");
			sb.AppendLine("using UnityEngine;");
			sb.AppendLine();
			sb.AppendLine("namespace UnityChess.Presentation {");
			sb.AppendLine("\tpublic partial class Bootstrapper {");
			sb.AppendLine("\t\tprivate static readonly Dictionary<string, Action<Bootstrapper, ServiceRegistry>> _installers = new() {");

			foreach (string compositionName in compositionNames.OrderBy(x => x)) {
				sb.AppendLine($"\t\t\t[\"{compositionName}\"] = (self, registry) => self.Install{compositionName}(registry),");
			}

			sb.AppendLine("\t\t};");
			sb.AppendLine();
			sb.AppendLine("\t\tpartial void InstallComposition(string compositionName, ServiceRegistry registry) {");
			sb.AppendLine("\t\t\tif (_installers.TryGetValue(compositionName, out var installer)) {");
			sb.AppendLine("\t\t\t\tinstaller(this, registry);");
			sb.AppendLine("\t\t\t} else {");
			sb.AppendLine("\t\t\t\tDebug.LogError($\"No installer found for composition '{compositionName}'. Available compositions: {string.Join(\", \", _installers.Keys)}\");");
			sb.AppendLine("\t\t\t}");
			sb.AppendLine("\t\t}");
			sb.AppendLine("\t}");
			sb.AppendLine("}");

			// Ensure directory exists
			if (!Directory.Exists(OUTPUT_DIR)) {
				Directory.CreateDirectory(OUTPUT_DIR);
			}

			// Write to file
			File.WriteAllText(INSTALL_COMPOSITION_PATH, sb.ToString());
		}

		private static void GenerateUpdateMethod() {
			StringBuilder sb = new();
			sb.AppendLine($"// This file is auto-generated by {nameof(BootstrapperGenerator)}. Do not modify manually.");
			sb.AppendLine("using System.Collections.Generic;");
			sb.AppendLine("using UnityChess.Application;");
			sb.AppendLine("using UnityEngine;");
			sb.AppendLine();
			sb.AppendLine("namespace UnityChess.Presentation {");
			sb.AppendLine("\tpublic partial class Bootstrapper {");
			sb.AppendLine("\t\tprivate IReadOnlyList<IUpdateable> _updateables;");
			sb.AppendLine();
			sb.AppendLine("\t\tprivate void Update() {");
			sb.AppendLine("\t\t\tif (_updateables == null) { return; }");
			sb.AppendLine();
			sb.AppendLine("\t\t\tfloat dt = Time.deltaTime;");
			sb.AppendLine("\t\t\tforeach (IUpdateable updateable in _updateables) {");
			sb.AppendLine("\t\t\t\tupdateable.Update(dt);");
			sb.AppendLine("\t\t\t}");
			sb.AppendLine("\t\t}");
			sb.AppendLine("\t}");
			sb.AppendLine("}");

			// Ensure directory exists
			if (!Directory.Exists(OUTPUT_DIR)) {
				Directory.CreateDirectory(OUTPUT_DIR);
			}

			// Write to file
			string outputPath = $"{OUTPUT_DIR}/Bootstrapper.Update.Generated.cs";
			File.WriteAllText(outputPath, sb.ToString());
		}

		private static string GetConcreteNameForInterface(string interfaceTypeName) {
			return BUILD_TYPE + interfaceTypeName[1..];
		}

		private static void EmitRecursiveRegistrations(
			Type requestedType,
			HashSet<Type> visitedTypes,
			StringBuilder sb,
			IDictionary<Type, Type> interfaceToConcrete,
			Func<Type, IEnumerable<Type>> getResolvableInterfaces
		) {
			if (requestedType == null) { return; }

			Type concreteType = requestedType.IsInterface && interfaceToConcrete.TryGetValue(requestedType, out Type resolvedConcrete)
				? resolvedConcrete
				: requestedType;

			if (!visitedTypes.Add(concreteType)) { return; }

			ConstructorInfo ctor = concreteType.GetConstructors().FirstOrDefault()
				?? throw new InvalidOperationException($"Type '{concreteType.Name}' does not expose a public constructor.");

			List<string> parameterExpressions = new();
			foreach (ParameterInfo param in ctor.GetParameters()) {
				Type dependencyType = param.ParameterType;

				if (dependencyType.IsInterface) {
					if (!interfaceToConcrete.TryGetValue(dependencyType, out Type mappedConcrete)) {
						throw new InvalidOperationException($"No concrete type registered for interface {dependencyType.Name}, required by {concreteType.Name}.");
					}

					dependencyType = mappedConcrete;
				}

				EmitRecursiveRegistrations(dependencyType, visitedTypes, sb, interfaceToConcrete, getResolvableInterfaces);
				parameterExpressions.Add($"registry.Resolve<{dependencyType.Name}>()");
			}

			IReadOnlyList<Type> exposedInterfaces = getResolvableInterfaces(concreteType)?.ToArray() ?? Array.Empty<Type>();
			string interfaceArgs = exposedInterfaces.Count == 0
				? string.Empty
				: ", " + string.Join(", ", exposedInterfaces.Select(t => $"typeof({t.Name})"));

			string ctorArgs = parameterExpressions.Count == 0
				? string.Empty
				: string.Join(", ", parameterExpressions);

			sb.AppendLine($"\t\t\tregistry.RegisterSingleton<{concreteType.Name}>(() => new {concreteType.Name}({ctorArgs}){interfaceArgs});");
		}

		private static void TrackResolvableInterfaces(Type concreteType, HashSet<Type> interfaceTypes) {
			foreach (Type implementationType in GetImplementedResolvableInterfaces(concreteType)) {
				interfaceTypes.Add(implementationType);
			}
		}

		private static IEnumerable<Type> GetImplementedResolvableInterfaces(Type concreteType) {
			return concreteType.GetInterfaces().Where(interfaceType => interfaceType.Namespace != null && interfaceType.Namespace.StartsWith("UnityChess.Application", StringComparison.Ordinal));
		}
	}
}
