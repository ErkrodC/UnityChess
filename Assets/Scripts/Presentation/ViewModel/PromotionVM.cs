using System;
using UnityChess.Core;
using UnityChess.DependencyInjection;
using UnityChess.Util;

namespace UnityChess.Presentation.ViewModel {
	public class PromotionVM : IViewModel {
		public bool isRequesting;
		public Side requestingSide;

		public Action<ElectedPiece> OnPieceElected;
		public Action OnCancelled;
	}
}