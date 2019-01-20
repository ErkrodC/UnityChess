using NUnit.Framework;
using System.Collections.Generic;

namespace UnityChess.Test {
	[TestFixture]
	public class RulesTests {
		private Board board;

		//sets up a chess position to test
		public delegate void PositionInitializer(Board board, Side side);

		[SetUp]
		public void Init() {
			board = new Board();
			board.SetBlankBoard();
		}

		[Test]
		[TestCaseSource(typeof(RulesTestData), nameof(RulesTestData.CheckCases))]
		[TestCaseSource(typeof(RulesTestData), nameof(RulesTestData.CheckmateCases))]
		public void IsPlayerInCheck_CheckPositions_ReturnsTrue(PositionInitializer arrange, Side side) {
			arrange(board, side);

			bool actual = Rules.IsPlayerInCheck(board, side);

			Assert.AreEqual(true, actual, arrange.Method.Name);
			Assert.Pass(arrange.Method.Name);
		}

		[Test]
		[TestCaseSource(typeof(RulesTestData), nameof(RulesTestData.NoneCases))]
		[TestCaseSource(typeof(RulesTestData), nameof(RulesTestData.StalemateCases))]
		public void IsPlayerInCheck_NoncheckPositions_ReturnsFalse(PositionInitializer arrange, Side side) {
			arrange(board, side);

			bool actual = Rules.IsPlayerInCheck(board, side);

			Assert.AreEqual(false, actual, arrange.Method.Name);
			Assert.Pass(arrange.Method.Name);
		}

		[Test]
		[TestCaseSource(typeof(RulesTestData), nameof(RulesTestData.StalemateCases))]
		public void IsPlayerStalemated_StalematedPosition_ReturnsTrue(PositionInitializer arrange, Side side) {
			arrange(board, side);

			bool actual = Rules.IsPlayerStalemated(board, side);

			Assert.AreEqual(true, actual, arrange.Method.Name);
			Assert.Pass(arrange.Method.Name);
		}

		[Test]
		[TestCaseSource(typeof(RulesTestData), nameof(RulesTestData.NoneCases))]
		[TestCaseSource(typeof(RulesTestData), nameof(RulesTestData.CheckCases))]
		[TestCaseSource(typeof(RulesTestData), nameof(RulesTestData.CheckmateCases))]
		public void IsPlayerStalemated_NonstalematedPosition_ReturnsFalse(PositionInitializer arrange, Side side) {
			arrange(board, side);

			bool actual = Rules.IsPlayerStalemated(board, side);

			Assert.AreEqual(false, actual, arrange.Method.Name);
			Assert.Pass(arrange.Method.Name);
		}

		[Test]
		[TestCaseSource(typeof(RulesTestData), nameof(RulesTestData.CheckmateCases))]
		public void IsPlayerCheckmated_CheckmatedPosition_ReturnsTrue(PositionInitializer arrange, Side side) {
			arrange(board, side);

			bool actual = Rules.IsPlayerCheckmated(board, side);

			Assert.AreEqual(true, actual, arrange.Method.Name);
			Assert.Pass(arrange.Method.Name);
		}

		[Test]
		[TestCaseSource(typeof(RulesTestData), nameof(RulesTestData.NoneCases))]
		[TestCaseSource(typeof(RulesTestData), nameof(RulesTestData.CheckCases))]
		[TestCaseSource(typeof(RulesTestData), nameof(RulesTestData.StalemateCases))]
		public void IsPlayerCheckmated_NoncheckmatedPosition_ReturnsFalse(PositionInitializer arrange, Side side) {
			arrange(board, side);

			bool actual = Rules.IsPlayerCheckmated(board, side);

			Assert.AreEqual(false, actual, arrange.Method.Name);
			Assert.Pass(arrange.Method.Name);
		}

		private enum Direction {
			Kingside,
			BlackKingside,
			Black,
			BlackQueenside,
			Queenside,
			WhiteQueenside,
			White,
			WhiteKingside
		}

		private enum KnightDirection {
			KingBlackKingside,
			BlackBlackKingside,
			BlackBlackQueenside,
			QueenBlackQueenside,
			QueenWhiteQueenside,
			WhiteWhiteQueenside,
			WhiteWhiteKingside,
			KingWhiteKingside
		}

		private static class RulesTestData {
			private static readonly LinkedList<Turn> dummyPreviousMoves = new LinkedList<Turn>();

			private static void StartingPositionNone(Board board, Side side) {
				board.SetStartingPosition();

				board.InitKings();
				Game.UpdateAllPiecesValidMoves(board, dummyPreviousMoves, side);
			}

			private static void BishopPinRookNone(Board board, Side side) {
				King pinnedKing = new King(new Square(4, 4), side);
				Rook pinnedRook = new Rook(new Square(5, 5), side);
				King pinningKing = new King(new Square(8, 1), side.Complement());
				Bishop pinningBishop = new Bishop(new Square(6, 6), side.Complement());

				PlacePieces(board, pinnedKing, pinnedRook, pinningKing, pinningBishop);

				board.InitKings();
				Game.UpdateAllPiecesValidMoves(board, dummyPreviousMoves, side);
			}

			private static void RookPinBishopNone(Board board, Side side) {
				King pinnedKing = new King(new Square(4, 4), side);
				Bishop pinnedBishop = new Bishop(new Square(4, 5), side);
				King pinningKing = new King(new Square(8, 1), side.Complement());
				Rook pinningRook = new Rook(new Square(4, 6), side.Complement());
				
				PlacePieces(board, pinnedKing, pinnedBishop, pinningKing, pinningRook);

				board.InitKings();
				Game.UpdateAllPiecesValidMoves(board, dummyPreviousMoves, side);
			}

