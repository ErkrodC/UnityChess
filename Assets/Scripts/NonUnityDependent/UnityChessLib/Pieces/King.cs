using System.Collections.Generic;

namespace UnityChess {
	public class King : Piece {
		private static int instanceCounter;

		public King(Square startingPosition, Side color) : base(startingPosition, color) {
			ID = ++instanceCounter;
		}

		private King(King kingCopy) : base(kingCopy) {
			ID = kingCopy.ID;
		}

		public override void UpdateValidMoves(Board board, LinkedList<Turn> previousMoves) {
			LegalMoves.Clear();

			CheckSurroundingSquares(board);
			CheckCastlingMoves(board);
		}

		public override Piece Clone() => new King(this);

		private void CheckSurroundingSquares(Board board) {
			for (int fileOffset = -1; fileOffset <= 1; fileOffset++) {
				for (int rankOffset = -1; rankOffset <= 1; rankOffset++) {
					if (fileOffset == 0 && rankOffset == 0) continue;

					Square testSquare = new Square(Position, fileOffset, rankOffset);
					Movement testMove = new Movement(Position, testSquare);
					Square enemyKingPosition = Color == Side.White ? board.BlackKing.Position : board.WhiteKing.Position;
					if (testSquare.IsValid() && !testSquare.IsOccupiedBySide(board, Color) && Rules.MoveObeysRules(board, testMove, Color) && testSquare != enemyKingPosition)
						LegalMoves.Add(new Movement(testMove));
				}
			}
		}

		private void CheckCastlingMoves(Board board) {
			if (!HasMoved && !Rules.IsPlayerInCheck(board, Color)) {
				int castlingRank = Color == Side.White ? 1 : 8;
				List<Square> rookSquares = new List<Square> { new Square(1, castlingRank), new Square(8, castlingRank) };
				List<Square> inBetweenSquares = new List<Square>();
				List<Movement> inBetweenMoves = new List<Movement>();

				for (int rookSquareIndex = 0; rookSquareIndex < rookSquares.Count; rookSquareIndex++) {
					if (board[rookSquares[rookSquareIndex]] is Rook rook && !rook.HasMoved && rook.Color == Color) {
						bool checkingQueensideCastle = rookSquareIndex == 0;
						inBetweenSquares.Add(new Square(checkingQueensideCastle ? 4 : 6, castlingRank));
						inBetweenSquares.Add(new Square(checkingQueensideCastle ? 3 : 7, castlingRank));
						if (checkingQueensideCastle) inBetweenSquares.Add(new Square(2, castlingRank));

						if (!inBetweenSquares[0].IsOccupied(board) && !inBetweenSquares[1].IsOccupied(board) && (!checkingQueensideCastle || !inBetweenSquares[2].IsOccupied(board))) {
							inBetweenMoves.Add(new Movement(Position, inBetweenSquares[0]));
							inBetweenMoves.Add(new Movement(Position, inBetweenSquares[1]));

							if (Rules.MoveObeysRules(board, inBetweenMoves[0], Color) && Rules.MoveObeysRules(board, inBetweenMoves[1], Color)) {
								LegalMoves.Add(new CastlingMove(Position, inBetweenSquares[1], rook));
							}
						}

						inBetweenSquares.Clear();
						inBetweenMoves.Clear();
					}
				}
			}
		}
	}
}