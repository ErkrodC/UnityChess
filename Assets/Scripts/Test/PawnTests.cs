using System.Collections.Generic;
using NUnit.Framework;

namespace UnityChess.Core.Test {
	[TestFixture]
	public class PawnTests {
		[Test]
		public void LegalMoveGeneration_BlockedPawn_PawnHasNoLegalMoves() {
			Game game = new FENSerializer().Deserialize(
				"rnbqk1nr/pppp1ppp/8/4p3/1b1P4/2N5/PPP1PPPP/R1BQKBNR w KQkq - 2 3"
			);

			Board board = game.BoardTimeline.Head;
			ICollection<Movement> legalMovesForBlockedPawn = game.GetLegalMovesForPiece(board[new Square("c2")]);

			Assert.AreEqual(0, legalMovesForBlockedPawn?.Count ?? 0);
		}
	}
}