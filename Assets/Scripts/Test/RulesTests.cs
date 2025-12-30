using NUnit.Framework;
using System.Collections.Generic;
using UnityChess.Core.Util;

namespace UnityChess.Core.Test {
	[TestFixture]
	public class RulesTests {
		//sets up a chess position to test
		public delegate Board PositionInitializer(Side side);

		[Test]
		[TestCaseSource(typeof(RulesTestData), nameof(RulesTestData.WithCheckCases))]
		[TestCaseSource(typeof(RulesTestData), nameof(RulesTestData.WithCheckmateCases))]
		public void IsPlayerInCheck_PositionsWithCheck_ReturnsTrue(PositionInitializer arrange, Side side) {
			Board board = arrange(side);

			bool actual = Rules.IsPlayerInCheck(board, side);

			Assert.AreEqual(true, actual, arrange.Method.Name);
			Assert.Pass(arrange.Method.Name);
		}

		[Test]
		[TestCaseSource(typeof(RulesTestData), nameof(RulesTestData.NoneCases))]
		[TestCaseSource(typeof(RulesTestData), nameof(RulesTestData.WithStalemateCases))]
		public void IsPlayerInCheck_PositionsWithoutCheck_ReturnsFalse(PositionInitializer arrange, Side side) {
			Board board = arrange(side);

			bool actual = Rules.IsPlayerInCheck(board, side);

			Assert.AreEqual(false, actual, arrange.Method.Name);
			Assert.Pass(arrange.Method.Name);
		}

		[Test]
		[TestCaseSource(typeof(RulesTestData), nameof(RulesTestData.WithStalemateCases))]
		public void IsPlayerStalemated_StalematedPosition_ReturnsTrue(PositionInitializer arrange, Side side) {
			Board board = arrange(side);

			int numLegalMoves
				= Game.GetNumLegalMoves(Game.CalculateLegalMovesForPosition(board, dummyConditionsBySide[side]));
			bool actual = Rules.IsPlayerStalemated(board, side, numLegalMoves);

			Assert.AreEqual(true, actual, arrange.Method.Name);
			Assert.Pass(arrange.Method.Name);
		}

		[Test]
		[TestCaseSource(typeof(RulesTestData), nameof(RulesTestData.NoneCases))]
		[TestCaseSource(typeof(RulesTestData), nameof(RulesTestData.WithCheckCases))]
		[TestCaseSource(typeof(RulesTestData), nameof(RulesTestData.WithCheckmateCases))]
		public void IsPlayerStalemated_NonStalematedPosition_ReturnsFalse(PositionInitializer arrange, Side side) {
			Board board = arrange(side);

			int numLegalMoves
				= Game.GetNumLegalMoves(Game.CalculateLegalMovesForPosition(board, dummyConditionsBySide[side]));
			bool actual = Rules.IsPlayerStalemated(board, side, numLegalMoves);

			Assert.AreEqual(false, actual, arrange.Method.Name);
			Assert.Pass(arrange.Method.Name);
		}

		[Test]
		[TestCaseSource(typeof(RulesTestData), nameof(RulesTestData.WithCheckmateCases))]
		public void IsPlayerCheckmated_CheckmatedPosition_ReturnsTrue(PositionInitializer arrange, Side side) {
			Board board = arrange(side);

			int numLegalMoves
				= Game.GetNumLegalMoves(Game.CalculateLegalMovesForPosition(board, dummyConditionsBySide[side]));
			bool actual = Rules.IsPlayerCheckmated(board, side, numLegalMoves);

			Assert.AreEqual(true, actual, arrange.Method.Name);
			Assert.Pass(arrange.Method.Name);
		}