			private static void NeutralKnightNone(Board board, Side side) {
				King defensiveKing = new King(new Square(4, 4), side);
				King offensiveKing = new King(new Square(8, 1), side.Complement());
				Knight offensiveKnight = new Knight(new Square(6, 4), side.Complement());

				PlacePieces(board, defensiveKing, offensiveKing, offensiveKnight);

				board.InitKings();
				Game.UpdateAllPiecesValidMoves(board, dummyPreviousMoves, side);
			}

			private static void NeutralPawnsNone(Board board, Side side) {
				King defensiveKing;
				King offensiveKing;
				Pawn offensivePawn1;
				Pawn offensivePawn2;

				if (side == Side.White) {
					defensiveKing = new King(new Square(4, 4), side);
					offensiveKing = new King(new Square(8, 1), side.Complement());
					offensivePawn1 = new Pawn(new Square(3, 3), side.Complement());
					offensivePawn2 = new Pawn(new Square(5, 3), side.Complement());
				} else {
					defensiveKing = new King(new Square(4, 4), side);
					offensiveKing = new King(new Square(8, 1), side.Complement());
					offensivePawn1 = new Pawn(new Square(3, 5), side.Complement());
					offensivePawn2 = new Pawn(new Square(5, 5), side.Complement());
				}

				PlacePieces(board, defensiveKing, offensiveKing, offensivePawn1, offensivePawn2);

				board.InitKings();
				Game.UpdateAllPiecesValidMoves(board, dummyPreviousMoves, side);
			}

			private static PositionInitializer QueenCheck(Direction direction) {
				Square queenSquare;

				switch (direction) {
					case Direction.Kingside:
						queenSquare = new Square(8, 4);
						break;
					case Direction.BlackKingside:
						queenSquare = new Square(8, 8);
						break;
					case Direction.Black:
						queenSquare = new Square(4, 5);
						break;
					case Direction.BlackQueenside:
						queenSquare = new Square(3, 5);
						break;
					case Direction.Queenside:
						queenSquare = new Square(1, 4);
						break;
					case Direction.WhiteQueenside:
						queenSquare = new Square(1, 1);
						break;
					case Direction.White:
						queenSquare = new Square(4, 3);
						break;
					case Direction.WhiteKingside:
						queenSquare = new Square(5, 3);
						break;
					default:
						queenSquare = default(Square);
						break;
				}

				PositionInitializer initializer = (board, side) => {
					King checkedKing = new King(new Square(4, 4), side);
					King checkingKing = new King(new Square(8, 1), side.Complement());
					Queen checkingQueen = new Queen(queenSquare, side.Complement());

					PlacePieces(board, checkedKing, checkingKing, checkingQueen);

					board.InitKings();
					Game.UpdateAllPiecesValidMoves(board, dummyPreviousMoves, side);
				};

				return initializer;
			}

			private static PositionInitializer RookCheck(Direction direction) {
				Square rookSquare;

				switch (direction) {
					case Direction.Kingside:
						rookSquare = new Square(5, 4);
						break;
					case Direction.Black:
						rookSquare = new Square(4, 8);
						break;
					case Direction.Queenside:
						rookSquare = new Square(3, 4);
						break;
					case Direction.White:
						rookSquare = new Square(4, 1);
						break;
					default:
						rookSquare = default(Square);
						break;
				}

				PositionInitializer initializer = (board, side) => {
					King checkedKing = new King(new Square(4, 4), side);
					King checkingKing = new King(new Square(8, 1), side.Complement());
					Rook checkingRook = new Rook(rookSquare, side.Complement());

					PlacePieces(board, checkedKing, checkingKing, checkingRook);

					board.InitKings();
					Game.UpdateAllPiecesValidMoves(board, dummyPreviousMoves, side);
				};

				return initializer;
			}

			private static PositionInitializer BishopCheck(Direction direction) {
				Square bishopSquare;

				switch (direction) {
					case Direction.BlackKingside:
						bishopSquare = new Square(5, 5);
						break;
					case Direction.BlackQueenside:
						bishopSquare = new Square(1, 7);
						break;
					case Direction.WhiteQueenside:
						bishopSquare = new Square(3, 3);
						break;
					case Direction.WhiteKingside:
						bishopSquare = new Square(7, 1);
						break;
					default:
						bishopSquare = default(Square);
						break;
				}

				PositionInitializer initializer = (board, side) => {
					King checkedKing = new King(new Square(4, 4), side);
					King checkingKing = new King(new Square(8, 1), side.Complement());
					Bishop checkingBishop = new Bishop(bishopSquare, side.Complement());
					
					PlacePieces(board, checkedKing, checkingKing, checkingBishop);

					board.InitKings();
					Game.UpdateAllPiecesValidMoves(board, dummyPreviousMoves, side);
				};

				return initializer;
			}

