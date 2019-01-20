using System;
using System.Collections.Generic;

namespace UnityChess {
	public class Knight : Piece {
		private static int instanceCounter;

		public Knight(Square startingPosition, Side color) : base(startingPosition, color) {
			ID = ++instanceCounter;
		}

		private Knight(Knight knightCopy) : base(knightCopy) {
			ID = knightCopy.ID;
		}

		public override void UpdateValidMoves(Board board, LinkedList<Turn> previousMoves) {
			LegalMoves.Clear();
			CheckKnightCircleSquares(board);
		}

		private void CheckKnightCircleSquares(Board board) {
			for (int fileOffset = -2; fileOffset <= 2; fileOffset++) {
				if (fileOffset == 0) continue;

				foreach (int rankOffset in Math.Abs(fileOffset) == 2 ? new[] {-1, 1} : new[] {-2, 2}) {
					Square testSquare = new Square(Position, fileOffset, rankOffset);
					Movement testMove = new Movement(Position, testSquare);

					Square enemyKingPosition = Color == Side.White ? board.BlackKing.Position : board.WhiteKing.Position;
					if (testSquare.IsValid() && !testSquare.IsOccupiedBySide(board, Color) && Rules.MoveObeysRules(board, testMove, Color) && testSquare != enemyKingPosition)
						LegalMoves.Add(new Movement(testMove));
				}
			}
		}

		public override Piece Clone() => new Knight(this);
	}
}