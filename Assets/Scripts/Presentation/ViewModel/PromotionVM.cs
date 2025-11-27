using System;
using UnityChess.Core;
using UnityChess.Util;
using UnityEngine;

namespace UnityChess.Presentation.ViewModel {
	[CreateAssetMenu(fileName = "PromotionVM", menuName = "ScriptableObjects/PromotionVM", order = 1)]
	public class PromotionVM : ScriptableObject {
		[SerializeField] public bool isRequesting;
		[SerializeField] public Side requestingSide;

		public Action<ElectedPiece> OnPieceElected;
		public Action OnCancelled;
	}
}