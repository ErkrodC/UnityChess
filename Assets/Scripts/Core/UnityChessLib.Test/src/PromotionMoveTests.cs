using NUnit.Framework;
using UnityChess.Util;

namespace UnityChess.Core.Test {
	[TestFixture]
	public class PromotionMoveTests {
		private Board board;

		[SetUp]
		public void Init() {
			board = new Board();
			board.ClearBoard();
		}

		[Test]
		[TestCase(ElectedPiece.Knight)]
		[TestCase(ElectedPiece.Bishop)]
		[TestCase(ElectedPiece.Rook)]
		[TestCase(ElectedPiece.Queen)]
		public void HandleAssociatedPiece_PromotionMove_ElectedPieceGenerated(ElectedPiece election) {
			Square expectedPosition = new Square(0, 7);
			PromotionMove mpm = new PromotionMove(Square.Invalid, expectedPosition);
			mpm.SetPromotionPiece(PromotionUtil.GeneratePromotionPiece(election, Side.White));

			mpm.HandleAssociatedPiece(board);

			Assert.AreEqual(election.ToString(), board[expectedPosition]?.GetType().Name);
		}
	}
}