			private static PositionInitializer KnightCheck(KnightDirection direction) {
				Square knightSquare;

				switch (direction) {
					case KnightDirection.KingBlackKingside:
						knightSquare = new Square(6, 5);
						break;
					case KnightDirection.BlackBlackKingside:
						knightSquare = new Square(5, 6);
						break;
					case KnightDirection.BlackBlackQueenside:
						knightSquare = new Square(3, 6);
						break;
					case KnightDirection.QueenBlackQueenside:
						knightSquare = new Square(2, 5);
						break;
					case KnightDirection.QueenWhiteQueenside:
						knightSquare = new Square(2, 3);
						break;
					case KnightDirection.WhiteWhiteQueenside:
						knightSquare = new Square(3, 2);
						break;
					case KnightDirection.WhiteWhiteKingside:
						knightSquare = new Square(5, 2);
						break;
					case KnightDirection.KingWhiteKingside:
						knightSquare = new Square(6, 3);
						break;
					default:
						knightSquare = default(Square);
						break;
				}

				PositionInitializer initializer = (board, side) => {
					King checkedKing = new King(new Square(4, 4), side);
					King checkingKing = new King(new Square(8, 1), side.Complement());
					Knight checkingKnight = new Knight(knightSquare, side.Complement());
					
					PlacePieces(board, checkedKing, checkingKing, checkingKnight);

					board.InitKings();
					Game.UpdateAllPiecesValidMoves(board, dummyPreviousMoves, side);
				};

				return initializer;
			}

			private static PositionInitializer PawnCheck(Direction direction, Side checkedSide) {
				Square pawnSquare;

				if (checkedSide == Side.White) {
					switch (direction) {
						case Direction.Kingside:
							pawnSquare = new Square(5, 5);
							break;
						case Direction.Queenside:
							pawnSquare = new Square(3, 5);
							break;
						default:
							pawnSquare = default(Square);
							break;
					}
				} else {
					switch (direction) {
						case Direction.Kingside:
							pawnSquare = new Square(5, 3);
							break;
						case Direction.Queenside:
							pawnSquare = new Square(3, 3);
							break;
						default:
							pawnSquare = default(Square);
							break;
					}
				}

				PositionInitializer initializer = (board, side) => {
					King checkedKing = new King(new Square(4, 4), side);
					King checkingKing = new King(new Square(8, 1), side.Complement());
					Pawn checkingPawn = new Pawn(pawnSquare, side.Complement());

					PlacePieces(board, checkedKing, checkingKing, checkingPawn);

					board.InitKings();
					Game.UpdateAllPiecesValidMoves(board, dummyPreviousMoves, side);
				};

				return initializer;
			}

			private static void KingPawnStalemate(Board board, Side side) {
				King stalematedKing;
				King blunderKing;
				Pawn blunderPawn;

				if (side == Side.White) {
					stalematedKing = new King(new Square(6, 1), side);
					blunderKing = new King(new Square(6, 3), side.Complement());
					blunderPawn = new Pawn(new Square(6, 2), side.Complement());
				} else {
					stalematedKing = new King(new Square(6, 8), side);
					blunderKing = new King(new Square(6, 6), side.Complement());
					blunderPawn = new Pawn(new Square(6, 7), side.Complement());
				}

				PlacePieces(board, stalematedKing, blunderKing, blunderPawn);

				board.InitKings();
				Game.UpdateAllPiecesValidMoves(board, dummyPreviousMoves, side);
			}

			private static void KingRookStalemate(Board board, Side side) {
				King stalematedKing = new King(new Square(1, 1), side);
				King blunderKing = new King(new Square(3, 3), side.Complement());
				Rook blunderRook = new Rook(new Square(2, 2), side.Complement());
				
				PlacePieces(board, stalematedKing, blunderKing, blunderRook);

				board.InitKings();
				Game.UpdateAllPiecesValidMoves(board, dummyPreviousMoves, side);
			}

			private static void KingBishopStalemate(Board board, Side side) {
				King stalematedKing = new King(new Square(1, 8), side);
				King blunderKing = new King(new Square(1, 6), side.Complement());
				Bishop blunderBishop = new Bishop(new Square(6, 4), side.Complement());

				PlacePieces(board, stalematedKing, blunderKing, blunderBishop);

				board.InitKings();
				Game.UpdateAllPiecesValidMoves(board, dummyPreviousMoves, side);
			}

			private static void RookPinBishopStalemate(Board board, Side side) {
				King stalematedKing = new King(new Square(1, 8), side);
				King blunderKing = new King(new Square(2, 6), side.Complement());
				Rook blunderRook = new Rook(new Square(8, 8), side.Complement());
				Bishop stalematedBishop = new Bishop(new Square(2, 8), side);

				PlacePieces(board, stalematedKing, blunderKing, blunderRook, stalematedBishop);

				board.InitKings();
				Game.UpdateAllPiecesValidMoves(board, dummyPreviousMoves, side);
			}

			private static void QueenStalemate(Board board, Side side) {
				King stalematedKing = new King(new Square(1, 1), side);
				King blunderKing = new King(new Square(8, 8), side.Complement());
				Queen blunderQueen = new Queen(new Square(2, 3), side.Complement());

				PlacePieces(board, stalematedKing, blunderKing, blunderQueen);

				board.InitKings();
				Game.UpdateAllPiecesValidMoves(board, dummyPreviousMoves, side);
			}

