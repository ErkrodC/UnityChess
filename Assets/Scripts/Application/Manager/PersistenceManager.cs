using UnityChess.DependencyInjection;
using UnityChess.Resource;
using UnityChess.ResourceAccess;

namespace UnityChess.Application {
	public class PersistenceManager : IManager {
		private readonly IKeyStorage _keyStorage;

		public PersistenceManager(JsonKeyStorage keyStorage) {
			_keyStorage = keyStorage;
		}

		public bool TryGet<T>(string key, out T value)	=> _keyStorage.TryGet(key, out value);
		public void Set<T>(string key, T value)			=> _keyStorage.Set(key, value);
		public void Delete(string key)					=> _keyStorage.Delete(key);
		public void Save()								=> _keyStorage.Save();
	}
}