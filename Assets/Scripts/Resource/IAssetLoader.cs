using System.Collections.Generic;
using System.Threading.Tasks;

namespace UnityChess.Resource {
	public interface IAssetLoader {
		Task<IAssetRef<TAsset>> LoadAsync<TAsset>(string key);
		Task<IReadOnlyList<IAssetRef<TAsset>>> LoadAllAsync<TAsset>(string tag);
	}
}