			private static void AnandVsKramnikStalemate(Board board, Side side) {
				King stalematedKing;
				Pawn stalematedPawn;
				King blunderKing;
				Pawn blunderPawn1;
				Pawn blunderPawn2;

				if (side == Side.White) {
					stalematedKing = new King(new Square(8, 5), side);
					stalematedPawn = new Pawn(new Square(8, 4), side);
					blunderKing = new King(new Square(6, 5), side.Complement());
					blunderPawn1 = new Pawn(new Square(6, 6), side.Complement());
					blunderPawn2 = new Pawn(new Square(7, 7), side.Complement());
				} else {
					stalematedKing = new King(new Square(8, 4), side);
					stalematedPawn = new Pawn(new Square(8, 5), side);
					blunderKing = new King(new Square(6, 4), side.Complement());
					blunderPawn1 = new Pawn(new Square(6, 3), side.Complement());
					blunderPawn2 = new Pawn(new Square(7, 2), side.Complement());
				}

				PlacePieces(board, stalematedKing, stalematedPawn, blunderKing, blunderPawn1, blunderPawn2);

				board.InitKings();
				Game.UpdateAllPiecesValidMoves(board, dummyPreviousMoves, side);
			}

			private static void KorchnoiVsKarpovStalemate(Board board, Side side) {
				King stalematedKing;
				Pawn stalematedPawn;
				King blunderKing;
				Bishop blunderBishop;
				Pawn blunderPawn;

				if (side == Side.White) {
					stalematedKing = new King(new Square(8, 2), side);
					stalematedPawn = new Pawn(new Square(1, 5), side);
					blunderKing = new King(new Square(6, 2), side.Complement());
					blunderBishop = new Bishop(new Square(7, 2), side.Complement());
					blunderPawn = new Pawn(new Square(1, 6), side.Complement());
				} else {
					stalematedKing = new King(new Square(8, 7), side);
					stalematedPawn = new Pawn(new Square(1, 4), side);
					blunderKing = new King(new Square(6, 7), side.Complement());
					blunderBishop = new Bishop(new Square(7, 7), side.Complement());
					blunderPawn = new Pawn(new Square(1, 3), side.Complement());
				}
				
				PlacePieces(board, stalematedKing, stalematedPawn, blunderKing, blunderBishop, blunderPawn);

				board.InitKings();
				Game.UpdateAllPiecesValidMoves(board, dummyPreviousMoves, side);
			}

			private static void BernsteinVsSmyslovStalemate(Board board, Side side) {
				King stalematedKing;
				King blunderKing;
				Rook blunderRook;
				Pawn blunderPawn;

				if (side == Side.White) {
					stalematedKing = new King(new Square(6, 3), side);
					blunderKing = new King(new Square(6, 5), side.Complement());
					blunderRook = new Rook(new Square(2, 2), side.Complement());
					blunderPawn = new Pawn(new Square(6, 4), side.Complement());
				} else {
					stalematedKing = new King(new Square(6, 6), side);
					blunderKing = new King(new Square(6, 4), side.Complement());
					blunderRook = new Rook(new Square(2, 7), side.Complement());
					blunderPawn = new Pawn(new Square(6, 5), side.Complement());
				}

				PlacePieces(board, stalematedKing, blunderKing, blunderRook, blunderPawn);

				board.InitKings();
				Game.UpdateAllPiecesValidMoves(board, dummyPreviousMoves, side);
			}

			private static void GelfandVsKramnikStalemate(Board board, Side side) {
				King stalematedKing;
				Pawn[] stalematedPawns = new Pawn[4];
				King blunderKing;
				Rook blunderRook;
				Queen blunderQueen;
				Pawn[] blunderPawns = new Pawn[6];

				if (side == Side.White) {
					stalematedKing = new King(new Square(8, 2), side);
					stalematedPawns[0] = new Pawn(new Square(1, 3), side);
					stalematedPawns[1] = new Pawn(new Square(6, 3), side);
					stalematedPawns[2] = new Pawn(new Square(7, 2), side);
					stalematedPawns[3] = new Pawn(new Square(8, 3), side);
					blunderKing = new King(new Square(8, 7), side.Complement());
					blunderRook = new Rook(new Square(5, 2), side.Complement());
					blunderQueen = new Queen(new Square(4, 1), side.Complement());
					blunderPawns[0] = new Pawn(new Square(1, 4), side.Complement());
					blunderPawns[1] = new Pawn(new Square(4, 5), side.Complement());
					blunderPawns[2] = new Pawn(new Square(6, 4), side.Complement());
					blunderPawns[3] = new Pawn(new Square(6, 6), side.Complement());
					blunderPawns[4] = new Pawn(new Square(7, 5), side.Complement());
					blunderPawns[5] = new Pawn(new Square(8, 4), side.Complement());
				} else {
					stalematedKing = new King(new Square(8, 7), side);
					stalematedPawns[0] = new Pawn(new Square(1, 6), side);
					stalematedPawns[1] = new Pawn(new Square(6, 6), side);
					stalematedPawns[2] = new Pawn(new Square(7, 7), side);
					stalematedPawns[3] = new Pawn(new Square(8, 6), side);
					blunderKing = new King(new Square(8, 2), side.Complement());
					blunderRook = new Rook(new Square(5, 7), side.Complement());
					blunderQueen = new Queen(new Square(4, 8), side.Complement());
					blunderPawns[0] = new Pawn(new Square(1, 5), side.Complement());
					blunderPawns[1] = new Pawn(new Square(4, 4), side.Complement());
					blunderPawns[2] = new Pawn(new Square(6, 5), side.Complement());
					blunderPawns[3] = new Pawn(new Square(6, 3), side.Complement());
					blunderPawns[4] = new Pawn(new Square(7, 4), side.Complement());
					blunderPawns[5] = new Pawn(new Square(8, 5), side.Complement());
				}
				
				PlacePieces(board, stalematedKing, blunderKing, blunderRook, blunderQueen);

				foreach (Pawn stalematedPawn in stalematedPawns) {
					PlacePieces(board, stalematedPawn);
				}

				foreach (Pawn blunderPawn in blunderPawns) {
					PlacePieces(board, blunderPawn);
				}

				board.InitKings();
				Game.UpdateAllPiecesValidMoves(board, dummyPreviousMoves, side);
			}

