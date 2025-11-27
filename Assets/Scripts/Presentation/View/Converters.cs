using UnityChess.Core;
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

		public static string PieceVMToTextArt(ref PieceVM piece) {
			return PieceUtil.GetPieceTextArt(piece.type, piece.side);
		}

		public static string ElectedPieceToTextArt(ElectedPiece electedPiece, Side side) {
			PieceType pieceType = electedPiece switch {
				ElectedPiece.Knight => PieceType.Knight,
				ElectedPiece.Bishop => PieceType.Bishop,
				ElectedPiece.Rook => PieceType.Rook,
				ElectedPiece.Queen => PieceType.Queen,
				_ => PieceType.None
			};
			return PieceUtil.GetPieceTextArt(pieceType, side);
		}

		public static Visibility BoolToVisibility(ref bool isVisible) {
			return isVisible ? Visibility.Visible : Visibility.Hidden;
		}
	}
}