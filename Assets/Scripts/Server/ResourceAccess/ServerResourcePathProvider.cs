using System.IO;
using UnityChess.Core.Resource;

namespace Server.ResourceAccess {
	public class ServerResourcePathProvider : IResourcePathProvider {
		public string streamingAssetsPath => Directory.GetCurrentDirectory();
	}
}