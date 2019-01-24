using System;

namespace UnityChess {
	public class Rook : Piece {
		public Rook(Square startingPosition, Side color) : base(startingPosition, color) {}
		public Rook(Rook rookCopy) : base(rookCopy) {}
		
		public override void UpdateLegalMoves(Board board, Square enPassantEligibleSquare) {
			LegalMoves.Clear();

			CheckCardinalDirections(board);
		}

		private void CheckCardinalDirections(Board board) {
			foreach (int fileOffset in new[] {-1, 0, 1}) {
				foreach (int rankOffset in Math.Abs(fileOffset) == 1 ? new[] {0} : new[] {-1, 1}) {
					Square testSquare = new Square(Position, fileOffset, rankOffset);
					Movement testMove = new Movement(Position, testSquare);

					while (testSquare.IsValid) {
						Square enemyKingPosition = Color == Side.White ? board.BlackKing.Position : board.WhiteKing.Position;
						if (board.IsOccupied(testSquare)) {
							if (!board.IsOccupiedBySide(testSquare, Color) && Rules.MoveObeysRules(board, testMove, Color) && testSquare != enemyKingPosition)
								LegalMoves.Add(new Movement(testMove));

							break;
						}

						if (Rules.MoveObeysRules(board, testMove, Color) && testSquare != enemyKingPosition)
							LegalMoves.Add(new Movement(testMove));

						testSquare = new Square(testSquare, fileOffset, rankOffset);
						testMove = new Movement(Position, testSquare);
					}
				}
			}
		}
	}
}