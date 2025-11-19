using UnityChess.Core;
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

		public static string PieceToTextArt(ref Piece piece) {
			return piece?.ToTextArt() ?? string.Empty;
		}
	}
}