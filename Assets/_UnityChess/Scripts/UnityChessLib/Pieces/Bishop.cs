using System.Collections.Generic;
using UnityEngine;

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

			foreach (int i in new[] {-1, 1}) {
				foreach (int j in new[] {-1, 1}) {
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
			return new Bishop(this);
		}
	}
}