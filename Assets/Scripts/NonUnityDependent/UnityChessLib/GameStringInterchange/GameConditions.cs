using System.Collections.Generic;

namespace UnityChess {
	public struct GameConditions {
		public static GameConditions NormalStartingConditions = new GameConditions(true, true, true, true, true, Square.Invalid, 0, 1);
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
		
		public GameConditions GetEndingGameConditions(Board currentBoard, List<HalfMove> movesFromStart) {
			if (movesFromStart.Count == 0) return this;
			
			bool whiteEligibleForCastling = currentBoard.WhiteKing.Position.Equals(5, 1) && !currentBoard.WhiteKing.HasMoved;
			bool whiteCanCastleKingside = whiteEligibleForCastling && currentBoard[8, 1] is Rook whiteKingsideRook && !whiteKingsideRook.HasMoved;
			bool whiteCanCastleQueenside = whiteEligibleForCastling && currentBoard[1, 1] is Rook whiteQueensideRook && !whiteQueensideRook.HasMoved;
			
			bool blackEligibleForCastling = currentBoard.BlackKing.Position.Equals(5, 8) && !currentBoard.BlackKing.HasMoved;
			bool blackCanCastleKingside = blackEligibleForCastling && currentBoard[8, 8] is Rook blackKingsideRook && !blackKingsideRook.HasMoved;
			bool blackCanCastleQueenside = blackEligibleForCastling && currentBoard[1, 8] is Rook blackQueensideRook && !blackQueensideRook.HasMoved;

			int halfMoveClock = this.HalfMoveClock;

			foreach (HalfMove halfMove in movesFromStart) {
				if (!(halfMove.Piece is Pawn) && !halfMove.CapturedPiece) halfMoveClock++;
				else halfMoveClock = 0;
			}
			
			
			bool whiteToMove = movesFromStart.Count % 2 == 0 ? this.WhiteToMove : !this.WhiteToMove;

			int turnNumber = this.TurnNumber;
			if (this.WhiteToMove) {
				turnNumber += movesFromStart.Count / 2;
			} else {
				turnNumber += (movesFromStart.Count + 1) / 2;
			}

			HalfMove lastHalfMove = movesFromStart[movesFromStart.Count - 1];
			Side lastTurnPieceColor = lastHalfMove.Piece.Color;
			int pawnStartingRank = lastTurnPieceColor == Side.White ? 2 : 7;
			int pawnEndingRank = lastTurnPieceColor == Side.White ? 4 : 5;

			Square enPassantSquare = Square.Invalid;
			if (lastHalfMove.Piece is Pawn && lastHalfMove.Move.Start.Rank == pawnStartingRank && lastHalfMove.Move.End.Rank == pawnEndingRank) {
				int rankOffset = lastTurnPieceColor == Side.White ? -1 : 1;
				enPassantSquare = new Square(lastHalfMove.Move.End, 0, rankOffset);
			}
			
			return new GameConditions(whiteToMove, whiteCanCastleKingside, whiteCanCastleQueenside, blackCanCastleKingside, blackCanCastleQueenside, enPassantSquare, halfMoveClock, turnNumber);
		}
	}
}