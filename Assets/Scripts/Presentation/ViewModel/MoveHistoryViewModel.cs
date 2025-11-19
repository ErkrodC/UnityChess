using System;
using UnityChess.Core;
using UnityEngine;

namespace UnityChess.Presentation.ViewModel {
	[Serializable]
	public class MoveHistoryViewModel {
		[field: SerializeField] public Timeline<HalfMove> halfMoves { get; set; } = new Timeline<HalfMove>();
	}
}