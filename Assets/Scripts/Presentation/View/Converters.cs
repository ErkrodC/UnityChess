using UnityChess.Presentation.ViewModel;
using UnityChess.Util;
using UnityEngine.UIElements;

namespace UnityChess.Presentation {
	public static class Converters {
		public static string BoolToTurnText(ref Side currentSideToMove) { return currentSideToMove.ToString(); }

		public static DisplayStyle WhiteActiveToDisplayStyle(ref Side currentSideToMove) {
			return currentSideToMove == Side.White ? DisplayStyle.Flex : DisplayStyle.None;
		}

		public static DisplayStyle BlackActiveToDisplayStyle(ref Side currentSideToMove) {
			return currentSideToMove == Side.Black ? DisplayStyle.Flex : DisplayStyle.None;
		}

		public static string PieceToTextArt(ref PieceVM piece) {
			return PieceUtil.GetPieceTextArt(piece.type, piece.side);
		}
	}
}