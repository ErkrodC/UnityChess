using System;

namespace UnityChess.Resource {
	public interface IAssetRef<out TAsset> : IDisposable {
		TAsset asset { get; }
	}
}