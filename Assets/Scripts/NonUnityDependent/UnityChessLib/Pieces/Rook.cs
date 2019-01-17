using System;
using System.Collections.Generic;

namespace UnityChess {
	public class Rook : Piece {
		private static int instanceCounter;

		public Rook(Square startingPosition, Side pieceOwner) : base(startingPosition, pieceOwner) {
			ID = ++instanceCounter;
		}

		private Rook(Rook rookCopy) : base(rookCopy) {
			ID = rookCopy.ID;
		}

		public override void UpdateValidMoves(Board board, LinkedList<Turn> previousMoves, Side turn) {
			ValidMoves.Clear();

			CheckCardinalDirections(board, turn);
		}

		private void CheckCardinalDirections(Board board, Side turn) {
			Square testSquare = new Square(Position);
			Movement testMove = new Movement(this, testSquare);

			foreach (int fileOffset in new[] {-1, 0, 1}) {
				foreach (int rankOffset in Math.Abs(fileOffset) == 1 ? new[] {0} : new[] {-1, 1}) {
					testSquare = new Square(Position, fileOffset, rankOffset);

					while (testSquare.IsValid()) {
						Square enemyKingPosition = PieceOwner == Side.White ? board.BlackKing.Position : board.WhiteKing.Position;
						if (testSquare.IsOccupied(board)) {
							if (!testSquare.IsOccupiedBySide(board, PieceOwner) && Rules.MoveObeysRules(board, testMove, PieceOwner) && testSquare != enemyKingPosition)
								ValidMoves.Add(new Movement(testMove));

							break;
						}

						if (Rules.MoveObeysRules(board, testMove, PieceOwner) && testSquare != enemyKingPosition)
							ValidMoves.Add(new Movement(testMove));

						testSquare = new Square(testSquare, fileOffset, rankOffset);
					}
				}
			}
		}

		public override Piece Clone() {
			return new Rook(this);
		}
	}
}