using Moq;
using NUnit.Framework;

namespace UnityChess.Test {
	[TestFixture]
	public class BoardTests {
		private static Board board;
		private static Piece pawn;

		[SetUp]
		public void Init() {
			board = new Board();
			pawn = board[1, 2] as Pawn;
		}

		[Test]
		[TestCase(1, 3)]
		[TestCase(3, 5)]
		[TestCase(4, 8)]
		[TestCase(8, 2)]
		[TestCase(3, 4)]
		public void MovePiece_NormalMove_PieceIsMoved(int expectedFile, int expectedRank) {
			Square initialPosition = pawn.Position;
			Square expectedPosition = new Square(expectedFile, expectedRank);
			Movement move = new Movement(initialPosition, expectedPosition);
			
			board.MovePiece(move);

			Assert.Multiple(() => {
				Assert.AreEqual(expectedPosition, pawn.Position);
				Assert.AreEqual(board[expectedPosition], pawn);
				Assert.AreEqual(board[initialPosition], null);
				Assert.True(pawn.HasMoved);
			});
		}

		[Test]
		public void MovePiece_SpecialMove_HandleAssocPieceCalled() {
			Mock<MockSpecialMove> mockSpecialMove = new Mock<MockSpecialMove>();
			board.MovePiece(mockSpecialMove.Object);

			mockSpecialMove.Verify(specialMove => specialMove.HandleAssociatedPiece(board), Times.Exactly(1));
		}
	}

	public class MockSpecialMove : SpecialMove {
		public MockSpecialMove() : base(new Square(1, 2), new Square(1, 3), new Pawn(new Square(1, 2), Side.White)) { }

		public override void HandleAssociatedPiece(Board board) { }
	}
}