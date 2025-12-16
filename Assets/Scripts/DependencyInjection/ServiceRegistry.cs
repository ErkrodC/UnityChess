using System;
using System.Collections.Generic;

namespace UnityChess.DependencyInjection {
	public class ServiceRegistry {
		public enum Scope { Scene }

		private readonly Dictionary<Type, object> _singletons = new();
		private readonly Dictionary<Scope, ScopedRegistry> _scopedRegistries;

		public ServiceRegistry() {
			_scopedRegistries = new Dictionary<Scope, ScopedRegistry> {
				[Scope.Scene] = null
			};
		}

		public void RegisterSingleton<T>(T instance) where T : class {
			_singletons[typeof(T)] = instance;
		}

		public T Resolve<T>() where T : class {
			// ER NOTE: here is essentially what determines order of resolution
			if (_scopedRegistries.TryGetValue(Scope.Scene, out ScopedRegistry sceneRegistry) && sceneRegistry.TryResolve(out T instance)) {
				return instance;
			}

			if (_singletons.TryGetValue(typeof(T), out object existing)) {
				return (T)existing;
			}

			throw new InvalidOperationException($"No registration found for type {typeof(T).Name}");
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
