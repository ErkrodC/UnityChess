using System.Collections.Generic;

namespace UnityChess {
	public class Queen : Piece {
		private static int instanceCounter;

		public Queen(Square startingPosition, Side side) : base(startingPosition, side) {
			ID = ++instanceCounter;
		}

		private Queen(Queen queenCopy) : base(queenCopy) {
			ID = queenCopy.ID;
		}

		public override void UpdateValidMoves(Board board, LinkedList<Movement> previousMoves, Side turn) {
			ValidMoves.Clear();

			CheckRoseDirections(board, turn);
		}

		private void CheckRoseDirections(Board board, Side turn) {
			Square testSquare = new Square(Position);
			Movement testMove = new Movement(testSquare, this);

			foreach (int i in new[] {-1, 0, 1}) {
				foreach (int j in new[] {-1, 0, 1}) {
					if (i == 0 && j == 0) continue;

					testSquare.CopyPosition(Position);
					testSquare.AddVector(i, j);

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

						testSquare.AddVector(i, j);
					}
				}
			}
		}

		public override Piece Clone() {
			return new Queen(this);
		}
	}
}