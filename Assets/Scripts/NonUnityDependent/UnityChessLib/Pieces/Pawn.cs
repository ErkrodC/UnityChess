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

		public override void UpdateValidMoves(Board board, LinkedList<Turn> previousMoves) {
			LegalMoves.Clear();

			CheckForwardMovingSquares(board);
			CheckAttackingSquares(board);
			CheckEnPassantCaptures(board, previousMoves);
		}

		private void CheckForwardMovingSquares(Board board) {
			Square testSquare = new Square(Position, 0, Color == Side.White ? 1 : -1);
			Movement testMove = new Movement(Position, testSquare);
			
			if (!testSquare.IsOccupied(board) && Rules.MoveObeysRules(board, testMove, Color)) {
				if (Position.Rank == (Color == Side.White ? 7 : 2)) {
					// PSEUDO call to gui method which gets user promotion piece choice
					// ElectedPiece userElection = GUI.getElectionChoice();

					//for now will default to Queen election
					ElectedPiece userElection = ElectedPiece.Queen;
					LegalMoves.Add(new PromotionMove(Position, new Square(testSquare), userElection, Color));
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
				if (testSquare.IsValid() && testSquare.IsOccupiedBySide(board, Color.Complement()) && Rules.MoveObeysRules(board, testMove, Color) && testSquare != enemyKingPosition) {
					bool pawnAtSecondToLastRank = Position.Rank == (Color == Side.White ? 7 : 2);
					Movement move = pawnAtSecondToLastRank ? new PromotionMove(Position, testSquare, ElectedPiece.None, Color) : new Movement(testMove);
					LegalMoves.Add(move);
				}
			}
		}

		private void CheckEnPassantCaptures(Board board, LinkedList<Turn> previousMoves) {
			if (Color == Side.White ? Position.Rank == 5 : Position.Rank == 4) {
				foreach (int fileOffset in new[] {-1, 1}) {
					Square testSquare = new Square(Position, fileOffset, 0);

					if (testSquare.IsValid() && board[testSquare] is Pawn enemyLateralPawn && enemyLateralPawn.Color != Color) {
						Piece lastMovedPiece = previousMoves.Last.Value.Piece;

						if (lastMovedPiece is Pawn pawn && pawn.ID == enemyLateralPawn.ID && pawn.Position.Rank == (pawn.Color == Side.White ? 2 : 7)) {
							testSquare = new Square(testSquare, 0, Color == Side.White ? 1 : -1);
							EnPassantMove testMove = new EnPassantMove(Position, testSquare, enemyLateralPawn);

							if (Rules.MoveObeysRules(board, testMove, Color))
								LegalMoves.Add(new EnPassantMove(Position, testSquare, enemyLateralPawn));
						}
					}
				}
			}
		}

		public override Piece Clone() => new Pawn(this);
	}
}