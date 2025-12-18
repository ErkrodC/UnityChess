using System.Collections.Generic;
using UnityChess.Util;

namespace UnityChess.Core {
	public class King : Piece<King> {
		private static readonly int[] rookFiles = { 0, 7 };

		public King() : base(Side.None) {}
		public King(Side owner) : base(owner) {}

		public override Dictionary<(Square, Square), Movement> CalculateLegalMoves(
			Board board,
			GameConditions gameConditions,
			Square position
		) {
			Dictionary<(Square, Square), Movement> result = null;

			CheckSurroundingSquares(board, position, ref result);

			if (Owner == Side.White) {
				CheckCastlingMoves(
					board,
					position,
					gameConditions.WhiteCanCastleKingside,
					gameConditions.WhiteCanCastleQueenside,
					ref result
				);
			} else {
				CheckCastlingMoves(
					board,
					position,
					gameConditions.BlackCanCastleKingside,
					gameConditions.BlackCanCastleQueenside,
					ref result
				);
			}

			return result;
		}

		public override string ToTextArt() => Owner switch {
			Side.White => "♚",
			Side.Black => "♔",
			_ => "."
		};

		private void CheckSurroundingSquares(
			Board board,
			Square position,
			ref Dictionary<(Square, Square), Movement> movesByStartEndSquare
		) {
			foreach (Square offset in SquareUtil.SurroundingOffsets) {
				Movement testMove = new Movement(position, position + offset);

				if (Rules.MoveObeysRules(board, testMove, Owner)) {
					if (movesByStartEndSquare == null) {
						movesByStartEndSquare = new Dictionary<(Square, Square), Movement>();
					}

					movesByStartEndSquare[(testMove.Start, testMove.End)] = new Movement(testMove);
				}
			}
		}

		private void CheckCastlingMoves(
			Board board,
			Square position,
			bool canCastleKingSide,
			bool canCastleQueenside,
			ref Dictionary<(Square, Square), Movement> movesByStartEndSquare
		) {
			if (Rules.IsPlayerInCheck(board, Owner)
				|| !canCastleKingSide && !canCastleQueenside
			) { return; }

			int castlingRank = Owner.CastlingRank();

			foreach (int rookFile in rookFiles) {
				bool checkingQueenside = rookFile == 1;

				Square rookSquare = new Square(rookFile, castlingRank);
				if (board[rookSquare] is not Rook rook
				    || rook.Owner != Owner
				    || checkingQueenside && !canCastleQueenside
				    || !checkingQueenside && !canCastleKingSide
				) {
					continue;
				}

				Square inBetweenSquare0 = new Square(checkingQueenside ? 3 : 5, castlingRank);
				Square inBetweenSquare1 = new Square(checkingQueenside ? 2 : 6, castlingRank);
				Square inBetweenSquare2 = new Square(1, castlingRank);
				Movement castlingMove = new CastlingMove(position, inBetweenSquare1, rookSquare);

				if (!board.IsOccupiedAt(inBetweenSquare0)
				    && !board.IsOccupiedAt(inBetweenSquare1)
				    && (!board.IsOccupiedAt(inBetweenSquare2) || !checkingQueenside)
				    && !Rules.IsSquareAttacked(inBetweenSquare0, board, Owner)
				    && !Rules.IsSquareAttacked(inBetweenSquare1, board, Owner)
				    && Rules.MoveObeysRules(board, castlingMove, Owner)
				) {
					if (movesByStartEndSquare == null) {
						movesByStartEndSquare = new Dictionary<(Square, Square), Movement>();
					}

					movesByStartEndSquare[(position, inBetweenSquare1)]
						= new CastlingMove(position, inBetweenSquare1, rookSquare);
				}
			}
		}
	}
}