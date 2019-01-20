using NUnit.Framework;
using Moq;
using System.Collections.Generic;

namespace UnityChess.Test {
	[TestFixture]
	public class GameTests {
		private Board board;
		private LinkedList<Turn> dummyPreviousMoves;

		[SetUp]
		public void Init() {
			board = new Board();
			board.SetBlankBoard();

			dummyPreviousMoves = new LinkedList<Turn>();
		}

		[Test, TestCase(0), TestCase(1), TestCase(10), TestCase(40), TestCase(64)]
		public void UpdateAllPiecesValidMoves_PiecesOnBoard_UpdateValidMovesCalled(int numberOfPieces) {
			Mock<MockPiece> mockPiece = new Mock<MockPiece>();
			PopulateBoard(numberOfPieces, mockPiece);

			Game.UpdateAllPiecesValidMoves(board, dummyPreviousMoves, Side.White);

			mockPiece.Verify(piece => piece.UpdateValidMoves(board, dummyPreviousMoves), Times.Exactly(numberOfPieces));
		}

		private void PopulateBoard(int numberOfPieces, IMock<MockPiece> mockPiece) {
			for (int i = 0; i < numberOfPieces; i++) {
				int file = i / 8 + 1;
				int rank = i % 8 + 1; 
				board[file, rank] = mockPiece.Object;
			}
		}
	}

	public abstract class MockPiece : Piece {
		protected MockPiece() : base(new Square(21), Side.White) { }

		public override Piece Clone() => this;

		public override void UpdateValidMoves(Board board, LinkedList<Turn> previousMoves) { }
	}
}