using System.IO;
using UnityChess.Resource;

namespace Server.ResourceAccess {
	public class ServerResourcePathProvider : IResourcePathProvider {
		public string streamingAssetsPath => Directory.GetCurrentDirectory();
		public string persistentDataPath => Directory.GetCurrentDirectory();
	}
}