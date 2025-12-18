using UnityChess.Util;

namespace UnityChess.Core {
	public static class PromotionUtil {
		public static Piece GeneratePromotionPiece(ElectedPiece election, Side side) => election switch {
			ElectedPiece.Bishop => new Bishop(side),
			ElectedPiece.Knight => new Knight(side),
			ElectedPiece.Queen => new Queen(side),
			ElectedPiece.Rook => new Rook(side),
			_ => null
		};
	}
}
