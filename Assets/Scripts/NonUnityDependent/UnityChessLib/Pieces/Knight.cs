using System;
using System.Collections.Generic;

namespace UnityChess {
	public class Knight : Piece {
		private static int instanceCounter;

		public Knight(Square startingPosition, Side pieceOwner) : base(startingPosition, pieceOwner) {
			ID = ++instanceCounter;
		}

		private Knight(Knight knightCopy) : base(knightCopy) {
			ID = knightCopy.ID;
		}

		public override void UpdateValidMoves(Board board, LinkedList<Turn> previousMoves, Side turn) {
			ValidMoves.Clear();

			CheckKnightCircleSquares(board, turn);
		}

		private void CheckKnightCircleSquares(Board board, Side turn) {
			Square testSquare = new Square(Position);
			Movement testMove = new Movement(this, testSquare);

			for (int fileOffset = -2; fileOffset <= 2; fileOffset++) {
				if (fileOffset == 0) continue;

				foreach (int rankOffset in Math.Abs(fileOffset) == 2 ? new[] {-1, 1} : new[] {-2, 2}) {
					testSquare = new Square(Position, fileOffset, rankOffset);

					Square enemyKingPosition = PieceOwner == Side.White ? board.BlackKing.Position : board.WhiteKing.Position;
					if (testSquare.IsValid() && !testSquare.IsOccupiedBySide(board, PieceOwner) && Rules.MoveObeysRules(board, testMove, PieceOwner) && testSquare != enemyKingPosition) {
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