using System;
using System.Collections.Generic;

namespace UnityChess.DependencyInjection {
	public class ServiceRegistry {
		public enum Scope { Scene }

		private sealed class SingletonEntry {
			public readonly Func<object> _factory;
			public object _instance;

			public SingletonEntry(Func<object> factory) {
				_factory = factory ?? throw new ArgumentNullException(nameof(factory));
			}

			public object GetOrCreateInstance() {
				return _instance ??= _factory();
			}
		}

		private readonly Dictionary<Type, SingletonEntry> _entries = new();
		private readonly Dictionary<Scope, ScopedRegistry> _scopedRegistries;
		private readonly Dictionary<Type, List<SingletonEntry>> _entryListsByInterface = new();

		public ServiceRegistry() {
			_scopedRegistries = new Dictionary<Scope, ScopedRegistry> {
				[Scope.Scene] = null
			};
		}

		public void RegisterSingleton<T>(Func<T> factory, params Type[] interfacesResolvableBy) where T : class {
			Type concreteType = typeof(T);
			if (concreteType.IsInterface) { throw new ArgumentException("Cannot register a singleton as an interface type. Register as the concrete type instead."); }

			SingletonEntry entry = _entries[concreteType] = new(factory);

			foreach (Type interfaceType in interfacesResolvableBy) {
				if (!interfaceType.IsInterface) {
					throw new ArgumentException($"Type \"{interfaceType.Name}\" is not an interface, it cannot be used as a type for a singleton to be resolved by.");
				}

				if (!_entryListsByInterface.TryGetValue(interfaceType, out List<SingletonEntry> implementations)) {
					implementations = _entryListsByInterface[interfaceType] = new List<SingletonEntry>();
				}

				implementations.Add(entry);
			}
		}

		public T Resolve<T>() where T : class {
			if (typeof(T).IsInterface) { throw new ArgumentException($"Cannot resolve an interface type."); }

			// ER NOTE: here is essentially what determines order of resolution
			if (_scopedRegistries.TryGetValue(Scope.Scene, out ScopedRegistry sceneRegistry)
			    && (sceneRegistry?.TryResolve(out T instance) ?? false)
			) {
				return instance;
			}

			if (_entries.TryGetValue(typeof(T), out SingletonEntry entry)) {
				return (T)entry.GetOrCreateInstance();
			}

			throw new InvalidOperationException($"No registration found for type {typeof(T).Name}");
		}

		public IEnumerable<TInterface> ResolveMany<TInterface>() {
			if (_entryListsByInterface.TryGetValue(typeof(TInterface), out List<SingletonEntry> implementationEntries)) {
				foreach (SingletonEntry implementationEntry in implementationEntries) {
					yield return (TInterface)implementationEntry.GetOrCreateInstance();
				}
			}
		}

		public ScopedRegistry BeginScope(Scope scope) {
			if (_scopedRegistries[scope] != null) {
				// ER TODO: in the future, BeginScope could push new registries onto a stack, EndScope could pop them off
				// to allow for nested scopes. Resolve would check the topmost scope first and traverse down the stack
				throw new InvalidOperationException($"A scope of type {scope} is already active." +
				                                    $" Call {nameof(EndScope)} before beginning a new scope of the same type.");
			}

			_scopedRegistries[scope] = new ScopedRegistry();
			return _scopedRegistries[scope];
		}

		public void EndScope(Scope scope) {
			_scopedRegistries[scope]?.DisposeInternal();
			_scopedRegistries[scope] = null;
		}
	}
}
