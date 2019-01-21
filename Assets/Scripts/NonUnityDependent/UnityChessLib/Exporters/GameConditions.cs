namespace UnityChess {
	public struct GameConditions {
		public static GameConditions NormalStartingConditions = new GameConditions(true, true, true, true, true, new Square(-1, -1), 0, 1);
		public readonly bool WhiteToMove;
		public readonly bool WhiteCanCastleKingside;
		public readonly bool WhiteCanCastleQueenside;
		public readonly bool BlackCanCastleKingside;
		public readonly bool BlackCanCastleQueenside;
		public readonly Square EnPassantSquare;
		public readonly int HalfMoveClock;
		public readonly int TurnNumber;
	
		public GameConditions(bool whiteToMove, bool whiteCanCastleKingside, bool whiteCanCastleQueenside, bool blackCanCastleKingside, bool blackCanCastleQueenside, Square enPassantSquare, int halfMoveClock, int turnNumber) {
			WhiteToMove = whiteToMove;
			WhiteCanCastleKingside = whiteCanCastleKingside;
			WhiteCanCastleQueenside = whiteCanCastleQueenside;
			BlackCanCastleKingside = blackCanCastleKingside;
			BlackCanCastleQueenside = blackCanCastleQueenside;
			EnPassantSquare = enPassantSquare;
			HalfMoveClock = halfMoveClock;
			TurnNumber = turnNumber;
		}
	}
}