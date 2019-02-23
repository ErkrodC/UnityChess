using System.Collections.Generic;

namespace UnityChess {
	/// Non-board, non-move-record game state
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
		
		public GameConditions CalculateEndingGameConditions(Board endingBoard, List<HalfMove> halfMovesFromStart) {
			if (halfMovesFromStart.Count == 0) return this;
			
			bool whiteEligibleForCastling = endingBoard.WhiteKing.Position.Equals(5, 1) && !endingBoard.WhiteKing.HasMoved;
			bool blackEligibleForCastling = endingBoard.BlackKing.Position.Equals(5, 8) && !endingBoard.BlackKing.HasMoved;

			return new GameConditions(
				whiteToMove: halfMovesFromStart.Count % 2 == 0 ? WhiteToMove : !WhiteToMove,
				whiteCanCastleKingside: whiteEligibleForCastling && endingBoard[8, 1] is Rook whiteKingsideRook && !whiteKingsideRook.HasMoved,
				whiteCanCastleQueenside: whiteEligibleForCastling && endingBoard[1, 1] is Rook whiteQueensideRook && !whiteQueensideRook.HasMoved,
				blackCanCastleKingside: blackEligibleForCastling && endingBoard[8, 8] is Rook blackKingsideRook && !blackKingsideRook.HasMoved,
				blackCanCastleQueenside: blackEligibleForCastling && endingBoard[1, 8] is Rook blackQueensideRook && !blackQueensideRook.HasMoved,
				enPassantSquare: GetEndingEnPassantSquare(halfMovesFromStart[halfMovesFromStart.Count - 1]),
				halfMoveClock: GetEndingHalfMoveClock(halfMovesFromStart),
				turnNumber: TurnNumber + (WhiteToMove ? halfMovesFromStart.Count / 2 : (halfMovesFromStart.Count + 1) / 2)
			);
		}

		private int GetEndingHalfMoveClock(List<HalfMove> movesFromStart) {
			int endingHalfMoveClock = HalfMoveClock;
			
			foreach (HalfMove halfMove in movesFromStart) {
				if (!(halfMove.Piece is Pawn) && !halfMove.CapturedPiece) endingHalfMoveClock++;
				else endingHalfMoveClock = 0;
			}

			return endingHalfMoveClock;
		}

		// NOTE ending en passant square can be determined from simply the last half move made.
		private static Square GetEndingEnPassantSquare(HalfMove lastHalfMove) {
			Side lastTurnPieceColor = lastHalfMove.Piece.Color;
			int pawnStartingRank = lastTurnPieceColor == Side.White ? 2 : 7;
			int pawnEndingRank = lastTurnPieceColor == Side.White ? 4 : 5;

			Square enPassantSquare = Square.Invalid;
			if (lastHalfMove.Piece is Pawn && lastHalfMove.Move.Start.Rank == pawnStartingRank && lastHalfMove.Move.End.Rank == pawnEndingRank) {
				int rankOffset = lastTurnPieceColor == Side.White ? -1 : 1;
				enPassantSquare = new Square(lastHalfMove.Move.End, 0, rankOffset);
			}

			return enPassantSquare;
		}
	}
}