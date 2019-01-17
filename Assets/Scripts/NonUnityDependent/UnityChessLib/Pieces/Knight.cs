using System;
using System.Collections.Generic;

namespace UnityChess {
	public class Knight : Piece {
		private static int instanceCounter;

		public Knight(Square startingPosition, Side side) : base(startingPosition, side) {
			ID = ++instanceCounter;
		}

		private Knight(Knight knightCopy) : base(knightCopy) {
			ID = knightCopy.ID;
		}

		public override void UpdateValidMoves(Board board, LinkedList<Movement> previousMoves, Side turn) {
			ValidMoves.Clear();

			CheckKnightCircleSquares(board, turn);
		}

		private void CheckKnightCircleSquares(Board board, Side turn) {
			Square testSquare = new Square(Position);
			Movement testMove = new Movement(testSquare, this);

			for (int fileOffset = -2; fileOffset <= 2; fileOffset++) {
				if (fileOffset == 0) continue;

				foreach (int rankOffset in Math.Abs(fileOffset) == 2 ? new[] {-1, 1} : new[] {-2, 2}) {
					testSquare = new Square(Position, fileOffset, rankOffset);

					if (testSquare.IsValid() && !testSquare.IsOccupiedBySide(board, Side) && Rules.MoveObeysRules(board, testMove, turn) && !testSquare.Equals(Side == Side.White ? board.BlackKing.Position : board.WhiteKing.Position)) {
						ValidMoves.Add(new Movement(testMove));
					}
				}
			}
		}

		public override Piece Clone() {
			return new Knight(this);
		}
	}
}