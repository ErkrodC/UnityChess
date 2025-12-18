using UnityChess.Core.Resource;

namespace UnityChess.Presentation.ResourceAccess {
	public class UnityResourcePathProvider : IResourcePathProvider {
		public string streamingAssetsPath => UnityEngine.Application.streamingAssetsPath;
	}
}