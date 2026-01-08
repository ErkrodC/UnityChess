namespace UnityChess.Resource {
	public interface IStorage {
		bool TryLoad<T>(out T value, string fileName = null);
		void Save<T>(T value, string fileName = null);
		void Delete<T>();
		void Delete(string fileName);
	}
}