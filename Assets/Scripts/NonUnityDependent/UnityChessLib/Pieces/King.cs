using System.Collections.Generic;
using System.Linq;

namespace UnityChess {
	public class King : Piece {
		private static int instanceCounter;

		public King(Square startingPosition, Side pieceOwner) : base(startingPosition, pieceOwner) {
			ID = ++instanceCounter;
		}

		private King(King kingCopy) : base(kingCopy) {
			ID = kingCopy.ID;
		}

		public override void UpdateValidMoves(Board board, LinkedList<Turn> previousMoves) {
			ValidMoves.Clear();

			CheckSurroundingSquares(board);
			CheckCastlingMoves(board);
		}

		public override Piece Clone() => new King(this);

		private void CheckSurroundingSquares(Board board) {
			for (int fileOffset = -1; fileOffset <= 1; fileOffset++) {
				for (int rankOffset = -1; rankOffset <= 1; rankOffset++) {
					if (fileOffset == 0 && rankOffset == 0) continue;

					Square testSquare = new Square(Position, fileOffset, rankOffset);
					Movement testMove = new Movement(Position, testSquare);
					Square enemyKingPosition = PieceOwner == Side.White ? board.BlackKing.Position : board.WhiteKing.Position;
					if (testSquare.IsValid() && !testSquare.IsOccupiedBySide(board, PieceOwner) && Rules.MoveObeysRules(board, testMove, PieceOwner) && testSquare != enemyKingPosition)
						ValidMoves.Add(new Movement(testMove));
				}
			}
		}

		private void CheckCastlingMoves(Board board) {
			if (!HasMoved) {
				bool kingSideCheck = true;
				List<Square> inBtwnSquares = new List<Square>();
				List<Movement> inBtwnMoves = new List<Movement>();
				List<BasePiece> cornerPieces = new List<BasePiece> {
					//kingside corner square
					board.GetBasePiece(Position.File + 3, Position.Rank),
					//queenside corner square
					board.GetBasePiece(Position.File - 4, Position.Rank)
				};

				foreach (Rook rook in cornerPieces.OfType<Rook>()) {
					if (!rook.HasMoved && rook.PieceOwner == PieceOwner) {
						inBtwnSquares.Add(new Square(Position.File + 1 * (kingSideCheck ? 1 : -1), Position.Rank));
						inBtwnSquares.Add(new Square(Position.File + 2 * (kingSideCheck ? 1 : -1), Position.Rank));
						if (!kingSideCheck) inBtwnSquares.Add(new Square(Position.File - 3, Position.Rank));

						if (!inBtwnSquares[0].IsOccupied(board) && !inBtwnSquares[1].IsOccupied(board) && (kingSideCheck || !inBtwnSquares[2].IsOccupied(board))) {
							inBtwnMoves.Add(new Movement(Position, inBtwnSquares[0]));
							inBtwnMoves.Add(new Movement(Position, inBtwnSquares[1]));

							if (Rules.MoveObeysRules(board, inBtwnMoves[0], PieceOwner) && Rules.MoveObeysRules(board, inBtwnMoves[1], PieceOwner)) {
								ValidMoves.Add(new CastlingMove(Position, new Square(inBtwnSquares[1]), rook));
							}

							inBtwnMoves.Clear();
						}

						inBtwnSquares.Clear();
					}

					kingSideCheck = false;
				}
			}
		}
	}
}