﻿using NUnit.Framework;
using UnityChess;
using System;

namespace UnityChess.CastlingMoveTests
{
    [TestFixture()]
    [Author("Eric Rodriguez")]
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
        public void HandleAssociatedPiece_CastlingMove_RookMovedAsExpected(int expected, int rookStartingFile)
        {
            Rook rook = new Rook(new Square(rookStartingFile, 1), Side.White);
            board.PlacePiece(rook);
            MockCastlingMove mcm = new MockCastlingMove(rook);

            mcm.HandleAssociatedPiece(board);

            Assert.Multiple(() =>
            {
                Assert.True(rook.HasMoved);
                Assert.AreEqual(expected, rook.Position.File);
            });
        }
    }

    public class MockCastlingMove : CastlingMove
    {
        public MockCastlingMove(Rook rook) : base(new Square(7,1), new King(new Square(5,1), Side.White), rook)
        {
        }
    }
}