		[Test]
		[TestCaseSource(typeof(RulesTestData), nameof(RulesTestData.NoneCases))]
		[TestCaseSource(typeof(RulesTestData), nameof(RulesTestData.WithCheckCases))]
		[TestCaseSource(typeof(RulesTestData), nameof(RulesTestData.WithStalemateCases))]
		public void IsPlayerCheckmated_NonCheckmatedPosition_ReturnsFalse(PositionInitializer arrange, Side side) {
			Board board = arrange(side);

			int numLegalMoves
				= Game.GetNumLegalMoves(Game.CalculateLegalMovesForPosition(board, dummyConditionsBySide[side]));
			bool actual = Rules.IsPlayerCheckmated(board, side, numLegalMoves);

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

		private static Dictionary<Side, GameConditions> dummyConditionsBySide = new Dictionary<Side, GameConditions> {
			[Side.White] = new GameConditions(
				sideToMove: Side.White,
				whiteCanCastleKingside: false,
				whiteCanCastleQueenside: false,
				blackCanCastleKingside: false,
				blackCanCastleQueenside: false,
				enPassantSquare: Square.Invalid,
				halfMoveClock: 0,
				turnNumber: 1
			),
			[Side.Black] = new GameConditions(
				sideToMove: Side.Black,
				whiteCanCastleKingside: false,
				whiteCanCastleQueenside: false,
				blackCanCastleKingside: false,
				blackCanCastleQueenside: false,
				enPassantSquare: Square.Invalid,
				halfMoveClock: 0,
				turnNumber: 1
			)
		};

		private static class RulesTestData {
			private static Board StartingPositionNone(Side side) {
				return new Board(Board.StartingPositionPieces);
			}

			private static Board BishopPinRookNone(Side side) {
				return new Board(
					(new Square(3, 3), new King(side)),
					(new Square(4, 4), new Rook(side)),
					(new Square(7, 0), new King(side.Complement())),
					(new Square(5, 5), new Bishop(side.Complement()))
				);
			}

			private static Board RookPinBishopNone(Side side) {
				return new Board(
					(new Square(3, 3), new King(side)),
					(new Square(3, 4), new Bishop(side)),
					(new Square(7, 0), new King(side.Complement())),
					(new Square(3, 5), new Rook(side.Complement()))
				);
			}

			private static Board NeutralKnightNone(Side side) {
				return new Board(
					(new Square(3, 3), new King(side)),
					(new Square(7, 0), new King(side.Complement())),
					(new Square(5, 3), new Knight(side.Complement()))
				);
			}

			private static Board NeutralPawnsNone(Side side) {
				return side == Side.White
					? new Board(
						(new Square(3, 3), new King(side)),
						(new Square(7, 0), new King(side.Complement())),
						(new Square(2, 2), new Pawn(side.Complement())),
						(new Square(4, 2), new Pawn(side.Complement()))
					)
					// ER TODO shouldn't this check?
					: new Board(
						(new Square(3, 3), new King(side)),
						(new Square(7, 0), new King(side.Complement())),
						(new Square(2, 4), new Pawn(side.Complement())),
						(new Square(4, 4), new Pawn(side.Complement()))
					);
			}

			private static PositionInitializer QueenCheck(Direction direction) {
				Square queenSquare;

				switch (direction) {
					case Direction.Kingside:
						queenSquare = new Square(7, 3);
						break;
					case Direction.BlackKingside:
						queenSquare = new Square(7, 7);
						break;
					case Direction.Black:
						queenSquare = new Square(3, 4);
						break;
					case Direction.BlackQueenside:
						queenSquare = new Square(2, 4);
						break;
					case Direction.Queenside:
						queenSquare = new Square(0, 3);
						break;
					case Direction.WhiteQueenside:
						queenSquare = new Square(0, 0);
						break;
					case Direction.White:
						queenSquare = new Square(3, 2);
						break;
					case Direction.WhiteKingside:
						queenSquare = new Square(4, 2);
						break;
					default:
						queenSquare = default;
						break;
				}

				return side => new Board(
					(new Square(3, 3), new King(side)),
					(new Square(7, 0), new King(side.Complement())),
					(queenSquare, new Queen(side.Complement()))
				);
			}

			private static PositionInitializer RookCheck(Direction direction) {
				Square rookSquare;

				switch (direction) {
					case Direction.Kingside:
						rookSquare = new Square(4, 3);
						break;
					case Direction.Black:
						rookSquare = new Square(3, 7);
						break;
					case Direction.Queenside:
						rookSquare = new Square(2, 3);
						break;
					case Direction.White:
						rookSquare = new Square(3, 0);
						break;
					default:
						rookSquare = default;
						break;
				}

				return side => new Board(
					(new Square(3, 3), new King(side)),
					(new Square(7, 0), new King(side.Complement())),
					(rookSquare, new Rook(side.Complement()))
				);
			}

			private static PositionInitializer BishopCheck(Direction direction) {
				Square bishopSquare;

				switch (direction) {
					case Direction.BlackKingside:
						bishopSquare = new Square(4, 4);
						break;
					case Direction.BlackQueenside:
						bishopSquare = new Square(0, 6);
						break;
					case Direction.WhiteQueenside:
						bishopSquare = new Square(2, 2);
						break;
					case Direction.WhiteKingside:
						bishopSquare = new Square(6, 0);
						break;
					default:
						bishopSquare = default;
						break;
				}

				return side => new Board(
					(new Square(3, 3), new King(side)),
					(new Square(7, 0), new King(side.Complement())),
					(bishopSquare, new Bishop(side.Complement()))
				);
			}

			private static PositionInitializer KnightCheck(KnightDirection direction) {
				Square knightSquare = direction switch {
					KnightDirection.KingBlackKingside => new Square(5, 4),
					KnightDirection.BlackBlackKingside => new Square(4, 5),
					KnightDirection.BlackBlackQueenside => new Square(2, 5),
					KnightDirection.QueenBlackQueenside => new Square(1, 4),
					KnightDirection.QueenWhiteQueenside => new Square(1, 2),
					KnightDirection.WhiteWhiteQueenside => new Square(2, 1),
					KnightDirection.WhiteWhiteKingside => new Square(4, 1),
					KnightDirection.KingWhiteKingside => new Square(5, 2),
					_ => default
				};

				return side => new Board(
					(new Square(3, 3), new King(side)),
					(new Square(7, 0), new King(side.Complement())),
					(knightSquare, new Knight(side.Complement()))
				);
			}

			private static PositionInitializer PawnCheck(Direction direction, Side checkedSide) {
				Square pawnSquare = checkedSide switch {
					Side.White => direction switch {
						Direction.Kingside => new Square(4, 4),
						Direction.Queenside => new Square(2, 4),
						_ => Square.Invalid
					},
					Side.Black => direction switch {
						Direction.Kingside => new Square(4, 2),
						Direction.Queenside => new Square(2, 2),
						_ => Square.Invalid
					},
					_ => Square.Invalid
				};

				return side => new Board(
					(new Square(3, 3), new King(side)),
					(new Square(7, 0), new King(side.Complement())),
					(pawnSquare, new Pawn(side.Complement()))
				);
			}

			private static Board KingPawnStalemate(Side side) {
				return side switch {
					Side.White => new Board(
						(new Square(5, 0), new King(side)),
						(new Square(5, 2), new King(side.Complement())),
						(new Square(5, 1), new Pawn(side.Complement()))
					),
					Side.Black => new Board(
						(new Square(5, 7), new King(side)),
						(new Square(5, 5), new King(side.Complement())),
						(new Square(5, 6), new Pawn(side.Complement()))
					),
					_ => default
				};
			}

			private static Board KingRookStalemate(Side side) {
				return new Board(
					(new Square(0, 0), new King(side)),
					(new Square(2, 2), new King(side.Complement())),
					(new Square(1, 1), new Rook(side.Complement()))
				);
			}

			private static Board KingBishopStalemate(Side side) {
				return new Board(
					(new Square(0, 7), new King(side)),
					(new Square(0, 5), new King(side.Complement())),
					(new Square(5, 3), new Bishop(side.Complement()))
				);
			}

			private static Board RookPinBishopStalemate(Side side) {
				return new Board(
					(new Square(0, 7), new King(side)),
					(new Square(1, 5), new King(side.Complement())),
					(new Square(7, 7), new Rook(side.Complement())),
					(new Square(1, 7), new Bishop(side))
				);
			}

			private static Board QueenStalemate(Side side) {
				return new Board(
					(new Square(0, 0), new King(side)),
					(new Square(7, 7), new King(side.Complement())),
					(new Square(1, 2), new Queen(side.Complement()))
				);
			}

			private static Board AnandVsKramnikStalemate(Side side) {
				return side switch {
					Side.White => new Board(
						(new Square(7, 4), new King(side)),
						(new Square(7, 3), new Pawn(side)),
						(new Square(5, 4), new King(side.Complement())),
						(new Square(5, 5), new Pawn(side.Complement())),
						(new Square(6, 6), new Pawn(side.Complement()))
					),
					Side.Black => new Board(
						(new Square(7, 3), new King(side)),
						(new Square(7, 4), new Pawn(side)),
						(new Square(5, 3), new King(side.Complement())),
						(new Square(5, 2), new Pawn(side.Complement())),
						(new Square(6, 1), new Pawn(side.Complement()))
					),
					_ => null
				};
			}

			private static Board KorchnoiVsKarpovStalemate(Side side) {
				return side switch {
					Side.White => new Board(
						(new Square(7, 1), new King(side)),
						(new Square(0, 4), new Pawn(side)),
						(new Square(5, 1), new King(side.Complement())),
						(new Square(6, 1), new Bishop(side.Complement())),
						(new Square(0, 5), new Pawn(side.Complement()))
					),
					Side.Black => new Board(
						(new Square(7, 6), new King(side)),
						(new Square(0, 3), new Pawn(side)),
						(new Square(5, 6), new King(side.Complement())),
						(new Square(6, 6), new Bishop(side.Complement())),
						(new Square(0, 2), new Pawn(side.Complement()))
					),
					_ => null
				};
			}

			private static Board BernsteinVsSmyslovStalemate(Side side) {
				return side switch {
					Side.White => new Board(
						(new Square(5, 2), new King(side)),
						(new Square(5, 4), new King(side.Complement())),
						(new Square(1, 1), new Rook(side.Complement())),
						(new Square(5, 3), new Pawn(side.Complement()))
					),
					Side.Black => new Board(
						(new Square(5, 5), new King(side)),
						(new Square(5, 3), new King(side.Complement())),
						(new Square(1, 6), new Rook(side.Complement())),
						(new Square(5, 4), new Pawn(side.Complement()))
					),
					_ => null
				};
			}

			private static Board GelfandVsKramnikStalemate(Side side) {
				return side switch {
					Side.White => new Board(
						(new Square(7, 1), new King(side)),
						(new Square(0, 2), new Pawn(side)),
						(new Square(5, 2), new Pawn(side)),
						(new Square(6, 1), new Pawn(side)),
						(new Square(7, 2), new Pawn(side)),
						(new Square(7, 6), new King(side.Complement())),
						(new Square(4, 1), new Rook(side.Complement())),
						(new Square(3, 0), new Queen(side.Complement())),
						(new Square(0, 3), new Pawn(side.Complement())),
						(new Square(3, 4), new Pawn(side.Complement())),
						(new Square(5, 3), new Pawn(side.Complement())),
						(new Square(5, 5), new Pawn(side.Complement())),
						(new Square(6, 4), new Pawn(side.Complement())),
						(new Square(7, 3), new Pawn(side.Complement()))
					),
					Side.Black => new Board(
						(new Square(7, 6), new King(side)),
						(new Square(0, 5), new Pawn(side)),
						(new Square(5, 5), new Pawn(side)),
						(new Square(6, 6), new Pawn(side)),
						(new Square(7, 5), new Pawn(side)),
						(new Square(7, 1), new King(side.Complement())),
						(new Square(4, 6), new Rook(side.Complement())),
						(new Square(3, 7), new Queen(side.Complement())),
						(new Square(0, 4), new Pawn(side.Complement())),
						(new Square(3, 3), new Pawn(side.Complement())),
						(new Square(5, 4), new Pawn(side.Complement())),
						(new Square(5, 2), new Pawn(side.Complement())),
						(new Square(6, 3), new Pawn(side.Complement())),
						(new Square(7, 4), new Pawn(side.Complement()))
					),
					_ => null
				};
			}

			private static Board DoubleRookCheckmate(Side side) {
				return new Board(
					(new Square(0, 0), new King(side)),
					(new Square(7, 7), new King(side.Complement())),
					(new Square(7, 0), new Rook(side.Complement())),
					(new Square(7, 1), new Rook(side.Complement()))
				);
			}

			private static Board KingQueenCheckmate(Side side) {
				return new Board(
					(new Square(7, 4), new King(side)),
					(new Square(5, 4), new King(side.Complement())),
					(new Square(6, 4), new Queen(side.Complement()))
				);
			}

			private static Board KingRookCheckmate(Side side) {
				return new Board(
					(new Square(7, 4), new King(side)),
					(new Square(5, 4), new King(side.Complement())),
					(new Square(7, 0), new Rook(side.Complement()))
				);
			}

			private static Board KingDoubleBishopCheckmate(Side side) {
				return new Board(
					(new Square(7, 7), new King(side)),
					(new Square(6, 5), new King(side.Complement())),
					(new Square(0, 1), new Bishop(side.Complement())),
					(new Square(1, 1), new Bishop(side.Complement()))
				);
			}

			private static Board KingBishopKnightCheckmate(Side side) {
				return new Board(
					(new Square(0, 7), new King(side)),
					(new Square(1, 5), new King(side.Complement())),
					(new Square(2, 5), new Bishop(side.Complement())),
					(new Square(0, 5), new Knight(side.Complement()))
				);
			}

			private static Board KingDoubleKnightCheckmate(Side side) {
				return new Board(
					(new Square(7, 7), new King(side)),
					(new Square(7, 5), new King(side.Complement())),
					(new Square(5, 5), new Knight(side.Complement())),
					(new Square(6, 5), new Knight(side.Complement()))
				);
			}

			private static Board KingDoublePawnCheckmate(Side side) {
				return side switch {
					Side.White => new Board(
						(new Square(4, 0), new King(side)),
						(new Square(4, 2), new King(side.Complement())),
						(new Square(4, 1), new Pawn(side.Complement())),
						(new Square(3, 1), new Pawn(side.Complement()))
					),
					Side.Black => new Board(
						(new Square(4, 7), new King(side)),
						(new Square(4, 5), new King(side.Complement())),
						(new Square(4, 6), new Pawn(side.Complement())),
						(new Square(3, 6), new Pawn(side.Complement()))
					),
					_ => null
				};
			}

			private static Board BackRankCheckmate(Side side) {
				return side switch {
					Side.White => new Board(
						(new Square(6, 0), new King(side)),
						(new Square(6, 7), new King(side.Complement())),
						(new Square(0, 0), new Rook(side.Complement())),
						(new Square(5, 1), new Pawn(side)),
						(new Square(6, 1), new Pawn(side)),
						(new Square(7, 1), new Pawn(side))
					),
					Side.Black => new Board(
						(new Square(6, 7), new King(side)),
						(new Square(6, 0), new King(side.Complement())),
						(new Square(0, 7), new Rook(side.Complement())),
						(new Square(5, 6), new Pawn(side)),
						(new Square(6, 6), new Pawn(side)),
						(new Square(7, 6), new Pawn(side))
					),
					_ => null
				};
			}

			private static Board SmotheredCheckmate(Side side) {
				return side switch {
					Side.White => new Board(
						(new Square(7, 0), new King(side)),
						(new Square(6, 6), new King(side.Complement())),
						(new Square(5, 1), new Knight(side.Complement())),
						(new Square(6, 1), new Pawn(side)),
						(new Square(7, 1), new Pawn(side)),
						(new Square(6, 0), new Rook(side))
					),
					Side.Black => new Board(
						(new Square(7, 7), new King(side)),
						(new Square(6, 1), new King(side.Complement())),
						(new Square(5, 6), new Knight(side.Complement())),
						(new Square(6, 6), new Pawn(side)),
						(new Square(7, 6), new Pawn(side)),
						(new Square(6, 7), new Rook(side))
					),
					_ => null
				};
			}

			private static Board KnightRookCheckmate(Side side) {
				return new Board(
					(new Square(7, 7), new King(side)),
					(new Square(6, 0), new King(side.Complement())),
					(new Square(5, 5), new Knight(side.Complement())),
					(new Square(7, 6), new Rook(side.Complement()))
				);
			}

			private static Board QueenBishopCheckmate(Side side) {
				return new Board(
					(new Square(6, 7), new King(side)),
					(new Square(6, 0), new King(side.Complement())),
					(new Square(6, 6), new Queen(side.Complement())),
					(new Square(7, 5), new Bishop(side.Complement()))
				);
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

			public static object[] WithCheckCases = {
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

			public static object[] WithStalemateCases = {
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

			public static object[] WithCheckmateCases = {
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