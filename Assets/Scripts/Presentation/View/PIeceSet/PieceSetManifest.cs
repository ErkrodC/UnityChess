using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityChess.Presentation {
	[CreateAssetMenu(menuName = "UnityChess/Piece Set Manifest")]
	public class PieceSetManifest : ScriptableObject {
		[Serializable]
		public class Entry {
			public string key;
			public string displayName;
			public Sprite previewIcon;
		}

		[field: SerializeField, HideInInspector] public List<Entry> entries { get; private set; }
	}
}