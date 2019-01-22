using System.Collections.Generic;

namespace UnityChess {
	public class Pawn : Piece {
		public delegate ElectedPiece PieceChoiceAction();

		private static int instanceCounter;

		public Pawn(Square startingPosition, Side color) : base(startingPosition, color) {
			ID = ++instanceCounter;
		}

		private Pawn(Pawn pawnCopy) : base(pawnCopy) {
			ID = pawnCopy.ID;
		}

		public override void UpdateLegalMoves(Board board, Square enPassantEligibleSquare) {
			LegalMoves.Clear();

			CheckForwardMovingSquares(board);
			CheckAttackingSquares(board);
			CheckEnPassantCaptures(board, enPassantEligibleSquare);
		}

		private void CheckForwardMovingSquares(Board board) {
			Square testSquare = new Square(Position, 0, Color == Side.White ? 1 : -1);
			Movement testMove = new Movement(Position, testSquare);
			
			if (!testSquare.IsOccupied(board) && Rules.MoveObeysRules(board, testMove, Color)) {
				if (Position.Rank == (Color == Side.White ? 7 : 2)) {
					LegalMoves.Add(new PromotionMove(Position, testSquare));
				} else {
					LegalMoves.Add(new Movement(testMove));

					if (!HasMoved) {
						testSquare = new Square(testSquare, 0, Color == Side.White ? 1 : -1);
						testMove = new Movement(Position, testSquare);
						if (!testSquare.IsOccupied(board) && Rules.MoveObeysRules(board, testMove, Color))
							LegalMoves.Add(new Movement(testMove));
					}
				}
			}
		}

		private void CheckAttackingSquares(Board board) {
			foreach (int fileOffset in new[] {-1, 1}) {
				int rankOffset = Color == Side.White ? 1 : -1;
				Square testSquare = new Square(Position, fileOffset, rankOffset);
				Movement testMove = new Movement(Position, testSquare);

				Square enemyKingPosition = Color == Side.White ? board.BlackKing.Position : board.WhiteKing.Position;
				if (testSquare.IsValid && testSquare.IsOccupiedBySide(board, Color.Complement()) && Rules.MoveObeysRules(board, testMove, Color) && testSquare != enemyKingPosition) {
					bool pawnAtSecondToLastRank = Position.Rank == (Color == Side.White ? 7 : 2);
					Movement move = pawnAtSecondToLastRank ? new PromotionMove(Position, testSquare) : new Movement(testMove);
					LegalMoves.Add(move);
				}
			}
		}

		private void CheckEnPassantCaptures(Board board, Square enPassantEligibleSquare) {
			if (Color == Side.White ? Position.Rank == 5 : Position.Rank == 4) {
				foreach (int fileOffset in new[] {-1, 1}) {
					Square lateralSquare = new Square(Position, fileOffset, 0);

					if (lateralSquare.IsValid && board[lateralSquare] is Pawn enemyLateralPawn && enemyLateralPawn.Color != Color) {
						Square squareToCheckWithEligibleSquare = new Square(enemyLateralPawn.Position, 0, enemyLateralPawn.Color == Side.White ? -1 : 1);
						if (squareToCheckWithEligibleSquare.Equals(enPassantEligibleSquare)) {
							EnPassantMove testMove = new EnPassantMove(Position, enPassantEligibleSquare, enemyLateralPawn);

							if (Rules.MoveObeysRules(board, testMove, Color))
								LegalMoves.Add(new EnPassantMove(Position, enPassantEligibleSquare, enemyLateralPawn));
						}
					}
				}
			}
		}

		public override Piece Clone() => new Pawn(this);
	}
}