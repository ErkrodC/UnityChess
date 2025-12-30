using System;
using UnityChess.Core;
using UnityChess.Core.Util;
using UnityChess.DependencyInjection;

namespace UnityChess.Presentation.ViewModel {
	public class PromotionVM : IViewModel {
		public bool isRequesting;
		public Side requestingSide;

		public Action<ElectedPiece> onPieceElected;
		public Action onCancelled;
	}
}