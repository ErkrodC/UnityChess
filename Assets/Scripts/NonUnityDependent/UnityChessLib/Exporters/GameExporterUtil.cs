using System.Collections.Generic;

namespace UnityChess {
	public static class GameExporterUtil {
		public static GameConditions GetEndingGameConditions(GameConditions startingConditions, Board currentBoard, List<HalfMove> halfMoves) {
			if (halfMoves.Count == 0) return startingConditions;
			
			bool whiteEligibleForCastling = currentBoard.WhiteKing.Position.Equals(5, 1) && !currentBoard.WhiteKing.HasMoved;
			bool whiteCanCastleKingside = whiteEligibleForCastling && currentBoard[8, 1] is Rook whiteKingsideRook && !whiteKingsideRook.HasMoved;
			bool whiteCanCastleQueenside = whiteEligibleForCastling && currentBoard[1, 1] is Rook whiteQueensideRook && !whiteQueensideRook.HasMoved;
			
			bool blackEligibleForCastling = currentBoard.BlackKing.Position.Equals(5, 8) && !currentBoard.BlackKing.HasMoved;
			bool blackCanCastleKingside = blackEligibleForCastling && currentBoard[8, 8] is Rook blackKingsideRook && !blackKingsideRook.HasMoved;
			bool blackCanCastleQueenside = blackEligibleForCastling && currentBoard[1, 8] is Rook blackQueensideRook && !blackQueensideRook.HasMoved;

			int halfMoveClock = startingConditions.HalfMoveClock;

			foreach (HalfMove halfMove in halfMoves) {
				if (!(halfMove.Piece is Pawn) && !halfMove.CapturedPiece) halfMoveClock++;
				else halfMoveClock = 0;
			}
			
			
			bool whiteToMove = halfMoves.Count % 2 == 0 ? startingConditions.WhiteToMove : !startingConditions.WhiteToMove;

			int turnNumber = startingConditions.TurnNumber;
			if (startingConditions.WhiteToMove) {
				turnNumber += halfMoves.Count / 2;
			} else {
				turnNumber += (halfMoves.Count + 1) / 2;
			}

			Square enPassantSquare = new Square(-1, -1);

			HalfMove lastHalfMove = halfMoves[halfMoves.Count - 1];
			Side lastTurnPieceColor = lastHalfMove.Piece.Color;
			int pawnStartingRank = lastTurnPieceColor == Side.White ? 2 : 7;
			int pawnEndingRank = lastTurnPieceColor == Side.White ? 4 : 5;

			if (lastHalfMove.Piece is Pawn && lastHalfMove.Move.Start.Rank == pawnStartingRank && lastHalfMove.Move.End.Rank == pawnEndingRank) {
				int rankOffset = lastTurnPieceColor == Side.White ? -1 : 1;
				enPassantSquare = new Square(lastHalfMove.Move.End, 0, rankOffset);
			}
			
			return new GameConditions(whiteToMove, whiteCanCastleKingside, whiteCanCastleQueenside, blackCanCastleKingside, blackCanCastleQueenside, enPassantSquare, halfMoveClock, turnNumber);
		}
	}
}