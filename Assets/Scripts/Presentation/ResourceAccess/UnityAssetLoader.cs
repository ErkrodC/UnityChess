using System.Collections.Generic;
using System.Threading.Tasks;
using UnityChess.Resource;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace UnityChess.Presentation.ResourceAccess {
	public class UnityAssetLoader : IAssetLoader {
		public async Task<IAssetRef<TAsset>> LoadAsync<TAsset>(string key) {
			AsyncOperationHandle<TAsset> handle = Addressables.LoadAssetAsync<TAsset>(key);
			await handle.Task;
			return new AddressablesAssetRef<TAsset>(handle);
		}

		public async Task<IReadOnlyList<IAssetRef<TAsset>>> LoadAllAsync<TAsset>(string tag) {
			IList<IResourceLocation> locations = await Addressables.LoadResourceLocationsAsync(tag).Task;

			List<IAssetRef<TAsset>> refs = new(locations.Count);
			foreach (IResourceLocation location in locations) {
				refs.Add(new AddressablesAssetRef<TAsset>(Addressables.LoadAssetAsync<TAsset>(location)));
			}

			Addressables.Release(Addressables.LoadResourceLocationsAsync(tag));
			return refs;
		}

		private sealed class AddressablesAssetRef<TAsset> : IAssetRef<TAsset> {
			public TAsset asset => _handle.Result;
			private AsyncOperationHandle<TAsset> _handle;

			public AddressablesAssetRef(AsyncOperationHandle<TAsset> handle) {
				_handle = handle;
			}

			public void Dispose() {
				if (_handle.IsValid()) {
					Addressables.Release(_handle);
					_handle = default;
				}
			}
		}
	}
}