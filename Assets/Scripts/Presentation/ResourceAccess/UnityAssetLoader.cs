using System.Threading.Tasks;
using UnityChess.Resource;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace UnityChess.Presentation.ResourceAccess {
	public class UnityAssetLoader : IAssetLoader {
		public Task<TAsset> LoadAsync<TAsset>(string assetKey) {
			AsyncOperationHandle<TAsset> handle = Addressables.LoadAssetAsync<TAsset>(assetKey);
			return handle.Task;
		}

		public void Release(object handle) {
			Addressables.Release(handle);
		}
	}
}