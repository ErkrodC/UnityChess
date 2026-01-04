namespace UnityChess.Resource {
	public interface IKeyStorage {
		bool TryGet<T>(string key, out T value);
		void Set<T>(string key, T value);
		void Delete(string key);
		void Save();
	}
}