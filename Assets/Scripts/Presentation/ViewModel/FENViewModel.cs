using System;
using UnityEngine;

namespace UnityChess.Presentation.ViewModel {
	[Serializable]
	public class FENViewModel {
		[field: SerializeField] public string fen { get; set; }
	}
}