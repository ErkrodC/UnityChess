using System;
using UnityChess.Util;

namespace UnityChess.Core {
	/// <summary>Contains methods for checking legality of moves and board positions.</summary>
	public static class Rules {
		/// <summary>Checks if the player of the given side has been checkmated.</summary>
		public static bool IsPlayerCheckmated(Board board, Side player, int numLegalMoves) =>
			numLegalMoves <= 0 && IsPlayerInCheck(board, player);

		/// <summary>Checks if the player of the given side has been stalemated.</summary>
		public static bool IsPlayerStalemated(Board board, Side player, int numLegalMoves) =>
			numLegalMoves <= 0 && !IsPlayerInCheck(board, player);

		/// <summary>Checks if the player of the given side is in check.</summary>
		public static bool IsPlayerInCheck(Board board, Side player) =>
			IsSquareAttacked(board.GetKingSquare(player), board, player);

		internal static bool MoveObeysRules(Board board, Movement move, Side movedPieceSide) {
			if (!move.Start.IsValid()
			    || !move.End.IsValid()
				|| board[move.End] is King
			    || board.IsOccupiedBySideAt(move.End, movedPieceSide)
			) { return false; }

			Board resultingBoard = new Board(board);
			resultingBoard.MovePiece(new Movement(move.Start, move.End));

			return !IsPlayerInCheck(resultingBoard, movedPieceSide);
		}

		public static bool IsSquareAttacked(Square squareInQuestion, Board board, Side friendlySide) {
			Side enemySide = friendlySide.Complement();
			int friendlyForward = friendlySide.ForwardDirection();

			foreach (Square offset in SquareUtil.SurroundingOffsets) {
				bool isDiagonalOffset = Math.Abs(offset.File) == Math.Abs(offset.Rank);
				Square endSquare = squareInQuestion + offset;

				while (endSquare.IsValid() && !board.IsOccupiedBySideAt(endSquare, friendlySide)) {
					if (board.IsOccupiedBySideAt(endSquare, enemySide)) {
						int fileDistance = Math.Abs(endSquare.File - squareInQuestion.File);
						int rankDistance = Math.Abs(endSquare.Rank - squareInQuestion.Rank);

						Piece enemyPiece = board[endSquare];
						switch (enemyPiece) {
							case Queen:
							case Bishop when isDiagonalOffset:
							case Rook when !isDiagonalOffset:
							case King when fileDistance <= 1 && rankDistance <= 1:
							case Pawn when fileDistance == 1
							               && endSquare.Rank == squareInQuestion.Rank + friendlyForward:
								return true;
						}

						// stop checking this diagonal in the case of enemy knight
						break;
					}

					endSquare += offset;
				}
			}

			foreach (Square offset in SquareUtil.KnightOffsets) {
				Square endSquare = squareInQuestion + offset;

				if (endSquare.IsValid()
				    && board[endSquare] is Knight knight
				    && knight.Owner == enemySide
				) {
					return true;
				}
			}

			return false;
		}
	}
}