using System;
using System.Collections.Generic;

namespace UnityChess {
	/// <summary>Contains methods for checking legality of moves and board positions.</summary>
	public static class NewRules {
		/// <summary>Checks if the player of the given side has been checkmated.</summary>
		public static bool IsPlayerCheckmated(ChessGameState gameState, Side player) => PlayerHasNoLegalMoves(gameState, player) && IsPlayerInCheck(gameState, player);

		/// <summary>Checks if the player of the given side has been stalemated.</summary>
		public static bool IsPlayerStalemated(ChessGameState gameState, Side player) => PlayerHasNoLegalMoves(gameState, player) && !IsPlayerInCheck(gameState, player);

		/// <summary>Checks if the player of the given side is in check.</summary>
		public static bool IsPlayerInCheck(ChessGameState gameState, Side player) => IsKingInCheck(gameState, player == Side.White ? board.WhiteKing : board.BlackKing);

		internal static bool MoveObeysRules(ChessGameState gameState, Movement move, Side movedPieceSide) => !MovePutsMoverInCheck(gameState, move, movedPieceSide) && MoveRemovesCheckFromPlayerIfNeeded(gameState, move, movedPieceSide);

		private static bool MoveRemovesCheckFromPlayerIfNeeded(ChessGameState gameState, Movement move, Side side) {
			if (!IsPlayerInCheck(gameState, side)) return true;

			ChessGameState resultState = new ChessGameState(gameState, move, out bool moveWasLegal);

			return !IsPlayerInCheck(resultState, side) && moveWasLegal;
		}

		private static bool MovePutsMoverInCheck(ChessGameState gameState, Movement move, Side moverSide) {
			ChessGameState resultState = new ChessGameState(gameState, move, out bool moveWasLegal);

			return IsPlayerInCheck(resultState, moverSide) && moveWasLegal;
		}

		private static bool PlayerHasNoLegalMoves(ChessGameState gameState, Side player) {
			int sumOfLegalMoves = 0;

			for (int file = 1; file <= 8; file++) {
				for (int rank = 1; rank <= 8; rank++) {
					Piece piece = board[file, rank];
					if (piece != null && piece.Color == player) sumOfLegalMoves += piece.LegalMoves.Count;;
				}
			}

			return sumOfLegalMoves == 0;
		}

		private static bool IsKingInCheck(ChessGameState gameState, King king) => IsCheckedRoseDirections(gameState, king) || IsCheckedKnightDirections(gameState, king);

		private static bool IsCheckedRoseDirections(ChessGameState gameState, King king) {
			List<Square> surroundingSquares = new List<Square>();
			List<Square> pawnAttackingSquares = new List<Square>();

			GenerateSquareLists(surroundingSquares, pawnAttackingSquares, king);

			foreach (int fileOffset in new[] {-1, 0, 1})
				foreach (int rankOffset in new[] {-1, 0, 1}) {
					if (fileOffset == 0 && rankOffset == 0) continue;

					Square testSquare = new Square(king.Position, fileOffset, rankOffset);

					while (testSquare.IsValid && !board.IsOccupiedBySide(testSquare, king.Color)) {
						if (board.IsOccupiedBySide(testSquare, king.Color.Complement())) {
							Piece piece = board[testSquare];

							//diagonal direction
							if (Math.Abs(fileOffset) == Math.Abs(rankOffset)) {
								if (piece is Bishop || piece is Queen || testSquare.Rank == king.Position.Rank + (king.Color == Side.White ? 1 : -1)
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

		private static bool IsCheckedKnightDirections(ChessGameState gameState, King king) {
			for (int fileOffset = -2; fileOffset <= 2; fileOffset++) {
				if (fileOffset == 0) continue;

				int[] knightRankOffset = Math.Abs(fileOffset) == 2 ? new[] {-1, 1} : new[] {-2, 2};
				foreach (int rankOffset in knightRankOffset) {
					Square testSquare = new Square(king.Position, fileOffset, rankOffset);

					if (testSquare.IsValid && board.IsOccupiedBySide(testSquare, king.Color.Complement()) && board[testSquare] is Knight)
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

					if (testSquare.IsValid) {
						if ((fileOffset == 1 || fileOffset == -1) && rankOffset == (king.Color == Side.White ? 1 : -1))
							pawnAttackingSquares.Add(testSquare);
						surroundingSquares.Add(testSquare);
					}
				}
			}
		}
	}
}