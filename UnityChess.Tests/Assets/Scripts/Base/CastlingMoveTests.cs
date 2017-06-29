using NUnit.Framework;
using UnityChess;
using System;

namespace UnityChess.CastlingMoveTests
{
    [TestFixture()]
    public class CastlingMoveTests
    {
        Board board;

        [SetUp()]
        public void Init()
        {
            board = new Board();
            board.SetBlankBoard();
        }

        [Test()]
        [TestCase(6, 8)] //Kingside castle
        [TestCase(4, 1)] //Queenside castle
        public void HandleAssociatedPiece_CastlingMove_RookFileIsExpected(int expected, int rookStartingFile)
        {
            Rook rook = new Rook(new Square(rookStartingFile, 1), Side.White);
            board.BoardPosition[rook.Position.AsIndex()] = rook;
            MockCastlingMove mcm = new MockCastlingMove(rook);

            mcm.HandleAssociatedPiece(board);

            Assert.AreEqual(expected, rook.Position.File);
        }
    }

    public class MockCastlingMove : CastlingMove
    {
        public MockCastlingMove(Rook rook) : base(new Square(7,1), new King(new Square(5,1), Side.White), rook)
        {
        }
    }
}