			private static void DoubleRookCheckmate(Board board, Side side) {
				King checkmatedKing = new King(new Square(1, 1), side);
				King winningKing = new King(new Square(8, 8), side.Complement());
				Rook winningRook1 = new Rook(new Square(8, 1), side.Complement());
				Rook winningRook2 = new Rook(new Square(8, 2), side.Complement());

				
				PlacePieces(board, checkmatedKing, winningKing, winningRook1, winningRook2);

				board.InitKings();
				Game.UpdateAllPiecesValidMoves(board, dummyPreviousMoves, side);
			}

			private static void KingQueenCheckmate(Board board, Side side) {
				King checkmatedKing = new King(new Square(8, 5), side);
				King winningKing = new King(new Square(6, 5), side.Complement());
				Queen winningQueen = new Queen(new Square(7, 5), side.Complement());

				
				PlacePieces(board, checkmatedKing, winningKing, winningQueen);

				board.InitKings();
				Game.UpdateAllPiecesValidMoves(board, dummyPreviousMoves, side);
			}

			private static void KingRookCheckmate(Board board, Side side) {
				King checkmatedKing = new King(new Square(8, 5), side);
				King winningKing = new King(new Square(6, 5), side.Complement());
				Rook winningRook = new Rook(new Square(8, 1), side.Complement());

				PlacePieces(board, checkmatedKing, winningKing, winningRook);

				board.InitKings();
				Game.UpdateAllPiecesValidMoves(board, dummyPreviousMoves, side);
			}

			private static void KingDoubleBishopCheckmate(Board board, Side side) {
				King checkmatedKing = new King(new Square(8, 8), side);
				King winningKing = new King(new Square(7, 6), side.Complement());
				Bishop winningBishop1 = new Bishop(new Square(1, 2), side.Complement());
				Bishop winningBishop2 = new Bishop(new Square(2, 2), side.Complement());
				
				PlacePieces(board, checkmatedKing, winningKing, winningBishop1, winningBishop2);

				board.InitKings();
				Game.UpdateAllPiecesValidMoves(board, dummyPreviousMoves, side);
			}

			private static void KingBishopKnightCheckmate(Board board, Side side) {
				King checkmatedKing = new King(new Square(1, 8), side);
				King winningKing = new King(new Square(2, 6), side.Complement());
				Bishop winningBishop = new Bishop(new Square(3, 6), side.Complement());
				Knight winningKnight = new Knight(new Square(1, 6), side.Complement());
				
				PlacePieces(board, checkmatedKing, winningKing, winningBishop, winningKnight);

				board.InitKings();
				Game.UpdateAllPiecesValidMoves(board, dummyPreviousMoves, side);
			}

			private static void KingDoubleKnightCheckmate(Board board, Side side) {
				King checkmatedKing = new King(new Square(8, 8), side);
				King winningKing = new King(new Square(8, 6), side.Complement());
				Knight winningKnight1 = new Knight(new Square(6, 6), side.Complement());
				Knight winningKnight2 = new Knight(new Square(7, 6), side.Complement());
				
				PlacePieces(board, checkmatedKing, winningKing, winningKnight1, winningKnight2);

				board.InitKings();
				Game.UpdateAllPiecesValidMoves(board, dummyPreviousMoves, side);
			}

			private static void KingDoublePawnCheckmate(Board board, Side side) {
				King checkmatedKing;
				King winningKing;
				Pawn winningPawn1;
				Pawn winningPawn2;

				if (side == Side.White) {
					checkmatedKing = new King(new Square(5, 1), side);
					winningKing = new King(new Square(5, 3), side.Complement());
					winningPawn1 = new Pawn(new Square(5, 2), side.Complement());
					winningPawn2 = new Pawn(new Square(4, 2), side.Complement());
				} else {
					checkmatedKing = new King(new Square(5, 8), side);
					winningKing = new King(new Square(5, 6), side.Complement());
					winningPawn1 = new Pawn(new Square(5, 7), side.Complement());
					winningPawn2 = new Pawn(new Square(4, 7), side.Complement());
				}

				PlacePieces(board, checkmatedKing, winningKing, winningPawn1, winningPawn2);

				board.InitKings();
				Game.UpdateAllPiecesValidMoves(board, dummyPreviousMoves, side);
			}

			private static void BackRankCheckmate(Board board, Side side) {
				King checkmatedKing;
				King winningKing;
				Rook winningRook;
				Pawn losingPawn1;
				Pawn losingPawn2;
				Pawn losingPawn3;

				if (side == Side.White) {
					checkmatedKing = new King(new Square(7, 1), side);
					winningKing = new King(new Square(7, 8), side.Complement());
					winningRook = new Rook(new Square(1, 1), side.Complement());
					losingPawn1 = new Pawn(new Square(6, 2), side);
					losingPawn2 = new Pawn(new Square(7, 2), side);
					losingPawn3 = new Pawn(new Square(8, 2), side);
				} else {
					checkmatedKing = new King(new Square(7, 8), side);
					winningKing = new King(new Square(7, 1), side.Complement());
					winningRook = new Rook(new Square(1, 8), side.Complement());
					losingPawn1 = new Pawn(new Square(6, 7), side);
					losingPawn2 = new Pawn(new Square(7, 7), side);
					losingPawn3 = new Pawn(new Square(8, 7), side);
				}

				PlacePieces(board, checkmatedKing, winningKing, winningRook, losingPawn1, losingPawn2, losingPawn3);

				board.InitKings();
				Game.UpdateAllPiecesValidMoves(board, dummyPreviousMoves, side);
			}

