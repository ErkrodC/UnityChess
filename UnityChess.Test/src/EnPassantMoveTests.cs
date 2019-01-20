using NUnit.Framework;

namespace UnityChess.Test {
	[TestFixture]
	public class EnPassantMoveTests {
		private Board board;

		[SetUp]
		public void Init() {
			board = new Board();
			board.SetBlankBoard();
		}

		[Test]
		public void HandleAssociatedPiece_EnPassantMove_AssocPawnIsRemoved() {
			Square pawnPosition = new Square(1, 2);
			Pawn pawn = new Pawn(pawnPosition, Side.White);
			board[pawnPosition] = pawn;
			MockEnPassantMove mepm = new MockEnPassantMove(pawn);

			mepm.HandleAssociatedPiece(board);

			Assert.AreNotEqual(board[pawn.Position], pawn);
		}
	}

	public class MockEnPassantMove : EnPassantMove {
		public MockEnPassantMove(Pawn pawn) : base(new Square(5, 1), new Square(7, 1), pawn) { }
	}
}