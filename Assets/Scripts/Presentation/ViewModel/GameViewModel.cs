using System;
using UnityEngine;

namespace UnityChess.Presentation.ViewModel {
	[Serializable]
	public class GameViewModel {
		[field: SerializeField] public BoardViewModel boardVM { get; set; }
		[field: SerializeField] public TurnViewModel turnVM { get; set; }
		[field: SerializeField] public MoveHistoryViewModel moveHistoryVM { get; set; }
		[field: SerializeField] public FENViewModel fenVM { get; set; }
	}
}