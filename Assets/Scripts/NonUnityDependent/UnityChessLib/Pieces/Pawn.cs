using System.Collections.Generic;

namespace UnityChess {
	public class Pawn : Piece {
		public delegate ElectedPiece PieceChoiceAction();

		private static int instanceCounter;

		public Pawn(Square startingPosition, Side pieceOwner) : base(startingPosition, pieceOwner) {
			ID = ++instanceCounter;
		}

		private Pawn(Pawn pawnCopy) : base(pawnCopy) {
			ID = pawnCopy.ID;
		}

		public override void UpdateValidMoves(Board board, LinkedList<Turn> previousMoves) {
			ValidMoves.Clear();

			CheckForwardMovingSquares(board);
			CheckAttackingSquares(board);
			CheckEnPassantCaptures(board, previousMoves);
		}

		private void CheckForwardMovingSquares(Board board) {
			Square testSquare = new Square(Position, 0, PieceOwner == Side.White ? 1 : -1);
			Movement testMove = new Movement(Position, testSquare);
			
			if (!testSquare.IsOccupied(board) && Rules.MoveObeysRules(board, testMove, PieceOwner)) {
				if (Position.Rank == (PieceOwner == Side.White ? 7 : 2)) {
					// PSEUDO call to gui method which gets user promotion piece choice
					// ElectedPiece userElection = GUI.getElectionChoice();

					//for now will default to Queen election
					ElectedPiece userElection = ElectedPiece.Queen;
					ValidMoves.Add(new PromotionMove(Position, new Square(testSquare), userElection, PieceOwner));
				} else {
					ValidMoves.Add(new Movement(testMove));

					if (!HasMoved) {
						testSquare = new Square(testSquare, 0, PieceOwner == Side.White ? 1 : -1);
						testMove = new Movement(Position, testSquare);
						if (!testSquare.IsOccupied(board) && Rules.MoveObeysRules(board, testMove, PieceOwner))
							ValidMoves.Add(new Movement(testMove));
					}
				}
			}
		}

		private void CheckAttackingSquares(Board board) {
			foreach (int fileOffset in new[] {-1, 1}) {
				int rankOffset = PieceOwner == Side.White ? 1 : -1;
				Square testSquare = new Square(Position, fileOffset, rankOffset);
				Movement testMove = new Movement(Position, testSquare);

				Square enemyKingPosition = PieceOwner == Side.White ? board.BlackKing.Position : board.WhiteKing.Position;
				if (testSquare.IsValid() && testSquare.IsOccupiedBySide(board, PieceOwner.Complement()) && Rules.MoveObeysRules(board, testMove, PieceOwner) && testSquare != enemyKingPosition) {
					bool pawnAtSecondToLastRank = Position.Rank == (PieceOwner == Side.White ? 7 : 2);
					Movement move = pawnAtSecondToLastRank ? new PromotionMove(Position, testSquare, ElectedPiece.None, PieceOwner) : new Movement(testMove);
					ValidMoves.Add(move);
				}
			}
		}

		private void CheckEnPassantCaptures(Board board, LinkedList<Turn> previousMoves) {
			if (PieceOwner == Side.White ? Position.Rank == 5 : Position.Rank == 4) {
				foreach (int fileOffset in new[] {-1, 1}) {
					Square testSquare = new Square(Position, fileOffset, 0);

					if (testSquare.IsValid() && board.GetPiece(testSquare) is Pawn enemyLateralPawn && enemyLateralPawn.PieceOwner != PieceOwner) {
						Piece lastMovedPiece = previousMoves.Last.Value.Piece;

						// TODO verify Equals call works 
						if (lastMovedPiece is Pawn pawn && Equals(pawn, enemyLateralPawn) && pawn.Position.Rank == (pawn.PieceOwner == Side.White ? 2 : 7)) {
							EnPassantMove testMove = new EnPassantMove(Position, new Square(testSquare.Rank + (PieceOwner == Side.White ? 1 : -1)), enemyLateralPawn);

							if (Rules.MoveObeysRules(board, testMove, PieceOwner))
								ValidMoves.Add(new EnPassantMove(Position, new Square(testSquare.Rank + (PieceOwner == Side.White ? 1 : -1)), enemyLateralPawn));
						}
					}
				}
			}
		}

		public override Piece Clone() => new Pawn(this);
	}
}