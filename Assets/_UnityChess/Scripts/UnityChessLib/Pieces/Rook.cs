using System;
using System.Collections.Generic;

namespace UnityChess {
	public class Rook : Piece {
		private static int instanceCounter;

		public Rook(Square startingPosition, Side side) : base(startingPosition, side) {
			ID = ++instanceCounter;
		}

		private Rook(Rook rookCopy) : base(rookCopy) {
			ID = rookCopy.ID;
		}

		public override void UpdateValidMoves(Board board, LinkedList<Movement> previousMoves, Side turn) {
			ValidMoves.Clear();

			CheckCardinalDirections(board, turn);
		}

		private void CheckCardinalDirections(Board board, Side turn) {
			Square testSquare = new Square(Position);
			Movement testMove = new Movement(testSquare, this);

			foreach (int fileOffset in new[] {-1, 0, 1}) {
				foreach (int rankOffset in Math.Abs(fileOffset) == 1 ? new[] {0} : new[] {-1, 1}) {
					testSquare = new Square(Position, fileOffset, rankOffset);

					while (testSquare.IsValid()) {
						if (testSquare.IsOccupied(board)) {
							if (!testSquare.IsOccupiedBySide(board, Side) && Rules.MoveObeysRules(board, testMove, turn) && !testSquare.Equals(Side == Side.White ? board.BlackKing.Position : board.WhiteKing.Position))
								ValidMoves.Add(new Movement(testMove));

							break;
						}

						if (Rules.MoveObeysRules(board, testMove, turn) && !testSquare.Equals(Side == Side.White ? board.BlackKing.Position : board.WhiteKing.Position))
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