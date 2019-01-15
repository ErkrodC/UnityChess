using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityChess {
	/// <summary>Contains methods for checking legality of moves and board positions.</summary>
	public static class Rules {
		/// <summary>Checks if the player of the given side has been checkmated.</summary>
		public static bool IsPlayerCheckmated(Board board, Side side) {
			return IsTotalValidMovesZero(board, side) && IsPlayerInCheck(board, side);
		}

		/// <summary>Checks if the player of the given side has been stalemated.</summary>
		public static bool IsPlayerStalemated(Board board, Side side) {
			return IsTotalValidMovesZero(board, side) && !IsPlayerInCheck(board, side);
		}

		/// <summary>Checks if the player of the given side is in check.</summary>
		public static bool IsPlayerInCheck(Board board, Side side) => IsKingInCheck(board, side == Side.White ? board.WhiteKing : board.BlackKing);

		internal static bool MoveObeysRules(Board board, Movement move, Side turn) => !DoesMoveCauseCheck(board, move, move.Piece.Side) && DoesMoveRemoveCheck(board, move, move.Piece.Side);

		private static bool DoesMoveRemoveCheck(Board board, Movement move, Side side) {
			if (!IsPlayerInCheck(board, side)) return true;

			Board resultingBoard = new Board(board);
			Piece analogPiece = resultingBoard.BasePieceList.Single(bp => bp is Piece piece && piece.Position.Equals(move.Piece.Position)) as Piece;
			resultingBoard.MovePiece(new Movement(move.End, analogPiece));

			return !IsPlayerInCheck(resultingBoard, side);
		}

		private static bool DoesMoveCauseCheck(Board board, Movement move, Side side) {
			Board resultingBoard = new Board(board);
			Piece analogPiece = resultingBoard.BasePieceList.Single(bp => bp is Piece && (bp as Piece).Position.Equals(move.Piece.Position)) as Piece;
			resultingBoard.MovePiece(new Movement(move.End, analogPiece));

			return IsPlayerInCheck(resultingBoard, side);
		}

		private static bool IsTotalValidMovesZero(Board board, Side side) {
			int sumOfValidMoves = 0;

			foreach (Piece p in board.BasePieceList.OfType<Piece>().ToList().FindAll(p => p.Side == side))
				sumOfValidMoves += p.ValidMoves.Count;

			return sumOfValidMoves == 0;
		}

		private static bool IsKingInCheck(Board board, King king) => IsCheckedRoseDirections(board, king) || IsCheckedKnightDirections(board, king);

		private static bool IsCheckedRoseDirections(Board board, King king) {
			List<Square> surroundingSquares = new List<Square>();
			List<Square> pawnAttackingSquares = new List<Square>();

			GenerateSquareLists(surroundingSquares, pawnAttackingSquares, king);

			foreach (int fileOffset in new[] {-1, 0, 1})
				foreach (int rankOffset in new[] {-1, 0, 1}) {
					if (fileOffset == 0 && rankOffset == 0) continue;

					Square testSquare = new Square(king.Position, fileOffset, rankOffset);

					while (testSquare.IsValid() && !testSquare.IsOccupiedBySide(board, king.Side)) {
						if (testSquare.IsOccupiedBySide(board, king.Side.Complement())) {
							Piece piece = board.GetPiece(testSquare);

							//diagonal direction
							if (Math.Abs(fileOffset) == Math.Abs(rankOffset)) {
								if (piece is Bishop || piece is Queen || testSquare.Rank == king.Position.Rank + (king.Side == Side.White ? 1 : -1)
								    && (testSquare.File == king.Position.File + 1 || testSquare.File == king.Position.File - 1)
								    && piece is Pawn) {
									return true;
								}

								switch (piece) {
									case King _ when surroundingSquares.Contains(piece.Position):
									case Pawn _ when pawnAttackingSquares.Contains(piece.Position):
										return true;
								}
							}
							//cardinal directions
							else {
								switch (piece) {
									case Rook _:
									case Queen _:
									case King _ when surroundingSquares.Contains(piece.Position):
										return true;
								}
							}

							break;
						}

						testSquare = new Square(testSquare, fileOffset, rankOffset);
					}
				}

			return false;
		}

		private static bool IsCheckedKnightDirections(Board board, King king) {
			for (int fileOffset = -2; fileOffset <= 2; fileOffset++) {
				if (fileOffset == 0) continue;

				int[] knightRankOffset = Math.Abs(fileOffset) == 2 ? new[] {-1, 1} : new[] {-2, 2};
				foreach (int rankOffset in knightRankOffset) {
					Square testSquare = new Square(king.Position, fileOffset, rankOffset);

					if (testSquare.IsValid() && testSquare.IsOccupiedBySide(board, king.Side.Complement()) && board.GetPiece(testSquare) is Knight)
						return true;
				}
			}

			return false;
		}

		private static void GenerateSquareLists(List<Square> surroundingSquares, List<Square> pawnAttackingSquares, King king) {
			foreach (int fileOffset in new[] {-1, 0, 1}) {
				foreach (int rankOffset in new[] {-1, 0, 1}) {
					if (fileOffset == 0 && rankOffset == 0) continue;

					Square testSquare = new Square(king.Position, fileOffset, rankOffset);

					if (testSquare.IsValid()) {
						if ((fileOffset == 1 || fileOffset == -1) && rankOffset == (king.Side == Side.White ? 1 : -1))
							pawnAttackingSquares.Add(new Square(testSquare));
						surroundingSquares.Add(new Square(testSquare));
					}
				}
			}
		}
	}
}