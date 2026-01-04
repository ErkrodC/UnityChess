using System.Threading.Tasks;

namespace UnityChess.Resource {
	public interface IAssetLoader {
		Task<TAsset> LoadAsync<TAsset>(string assetKey);
		void Release(object handle);
	}
}