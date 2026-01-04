namespace UnityChess.Resource {
	public interface IResourcePathProvider {
		string streamingAssetsPath { get; }
		string persistentDataPath { get; }
	}
}