using System.Collections.Generic;
using System.Threading.Tasks;
using UnityChess.DependencyInjection;
using UnityChess.Resource;

namespace UnityChess.Application {
	public class AssetManager : IManager {
		private readonly IAssetLoader _assetLoader;

		public AssetManager(IAssetLoader assetLoader) {
			_assetLoader = assetLoader;
		}

		public Task<IAssetRef<TAsset>> LoadAsync<TAsset>(string assetKey)
			=> _assetLoader.LoadAsync<TAsset>(assetKey);

		public Task<IReadOnlyList<IAssetRef<TAsset>>> LoadAllAsync<TAsset>(string tag)
			=> _assetLoader.LoadAllAsync<TAsset>(tag);
	}
}