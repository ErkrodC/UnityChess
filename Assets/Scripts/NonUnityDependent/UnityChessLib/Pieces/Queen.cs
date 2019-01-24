namespace UnityChess {
	public class Queen : Piece {
		public Queen(Square startingPosition, Side color) : base(startingPosition, color) {}
		public Queen(Queen queenCopy) : base(queenCopy) {}

		public override void UpdateLegalMoves(Board board, Square enPassantEligibleSquare) {
			LegalMoves.Clear();

			CheckRoseDirections(board);
		}

		private void CheckRoseDirections(Board board) {
			foreach (int fileOffset in new[] {-1, 0, 1}) {
				foreach (int rankOffset in new[] {-1, 0, 1}) {
					if (fileOffset == 0 && rankOffset == 0) continue;

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