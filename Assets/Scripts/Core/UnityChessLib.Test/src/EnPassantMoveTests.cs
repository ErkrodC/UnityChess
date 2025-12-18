using NUnit.Framework;
using UnityChess.Util;

namespace UnityChess.Core.Test {
	[TestFixture]
	public class EnPassantMoveTests {
		private Board board;

		[SetUp]
		public void Init() {
			board = new Board();
			board.ClearBoard();
		}

		[Test]
		public void HandleAssociatedPiece_EnPassantMove_AssocPawnIsRemoved() {
			Square capturedPawnSquare = new Square(0, 1);
			board[capturedPawnSquare] = new Pawn(Side.White);
			EnPassantMove enPassantMove = new EnPassantMove(Square.Invalid, Square.Invalid, capturedPawnSquare);

			enPassantMove.HandleAssociatedPiece(board);

			Assert.AreEqual(null, board[capturedPawnSquare]);
		}
	}
}