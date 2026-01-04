using UnityChess.Resource;

namespace UnityChess.Presentation.ResourceAccess {
	public class UnityResourcePathProvider : IResourcePathProvider {
		public string streamingAssetsPath => UnityEngine.Application.streamingAssetsPath;
		public string persistentDataPath => UnityEngine.Application.persistentDataPath;
	}
}