			private static void SmotheredCheckmate(Board board, Side side) {
				King checkmatedKing;
				King winningKing;
				Knight winningKnight;
				Pawn losingPawn1;
				Pawn losingPawn2;
				Rook losingRook;

				if (side == Side.White) {
					checkmatedKing = new King(new Square(8, 1), side);
					winningKing = new King(new Square(7, 7), side.Complement());
					winningKnight = new Knight(new Square(6, 2), side.Complement());
					losingPawn1 = new Pawn(new Square(7, 2), side);
					losingPawn2 = new Pawn(new Square(8, 2), side);
					losingRook = new Rook(new Square(7, 1), side);
				} else {
					checkmatedKing = new King(new Square(8, 8), side);
					winningKing = new King(new Square(7, 2), side.Complement());
					winningKnight = new Knight(new Square(6, 7), side.Complement());
					losingPawn1 = new Pawn(new Square(7, 7), side);
					losingPawn2 = new Pawn(new Square(8, 7), side);
					losingRook = new Rook(new Square(7, 8), side);
				}

				PlacePieces(board, checkmatedKing, winningKing, winningKnight, losingPawn1, losingPawn2, losingRook);

				board.InitKings();
				Game.UpdateAllPiecesValidMoves(board, dummyPreviousMoves, side);
			}

			private static void KnightRookCheckmate(Board board, Side side) {
				King checkmatedKing = new King(new Square(8, 8), side);
				King winningKing = new King(new Square(7, 1), side.Complement());
				Knight winningKnight = new Knight(new Square(6, 6), side.Complement());
				Rook winningRook = new Rook(new Square(8, 7), side.Complement());
				
				PlacePieces(board, checkmatedKing, winningKing, winningKnight, winningRook);

				board.InitKings();
				Game.UpdateAllPiecesValidMoves(board, dummyPreviousMoves, side);
			}

			private static void QueenBishopCheckmate(Board board, Side side) {
				King checkmatedKing = new King(new Square(7, 8), side);
				King winningKing = new King(new Square(7, 1), side.Complement());
				Queen winningQueen = new Queen(new Square(7, 7), side.Complement());
				Bishop winningBishop = new Bishop(new Square(8, 6), side.Complement());

				PlacePieces(board, checkmatedKing, winningKing, winningQueen, winningBishop);

				board.InitKings();
				Game.UpdateAllPiecesValidMoves(board, dummyPreviousMoves, side);
			}

			private static void PlacePieces(Board board, params Piece[] pieces) {
				foreach (Piece piece in pieces) {
					board[piece.Position] = piece;
				}
			}

			public static object[] NoneCases = {
				new object[] {new PositionInitializer(StartingPositionNone), Side.White},
				new object[] {new PositionInitializer(StartingPositionNone), Side.Black},
				new object[] {new PositionInitializer(BishopPinRookNone), Side.White},
				new object[] {new PositionInitializer(BishopPinRookNone), Side.Black},
				new object[] {new PositionInitializer(RookPinBishopNone), Side.White},
				new object[] {new PositionInitializer(RookPinBishopNone), Side.Black},
				new object[] {new PositionInitializer(NeutralKnightNone), Side.White},
				new object[] {new PositionInitializer(NeutralKnightNone), Side.Black},
				new object[] {new PositionInitializer(NeutralPawnsNone), Side.White},
				new object[] {new PositionInitializer(NeutralPawnsNone), Side.Black},
			};

			public static object[] CheckCases = {
				new TestCaseData(new object[] {QueenCheck(Direction.Kingside), Side.White}).SetName("{m}(QueenCheck(Kingside), {1})"),
				new TestCaseData(new object[] {QueenCheck(Direction.Kingside), Side.Black}).SetName("{m}(QueenCheck(Kingside), {1})"),
				new TestCaseData(new object[] {QueenCheck(Direction.BlackKingside), Side.White}).SetName("{m}(QueenCheck(BlackKingside), {1})"),
				new TestCaseData(new object[] {QueenCheck(Direction.BlackKingside), Side.Black}).SetName("{m}(QueenCheck(BlackKingside), {1})"),
				new TestCaseData(new object[] {QueenCheck(Direction.Black), Side.White}).SetName("{m}(QueenCheck(Black), {1})"),
				new TestCaseData(new object[] {QueenCheck(Direction.Black), Side.Black}).SetName("{m}(QueenCheck(Black), {1})"),
				new TestCaseData(new object[] {QueenCheck(Direction.BlackQueenside), Side.White}).SetName("{m}(QueenCheck(BlackQueenside), {1})"),
				new TestCaseData(new object[] {QueenCheck(Direction.BlackQueenside), Side.Black}).SetName("{m}(QueenCheck(BlackQueenside), {1})"),
				new TestCaseData(new object[] {QueenCheck(Direction.Queenside), Side.White}).SetName("{m}(QueenCheck(Queenside), {1})"),
				new TestCaseData(new object[] {QueenCheck(Direction.Queenside), Side.Black}).SetName("{m}(QueenCheck(Queenside), {1})"),
				new TestCaseData(new object[] {QueenCheck(Direction.WhiteQueenside), Side.White}).SetName("{m}(QueenCheck(WhiteQueenside), {1})"),
				new TestCaseData(new object[] {QueenCheck(Direction.WhiteQueenside), Side.Black}).SetName("{m}(QueenCheck(WhiteQueenside), {1})"),
				new TestCaseData(new object[] {QueenCheck(Direction.White), Side.White}).SetName("{m}(QueenCheck(White), {1})"),
				new TestCaseData(new object[] {QueenCheck(Direction.White), Side.Black}).SetName("{m}(QueenCheck(White), {1})"),
				new TestCaseData(new object[] {QueenCheck(Direction.WhiteKingside), Side.White}).SetName("{m}(QueenCheck(WhiteKingside), {1})"),
				new TestCaseData(new object[] {QueenCheck(Direction.WhiteKingside), Side.Black}).SetName("{m}(QueenCheck(WhiteKingside), {1})"),

