using System;
using UnityChess.Core;
using UnityEngine;

namespace UnityChess.Presentation.ViewModel {
	[Serializable]
	public class MoveViewModel {
		[field: SerializeField] public int halfMoveNumber { get; set; }
		[field: SerializeField] public Movement move { get; set; }

		public MoveViewModel(int halfMoveNumber, Movement move) {
			this.halfMoveNumber = halfMoveNumber;
			this.move = move;
		}
	}
}