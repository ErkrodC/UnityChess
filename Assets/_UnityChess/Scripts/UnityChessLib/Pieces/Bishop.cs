using System.Collections.Generic;

namespace UnityChess {
	public class Bishop : Piece {
		private static int instanceCounter;

		public Bishop(Square startingPosition, Side side) : base(startingPosition, side) {
			ID = ++instanceCounter;
		}

		private Bishop(Bishop bishopCopy) : base(bishopCopy) {
			ID = bishopCopy.ID;
		}

		public override void UpdateValidMoves(Board board, LinkedList<Movement> previousMoves, Side turn) {
			ValidMoves.Clear();

			CheckDiagonalDirections(board, turn);
		}

		private void CheckDiagonalDirections(Board board, Side turn) {
			Square testSquare = new Square(Position);
			Movement testMove = new Movement(testSquare, this);

			foreach (int fileOffset in new[] {-1, 1}) {
				foreach (int rankOffset in new[] {-1, 1}) {
					testSquare = new Square(Position, fileOffset, rankOffset);

					while (testSquare.IsValid()) {
						if (testSquare.IsOccupied(board)) {
							if (!testSquare.IsOccupiedBySide(board, Side) && Rules.MoveObeysRules(board, testMove, turn) && !testSquare.Equals(Side == Side.White ? board.BlackKing.Position : board.WhiteKing.Position)) {
								ValidMoves.Add(new Movement(testMove));
							}

							break;
						}

						if (Rules.MoveObeysRules(board, testMove, turn) && !testSquare.Equals(Side == Side.White ? board.BlackKing.Position : board.WhiteKing.Position)) {
							ValidMoves.Add(new Movement(testMove));
						}

						testSquare = new Square(testSquare, fileOffset, rankOffset);
					}
				}
			}
		}

		public override Piece Clone() {
			return new Bishop(this);
		}
	}
}