				new TestCaseData(new object[] {RookCheck(Direction.Kingside), Side.White}).SetName("{m}(RookCheck(Kingside), {1})"),
				new TestCaseData(new object[] {RookCheck(Direction.Kingside), Side.Black}).SetName("{m}(RookCheck(Kingside), {1})"),
				new TestCaseData(new object[] {RookCheck(Direction.Black), Side.White}).SetName("{m}(RookCheck(Black), {1})"),
				new TestCaseData(new object[] {RookCheck(Direction.Black), Side.Black}).SetName("{m}(RookCheck(Black), {1})"),
				new TestCaseData(new object[] {RookCheck(Direction.Queenside), Side.White}).SetName("{m}(RookCheck(Queenside), {1})"),
				new TestCaseData(new object[] {RookCheck(Direction.Queenside), Side.Black}).SetName("{m}(RookCheck(Queenside), {1})"),
				new TestCaseData(new object[] {RookCheck(Direction.White), Side.White}).SetName("{m}(RookCheck(White), {1})"),
				new TestCaseData(new object[] {RookCheck(Direction.White), Side.Black}).SetName("{m}(RookCheck(White), {1})"),

				new TestCaseData(new object[] {BishopCheck(Direction.BlackKingside), Side.White}).SetName("{m}(BishopCheck(BlackKingside), {1})"),
				new TestCaseData(new object[] {BishopCheck(Direction.BlackKingside), Side.Black}).SetName("{m}(BishopCheck(BlackKingside), {1})"),
				new TestCaseData(new object[] {BishopCheck(Direction.BlackQueenside), Side.White}).SetName("{m}(BishopCheck(BlackQueenside), {1})"),
				new TestCaseData(new object[] {BishopCheck(Direction.BlackQueenside), Side.Black}).SetName("{m}(BishopCheck(BlackQueenside), {1})"),
				new TestCaseData(new object[] {BishopCheck(Direction.WhiteQueenside), Side.White}).SetName("{m}(BishopCheck(WhiteQueenside), {1})"),
				new TestCaseData(new object[] {BishopCheck(Direction.WhiteQueenside), Side.Black}).SetName("{m}(BishopCheck(WhiteQueenside), {1})"),
				new TestCaseData(new object[] {BishopCheck(Direction.WhiteKingside), Side.White}).SetName("{m}(BishopCheck(WhiteKingside), {1})"),
				new TestCaseData(new object[] {BishopCheck(Direction.WhiteKingside), Side.Black}).SetName("{m}(BishopCheck(WhiteKingside), {1})"),

				new TestCaseData(new object[] {KnightCheck(KnightDirection.KingBlackKingside), Side.White}).SetName("{m}(KnightCheck(KingBlackKingside), {1})"),
				new TestCaseData(new object[] {KnightCheck(KnightDirection.KingBlackKingside), Side.Black}).SetName("{m}(KnightCheck(KingBlackKingside), {1})"),
				new TestCaseData(new object[] {KnightCheck(KnightDirection.BlackBlackKingside), Side.White}).SetName("{m}(KnightCheck(BlackBlackKingside), {1})"),
				new TestCaseData(new object[] {KnightCheck(KnightDirection.BlackBlackKingside), Side.Black}).SetName("{m}(KnightCheck(BlackBlackKingside), {1})"),
				new TestCaseData(new object[] {KnightCheck(KnightDirection.BlackBlackQueenside), Side.White}).SetName("{m}(KnightCheck(BlackBlackQueenside), {1})"),
				new TestCaseData(new object[] {KnightCheck(KnightDirection.BlackBlackQueenside), Side.Black}).SetName("{m}(KnightCheck(BlackBlackQueenside), {1})"),
				new TestCaseData(new object[] {KnightCheck(KnightDirection.QueenBlackQueenside), Side.White}).SetName("{m}(KnightCheck(QueenBlackQueenside), {1})"),
				new TestCaseData(new object[] {KnightCheck(KnightDirection.QueenBlackQueenside), Side.Black}).SetName("{m}(KnightCheck(QueenBlackQueenside), {1})"),
				new TestCaseData(new object[] {KnightCheck(KnightDirection.QueenWhiteQueenside), Side.White}).SetName("{m}(KnightCheck(QueenWhiteQueenside), {1})"),
				new TestCaseData(new object[] {KnightCheck(KnightDirection.QueenWhiteQueenside), Side.Black}).SetName("{m}(KnightCheck(QueenWhiteQueenside), {1})"),
				new TestCaseData(new object[] {KnightCheck(KnightDirection.WhiteWhiteQueenside), Side.White}).SetName("{m}(KnightCheck(WhiteWhiteQueenside), {1})"),
				new TestCaseData(new object[] {KnightCheck(KnightDirection.WhiteWhiteQueenside), Side.Black}).SetName("{m}(KnightCheck(WhiteWhiteQueenside), {1})"),
				new TestCaseData(new object[] {KnightCheck(KnightDirection.WhiteWhiteKingside), Side.White}).SetName("{m}(KnightCheck(WhiteWhiteKingside), {1})"),
				new TestCaseData(new object[] {KnightCheck(KnightDirection.WhiteWhiteKingside), Side.Black}).SetName("{m}(KnightCheck(WhiteWhiteKingside), {1})"),
				new TestCaseData(new object[] {KnightCheck(KnightDirection.KingWhiteKingside), Side.White}).SetName("{m}(KnightCheck(KingWhiteKingside), {1})"),
				new TestCaseData(new object[] {KnightCheck(KnightDirection.KingWhiteKingside), Side.Black}).SetName("{m}(KnightCheck(KingWhiteKingside), {1})"),

