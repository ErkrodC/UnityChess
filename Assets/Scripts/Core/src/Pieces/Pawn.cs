using System.Collections.Generic;
using UnityChess.Util;

namespace UnityChess.Core {
	public class Pawn : Piece<Pawn> {
		private static readonly int[] adjacentFileOffsets = {-1, 1};

		public Pawn() : base(Side.None) {}
		public Pawn(Side owner) : base(owner) {}

		public override Dictionary<(Square, Square), Movement> CalculateLegalMoves(
			Board board,
			GameConditions gameConditions,
			Square position
		) {
			Dictionary<(Square, Square), Movement> result = null;

			CheckForwardMovingSquares(board, position, ref result);
			CheckAttackingSquares(board, position, ref result);
			CheckEnPassantCaptures(board, position, gameConditions.EnPassantSquare, ref result);

			return result;
		}

		public override string ToTextArt() => Owner switch {
			Side.White => "♟",
			Side.Black => "♙",
			_ => "."
		};

		private void CheckForwardMovingSquares(
			Board board,
			Square position,
			ref Dictionary<(Square, Square), Movement> movesByStartEndSquares
		) {
			int forwardDirection = Owner.ForwardDirection();
			Square endSquare = new(position, 0, forwardDirection);
			Movement testMove = new(position, endSquare);

			if (!board.IsOccupiedAt(endSquare)
			) {
				if (Rules.MoveObeysRules(board, testMove, Owner)) {
					if (movesByStartEndSquares == null) {
						movesByStartEndSquares = new Dictionary<(Square, Square), Movement>();
					}

					bool amOnEnemyPawnRank = position.Rank == Owner.Complement().PawnRank();
					movesByStartEndSquares[(position, endSquare)] = amOnEnemyPawnRank
						? new PromotionMove(position, endSquare)
						: new Movement(position, endSquare);
				}

				if (position.Rank == Owner.PawnRank()) {
					endSquare += new Square(0, forwardDirection);
					testMove = new Movement(position, endSquare);
					if (!board.IsOccupiedAt(endSquare)
					    && Rules.MoveObeysRules(board, testMove, Owner)
					) {
						if (movesByStartEndSquares == null) {
							movesByStartEndSquares = new Dictionary<(Square, Square), Movement>();
						}

						movesByStartEndSquares[(testMove.Start, testMove.End)] = new Movement(testMove);
					}
				}
			}
		}

		private void CheckAttackingSquares(
			Board board,
			Square position,
			ref Dictionary<(Square, Square), Movement> movesByStartEndSquares
		) {
			foreach (int fileOffset in adjacentFileOffsets) {
				Square endSquare = position + new Square(fileOffset, Owner.ForwardDirection());
				Movement testMove = new Movement(position, endSquare);

				if (endSquare.IsValid()
					&& board.IsOccupiedBySideAt(endSquare, Owner.Complement())
				    && Rules.MoveObeysRules(board, testMove, Owner)
				) {
					if (movesByStartEndSquares == null) {
						movesByStartEndSquares = new Dictionary<(Square, Square), Movement>();
					}

					bool amOnEnemyPawnRank = position.Rank == Owner.Complement().PawnRank();
					movesByStartEndSquares[(testMove.Start, testMove.End)] = amOnEnemyPawnRank
						? new PromotionMove(position, endSquare)
						: new Movement(testMove);
				}
			}
		}

		private void CheckEnPassantCaptures(
			Board board,
			Square position,
			Square enPassantEligibleSquare,
			ref Dictionary<(Square, Square), Movement> movesByStartEndSquares
		) {
			if (position.Rank != Owner.EnPassantVantageRank()) {
				return;
			}

			Square capturedPawnSquare = enPassantEligibleSquare + new Square(0, Owner.BackwardDirection());
			if (capturedPawnSquare.IsValid()
			    && board[capturedPawnSquare] is Pawn capturedPawn
			    && capturedPawn.Owner != Owner
			    && Rules.MoveObeysRules(board, new EnPassantMove(position, enPassantEligibleSquare, capturedPawnSquare), Owner)
			) {
				if (movesByStartEndSquares == null) {
					movesByStartEndSquares = new Dictionary<(Square, Square), Movement>();
				}

				movesByStartEndSquares[(position, enPassantEligibleSquare)] = new EnPassantMove(
					position,
					enPassantEligibleSquare,
					capturedPawnSquare
				);
			}
		}
	}
}