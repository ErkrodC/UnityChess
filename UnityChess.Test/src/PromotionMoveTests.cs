using NUnit.Framework;

namespace UnityChess.Test {
	[TestFixture]
	public class PromotionMoveTests {
		private Board board;

		[SetUp]
		public void Init() {
			board = new Board();
			board.SetBlankBoard();
		}

		[Test]
		[TestCase(ElectedPiece.Knight)]
		[TestCase(ElectedPiece.Bishop)]
		[TestCase(ElectedPiece.Rook)]
		[TestCase(ElectedPiece.Queen)]
		public void HandleAssociatedPiece_PromotionMove_ElectedPieceGenerated(ElectedPiece election) {
			Square expectedPosition = new Square(8, 1);
			MockPromotionMove mpm = new MockPromotionMove(expectedPosition, election);

			mpm.HandleAssociatedPiece(board);

			Assert.AreEqual($"UnityChess.{election.ToString()}", board[expectedPosition].GetType().ToString());
		}
	}

	public class MockPromotionMove : PromotionMove {
		public MockPromotionMove(Square end, ElectedPiece election) : base(new Square(7, 1), end, election, Side.White) { }
	}
}