				new TestCaseData(new object[] {PawnCheck(Direction.Kingside, Side.White), Side.White}).SetName("{m}(PawnCheck(Kingside), {1})"),
				new TestCaseData(new object[] {PawnCheck(Direction.Kingside, Side.Black), Side.Black}).SetName("{m}(PawnCheck(Kingside), {1})"),
				new TestCaseData(new object[] {PawnCheck(Direction.Queenside, Side.White), Side.White}).SetName("{m}(PawnCheck(Queenside), {1})"),
				new TestCaseData(new object[] {PawnCheck(Direction.Queenside, Side.Black), Side.Black}).SetName("{m}(PawnCheck(Queenside), {1})"),
			};

			public static object[] StalemateCases = {
				new object[] {new PositionInitializer(KingPawnStalemate), Side.White},
				new object[] {new PositionInitializer(KingPawnStalemate), Side.Black},
				new object[] {new PositionInitializer(KingRookStalemate), Side.White},
				new object[] {new PositionInitializer(KingRookStalemate), Side.Black},
				new object[] {new PositionInitializer(KingBishopStalemate), Side.White},
				new object[] {new PositionInitializer(KingBishopStalemate), Side.Black},
				new object[] {new PositionInitializer(RookPinBishopStalemate), Side.White},
				new object[] {new PositionInitializer(RookPinBishopStalemate), Side.Black},
				new object[] {new PositionInitializer(QueenStalemate), Side.White},
				new object[] {new PositionInitializer(QueenStalemate), Side.Black},
				new object[] {new PositionInitializer(AnandVsKramnikStalemate), Side.White},
				new object[] {new PositionInitializer(AnandVsKramnikStalemate), Side.Black},
				new object[] {new PositionInitializer(KorchnoiVsKarpovStalemate), Side.White},
				new object[] {new PositionInitializer(KorchnoiVsKarpovStalemate), Side.Black},
				new object[] {new PositionInitializer(BernsteinVsSmyslovStalemate), Side.White},
				new object[] {new PositionInitializer(BernsteinVsSmyslovStalemate), Side.Black},
				new object[] {new PositionInitializer(GelfandVsKramnikStalemate), Side.White},
				new object[] {new PositionInitializer(GelfandVsKramnikStalemate), Side.Black},
			};

			public static object[] CheckmateCases = {
				new object[] {new PositionInitializer(DoubleRookCheckmate), Side.White},
				new object[] {new PositionInitializer(DoubleRookCheckmate), Side.Black},
				new object[] {new PositionInitializer(KingQueenCheckmate), Side.White},
				new object[] {new PositionInitializer(KingQueenCheckmate), Side.Black},
				new object[] {new PositionInitializer(KingRookCheckmate), Side.White},
				new object[] {new PositionInitializer(KingRookCheckmate), Side.Black},
				new object[] {new PositionInitializer(KingDoubleBishopCheckmate), Side.White},
				new object[] {new PositionInitializer(KingDoubleBishopCheckmate), Side.Black},
				new object[] {new PositionInitializer(KingBishopKnightCheckmate), Side.White},
				new object[] {new PositionInitializer(KingBishopKnightCheckmate), Side.Black},
				new object[] {new PositionInitializer(KingDoubleKnightCheckmate), Side.White},
				new object[] {new PositionInitializer(KingDoubleKnightCheckmate), Side.Black},
				new object[] {new PositionInitializer(KingDoublePawnCheckmate), Side.White},
				new object[] {new PositionInitializer(KingDoublePawnCheckmate), Side.Black},
				new object[] {new PositionInitializer(BackRankCheckmate), Side.White},
				new object[] {new PositionInitializer(BackRankCheckmate), Side.Black},
				new object[] {new PositionInitializer(SmotheredCheckmate), Side.White},
				new object[] {new PositionInitializer(SmotheredCheckmate), Side.Black},
				new object[] {new PositionInitializer(KnightRookCheckmate), Side.White},
				new object[] {new PositionInitializer(KnightRookCheckmate), Side.Black},
				new object[] {new PositionInitializer(QueenBishopCheckmate), Side.White},
				new object[] {new PositionInitializer(QueenBishopCheckmate), Side.Black}
			};
		}
	}
}