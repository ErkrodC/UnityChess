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

		public override void UpdateValidMoves(Board board, LinkedList<Turn> previousMoves) {
			ValidMoves.Clear();
			CheckKnightCircleSquares(board);
		}

		private void CheckKnightCircleSquares(Board board) {
			for (int fileOffset = -2; fileOffset <= 2; fileOffset++) {
				if (fileOffset == 0) continue;

				foreach (int rankOffset in Math.Abs(fileOffset) == 2 ? new[] {-1, 1} : new[] {-2, 2}) {
					Square testSquare = new Square(Position, fileOffset, rankOffset);
					Movement testMove = new Movement(Position, testSquare);

					Square enemyKingPosition = PieceOwner == Side.White ? board.BlackKing.Position : board.WhiteKing.Position;
					if (testSquare.IsValid() && !testSquare.IsOccupiedBySide(board, PieceOwner) && Rules.MoveObeysRules(board, testMove, PieceOwner) && testSquare != enemyKingPosition)
						ValidMoves.Add(new Movement(testMove));
				}
			}
		}

		public override Piece Clone() => new Knight(this);
	}
}