using System;
using System.Collections.Generic;

namespace UnityChess.DependencyInjection {
	public class ScopedRegistry : IDisposable {
		public enum InstantiationTime { Lazy, Eager }
		private readonly Dictionary<Type, object> _instances = new();
		private readonly Dictionary<Type, Func<object>> _factories = new();
		private readonly List<IDisposable> _disposables = new();

		public void Register<T>(InstantiationTime time, Func<T> factory) where T : class {
			_factories[typeof(T)] = factory;

			if (time == InstantiationTime.Eager) {
				object instance = _instances[typeof(T)] = factory();
				if (instance is IDisposable disposable) { _disposables.Add(disposable); }
			}
		}

		internal bool TryResolve<T>(out T instance) where T : class {
			instance = null;
			Type type = typeof(T);

			if (_instances.TryGetValue(type, out object existing)) {
				instance = (T)existing;
				return true;
			}

			if (_factories.TryGetValue(type, out Func<object> factory)) {
				instance = (T)factory();
				_instances[type] = instance;

				if (instance is IDisposable disposable) { _disposables.Add(disposable); }

				return true;
			}

			return false;
		}

		void IDisposable.Dispose() => DisposeInternal();
		internal void DisposeInternal() {
			for (int i = _disposables.Count - 1; i >= 0; i--) {
				_disposables[i].Dispose();
			}

			_instances.Clear();
			_disposables.Clear();
		}
	}
}