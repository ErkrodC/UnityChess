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

			for (int i = -2; i <= 2; i++) {
				if (i == 0) continue;

				foreach (int j in Math.Abs(i) == 2 ? new[] {-1, 1} : new[] {-2, 2}) {
					testSquare.CopyPosition(Position);
					testSquare.AddVector(i, j);

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