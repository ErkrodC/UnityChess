using System.Collections.Generic;

namespace UnityChess {
	public static class PieceUtil {
	
		public delegate Piece ElectedPieceGenerator(Square endSquare, Side side);
		public static readonly Dictionary<ElectedPiece, ElectedPieceGenerator> PromotionMoveGeneratorMap = new Dictionary<ElectedPiece, ElectedPieceGenerator>{
			{ElectedPiece.Bishop, (endSquare, side) => new Bishop(endSquare, side)},
			{ElectedPiece.Bishop, (endSquare, side) => new Bishop(endSquare, side)},
			{ElectedPiece.Bishop, (endSquare, side) => new Bishop(endSquare, side)},
			{ElectedPiece.Bishop, (endSquare, side) => new Bishop(endSquare, side)}
		};
	}
}
