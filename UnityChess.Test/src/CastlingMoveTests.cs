using NUnit.Framework;

namespace UnityChess.Test {
	[TestFixture]
	public class CastlingMoveTests {
		private Board board;

		[SetUp]
		public void Init() {
			board = new Board();
			board.SetBlankBoard();
		}

		[Test]
		[TestCase(6, 8)] //Kingside castle
		[TestCase(4, 1)] //Queenside castle
		public void HandleAssociatedPiece_CastlingMove_RookMovedAsExpected(int expected, int rookStartingFile) {
			Square startingSquare = new Square(rookStartingFile, 1);
			Rook rook = new Rook(startingSquare, Side.White);
			board[startingSquare] = rook;
			MockCastlingMove mcm = new MockCastlingMove(rook);

			mcm.HandleAssociatedPiece(board);

			Assert.Multiple(() => {
				Assert.True(rook.HasMoved);
				Assert.AreEqual(expected, rook.Position.File);
			});
		}
	}

	public class MockCastlingMove : CastlingMove {
		public MockCastlingMove(Rook rook) : base(new Square(5, 1), new Square(7, 1), rook) { }
	}
}