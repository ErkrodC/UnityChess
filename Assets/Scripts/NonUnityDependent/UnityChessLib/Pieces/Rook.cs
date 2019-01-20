using System;
using System.Collections.Generic;

namespace UnityChess {
	public class Rook : Piece {
		private static int instanceCounter;

		public Rook(Square startingPosition, Side color) : base(startingPosition, color) {
			ID = ++instanceCounter;
		}

		private Rook(Rook rookCopy) : base(rookCopy) {
			ID = rookCopy.ID;
		}

		public override void UpdateValidMoves(Board board, LinkedList<Turn> previousMoves) {
			LegalMoves.Clear();

			CheckCardinalDirections(board);
		}

		private void CheckCardinalDirections(Board board) {
			foreach (int fileOffset in new[] {-1, 0, 1}) {
				foreach (int rankOffset in Math.Abs(fileOffset) == 1 ? new[] {0} : new[] {-1, 1}) {
					Square testSquare = new Square(Position, fileOffset, rankOffset);
					Movement testMove = new Movement(Position, testSquare);

					while (testSquare.IsValid()) {
						Square enemyKingPosition = Color == Side.White ? board.BlackKing.Position : board.WhiteKing.Position;
						if (testSquare.IsOccupied(board)) {
							if (!testSquare.IsOccupiedBySide(board, Color) && Rules.MoveObeysRules(board, testMove, Color) && testSquare != enemyKingPosition)
								LegalMoves.Add(new Movement(testMove));

							break;
						}

						if (Rules.MoveObeysRules(board, testMove, Color) && testSquare != enemyKingPosition)
							LegalMoves.Add(new Movement(testMove));

						testSquare = new Square(testSquare, fileOffset, rankOffset);
						testMove = new Movement(Position, testSquare);
					}
				}
			}
		}

		public override Piece Clone() => new Rook(this);
	}
}