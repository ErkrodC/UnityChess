using System;
using UnityChess.Core;
using UnityEngine;

namespace UnityChess.Presentation.ViewModel {
	[Serializable]
	public class TurnViewModel {
		[field: SerializeField] public Side currentSideToMove { get; set; }
	}
}