using System;
using UnityChess.Core.Util;
using UnityEngine;

namespace UnityChess.Presentation.View {
	[CreateAssetMenu(fileName = "PieceSetDefinition", menuName = "UnityChess/Piece Set")]
	public class PieceSetDefinition : ScriptableObject {
		[Serializable]
		private struct Entry {
			public PieceType pieceType;
			public Side side;
			public Sprite sprite;
		}

		[field: SerializeField] public string key { get; }
		[SerializeField] private Entry[] _entries;

		public Sprite GetSprite(PieceType pieceType, Side side) {
			foreach (Entry entry in _entries) {
				if (entry.pieceType == pieceType && entry.side == side) {
					return entry.sprite;
				}
			}

			return null;
		}
	}
}