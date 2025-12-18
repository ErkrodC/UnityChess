using UnityChess.Util;

namespace UnityChess.Core {
	/// Non-board, non-move-record game state
	public struct GameConditions {
		public static GameConditions NormalStartingConditions = new GameConditions(
			sideToMove: Side.White,
			whiteCanCastleKingside: true,
			whiteCanCastleQueenside: true,
			blackCanCastleKingside: true,
			blackCanCastleQueenside: true,
			enPassantSquare: Square.Invalid,
			halfMoveClock: 0,
			turnNumber: 1
		);

		public readonly Side SideToMove;
		public readonly bool WhiteCanCastleKingside;
		public readonly bool WhiteCanCastleQueenside;
		public readonly bool BlackCanCastleKingside;
		public readonly bool BlackCanCastleQueenside;
		public readonly Square EnPassantSquare;
		public readonly int HalfMoveClock;
		public readonly int TurnNumber;

		public GameConditions(
			Side sideToMove,
			bool whiteCanCastleKingside,
			bool whiteCanCastleQueenside,
			bool blackCanCastleKingside,
			bool blackCanCastleQueenside,
			Square enPassantSquare,
			int halfMoveClock,
			int turnNumber
		) {
			SideToMove = sideToMove;
			WhiteCanCastleKingside = whiteCanCastleKingside;
			WhiteCanCastleQueenside = whiteCanCastleQueenside;
			BlackCanCastleKingside = blackCanCastleKingside;
			BlackCanCastleQueenside = blackCanCastleQueenside;
			EnPassantSquare = enPassantSquare;
			HalfMoveClock = halfMoveClock;
			TurnNumber = turnNumber;
		}

		public GameConditions CalculateEndingConditions(Board resultingBoard, HalfMove lastHalfMove) {
			bool whiteKingMoved = lastHalfMove.Piece is King { Owner: Side.White };
			bool whiteQueensideRookMoved = lastHalfMove is {
				Piece: Rook { Owner: Side.White },
				Move: { Start: { File: 1, Rank: 1 } }
			};
			bool whiteKingsideRookMoved = lastHalfMove is {
				Piece: Rook { Owner: Side.White },
				Move: { Start: { File: 8, Rank: 1 } }
			};

			bool blackKingMoved = lastHalfMove.Piece is King { Owner: Side.Black };
			bool blackQueensideRookMoved = lastHalfMove is {
				Piece: Rook { Owner: Side.Black },
				Move: { Start: { File: 1, Rank: 8 } }
			};
			bool blackKingsideRookMoved = lastHalfMove is {
				Piece: Rook { Owner: Side.Black },
				Move: { Start: { File: 8, Rank: 8 } }
			};

			return new GameConditions(
				sideToMove: SideToMove.Complement(),
				whiteCanCastleKingside: WhiteCanCastleKingside && !whiteKingMoved && !whiteKingsideRookMoved,
				whiteCanCastleQueenside: WhiteCanCastleKingside && !whiteKingMoved && !whiteQueensideRookMoved,
				blackCanCastleKingside: BlackCanCastleKingside && !blackKingMoved && !blackKingsideRookMoved,
				blackCanCastleQueenside: BlackCanCastleKingside && !blackKingMoved && !blackQueensideRookMoved,
				enPassantSquare: GetNextEnPassantSquare(lastHalfMove),
				halfMoveClock: GetNextHalfMoveClock(lastHalfMove, HalfMoveClock),
				turnNumber: TurnNumber + (SideToMove == Side.White ? 0 : 1)
			);
		}

		private static int GetNextHalfMoveClock(HalfMove lastHalfMove, int endingHalfMoveClock) {
			return lastHalfMove.Piece is Pawn || lastHalfMove.CapturedPiece
				? 0
				: endingHalfMoveClock + 1;
		}

		private static Square GetNextEnPassantSquare(HalfMove lastHalfMove) {
			Side lastTurnSide = lastHalfMove.Piece.Owner;
			Side currentSide = lastTurnSide.Complement();
			int vantageRank = currentSide.EnPassantVantageRank();

			Square enPassantSquare = Square.Invalid;
			if (lastHalfMove.Piece is Pawn
			    && lastHalfMove.Move.Start.Rank == lastTurnSide.PawnRank()
			    && lastHalfMove.Move.End.Rank == vantageRank
			) {
				enPassantSquare = new Square(lastHalfMove.Move.End, 0, currentSide.ForwardDirection());
			}

			return enPassantSquare;
		}
	}
}