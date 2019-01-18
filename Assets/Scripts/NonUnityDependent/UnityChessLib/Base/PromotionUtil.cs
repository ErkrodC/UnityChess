using System.Collections.Generic;

namespace UnityChess {
	public static class PromotionUtil {
		private delegate Piece ElectedPieceGenerator(Square endSquare, Side side);
		private static readonly Dictionary<ElectedPiece, ElectedPieceGenerator> ElectedPieceGeneratorMap = new Dictionary<ElectedPiece, ElectedPieceGenerator>{
			{ElectedPiece.Bishop, (endSquare, side) => new Bishop(endSquare, side)},
			{ElectedPiece.Knight, (endSquare, side) => new Knight(endSquare, side)},
			{ElectedPiece.Queen, (endSquare, side) => new Queen(endSquare, side)},
			{ElectedPiece.Rook, (endSquare, side) => new Rook(endSquare, side)},
			{ElectedPiece.None, (endSquare, side) => null}
		};
		
		public static Piece GeneratePromotionPiece(ElectedPiece election, Square position, Side side) {
			Piece piece = ElectedPieceGeneratorMap[election](position, side);
			
			if (piece == null) return null;
			
			piece.HasMoved = true;
			piece.Position = position;
			return piece;
		}
	}
}
