using System.Threading.Tasks;
using UnityChess.DependencyInjection;
using UnityChess.Resource;

namespace UnityChess.Application {
	public class DLCManager : IManager {
		private readonly IAssetLoader _assetLoader;

		public DLCManager(IAssetLoader assetLoader) {
			_assetLoader = assetLoader;
		}

		public Task<TAsset> LoadAsync<TAsset>(string assetKey) => _assetLoader.LoadAsync<TAsset>(assetKey);
		public void Release(object handle) => _assetLoader.Release(handle);
	}
}