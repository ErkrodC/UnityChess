using NUnit.Framework;
using System;
using UnityChess;

namespace UnityChess.PromotionMoveTests
{
    [TestFixture()]
    [Author("Eric Rodriguez")]
    public class PromotionMoveTests
    {
        Board board;

        [SetUp()]
        public void Init()
        {
            board = new Board();
            board.SetBlankBoard();
        }

        [Test()]
        [TestCase(ElectedPiece.Bishop)]
        [TestCase(ElectedPiece.Knight)]
        [TestCase(ElectedPiece.Queen)]
        [TestCase(ElectedPiece.Rook)]
        public void HandleAssociatedPiece_PromotionMove_ElectedPieceGenerated(ElectedPiece election)
        {
            Square expectedPosition = new Square(8, 1);
            MockPromotionMove mpm = new MockPromotionMove(expectedPosition, election);

            mpm.HandleAssociatedPiece(board);

            Assert.AreEqual($"UnityChess.{ election.ToString() }", board.BoardPosition[expectedPosition.AsIndex()].GetType().ToString());
        }
    }

    public class MockPromotionMove : PromotionMove
    {
        private static Pawn dummyPawn = new Pawn(new Square(7, 1), Side.White);

        public MockPromotionMove(Square end, ElectedPiece election) : base(end, dummyPawn, election) { }
    }
}