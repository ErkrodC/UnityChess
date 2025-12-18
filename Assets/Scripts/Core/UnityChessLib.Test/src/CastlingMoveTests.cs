using NUnit.Framework;
using UnityChess.Util;

namespace UnityChess.Core.Test {
	[TestFixture]
	public class CastlingMoveTests {
		private Board board;

		[SetUp]
		public void Init() {
			board = new Board();
		}

		[Test]
		[TestCase(5, 7)] //Kingside castle
		[TestCase(3, 0)] //Queenside castle
		public void HandleAssociatedPiece_CastlingMove_RookMovedAsExpected(int expected, int rookStartingFile) {
			Square rookStartSquare = new Square(rookStartingFile, 0);
			Rook rook = new Rook(Side.White);
			board[rookStartSquare] = rook;
			CastlingMove castlingMove = new CastlingMove(new Square(4, 0), new Square(6, 0), rookStartSquare);

			castlingMove.HandleAssociatedPiece(board);

			Assert.AreEqual(rook, board[castlingMove.GetRookEndSquare()]);
		}
	}
}