using System;
using System.Collections.Generic;

namespace UnityChess.DependencyInjection {
	public class ServiceRegistry {
		private readonly Dictionary<Type, object> _singletons = new();
		private readonly Dictionary<Type, Func<object>> _factories = new();

		public void RegisterSingleton<T>(Func<T> factory) where T : class {
			Type type = typeof(T);
			_factories[type] = factory;
		}

		public T Resolve<T>() where T : class {
			Type type = typeof(T);

			if (_singletons.TryGetValue(type, out object existing)) {
				return (T)existing;
			}

			if (_factories.TryGetValue(type, out Func<object> factory)) {
				T instance = (T)factory();
				_singletons[type] = instance;
				return instance;
			}

			throw new InvalidOperationException($"No registration found for type {type.Name}");
		}

		public void Clear() {
			_singletons.Clear();
			_factories.Clear();
		}
	}
}
