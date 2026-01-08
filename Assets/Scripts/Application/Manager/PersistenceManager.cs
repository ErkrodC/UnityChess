using UnityChess.DependencyInjection;
using UnityChess.Resource;
using UnityChess.ResourceAccess;

namespace UnityChess.Application {
	public class PersistenceManager : IManager {
		private readonly IStorage _storage;

		public PersistenceManager(JsonStorage storage) {
			_storage = storage;
		}

		public bool TryLoad<T>(out T value, string fileName = null)	=> _storage.TryLoad(out value, fileName);
		public void Save<T>(T value, string fileName = null)		=> _storage.Save(value, fileName);
		public void Delete<T>()										=> _storage.Delete<T>();
		public void Delete(string fileName)							=> _storage.Delete(fileName);
	}
}