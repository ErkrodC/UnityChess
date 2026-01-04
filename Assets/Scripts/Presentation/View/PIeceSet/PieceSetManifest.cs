using UnityChess.Presentation.View;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UnityChess.Presentation {
	[CreateAssetMenu(menuName = "UnityChess/Piece Set Manifest")]
	public class PieceSetManifest : ScriptableObject {
		public class Entry {
			public string displayName;
			public string addressablesKey;
			public AssetReferenceT<PieceSetDefinition> asset;
		}

		[field: SerializeField] public Entry[] entries { get; private set; }
	}
}