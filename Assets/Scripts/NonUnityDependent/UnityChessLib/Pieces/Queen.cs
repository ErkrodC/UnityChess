using System.Collections.Generic;

namespace UnityChess {
	public class Queen : Piece {
		private static int instanceCounter;

		public Queen(Square startingPosition, Side pieceOwner) : base(startingPosition, pieceOwner) {
			ID = ++instanceCounter;
		}

		private Queen(Queen queenCopy) : base(queenCopy) {
			ID = queenCopy.ID;
		}

		public override void UpdateValidMoves(Board board, LinkedList<Turn> previousMoves) {
			ValidMoves.Clear();

			CheckRoseDirections(board);
		}

		private void CheckRoseDirections(Board board) {
			foreach (int fileOffset in new[] {-1, 0, 1}) {
				foreach (int rankOffset in new[] {-1, 0, 1}) {
					if (fileOffset == 0 && rankOffset == 0) continue;

					Square testSquare = new Square(Position, fileOffset, rankOffset);
					Movement testMove = new Movement(Position, testSquare);

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
						testMove = new Movement(Position, testSquare);
					}
				}
			}
		}

		public override Piece Clone() => new Queen(this);
	}
}