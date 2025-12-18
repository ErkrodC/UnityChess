using NUnit.Framework;
using Moq;
using UnityChess.Util;

namespace UnityChess.Core.Test {
	[TestFixture]
	public class GameTests {
		[Test]
		[TestCase(0)]
		[TestCase(1)]
		[TestCase(10)]
		[TestCase(40)]
		[TestCase(64)]
		public void CalculateAllPiecesLegalMoves_PiecesOnBoard_CalculateLegalMovesCalled(int numberOfPieces) {
			Mock<Piece> mockPiece = new Mock<Piece>(MockBehavior.Loose, Side.White);
			Board board = CreateBoard(numberOfPieces, mockPiece);

			Game.CalculateLegalMovesForPosition(board, GameConditions.NormalStartingConditions);

			mockPiece.Verify(
				piece => piece.CalculateLegalMoves(
					board,
					GameConditions.NormalStartingConditions,
					It.IsAny<Square>()
				),
				Times.Exactly(numberOfPieces)
			);
		}

		private static Board CreateBoard(int numberOfPieces, Mock<Piece> mockPiece) {
			Board result = new Board();

			for (int i = 0; i < numberOfPieces; i++) {
				int file = i / 8;
				int rank = i % 8;
				result[file, rank] = mockPiece.Object;
			}

			return result;
		}
	}
}