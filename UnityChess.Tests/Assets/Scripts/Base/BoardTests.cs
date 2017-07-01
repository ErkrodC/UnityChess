using NUnit.Framework;
using UnityChess;
using Moq;
using System;

namespace UnityChess.BoardTests
{
    [TestFixture()]
    [Author("Eric Rodriguez")]
    public class BoardTests
    {
        internal static Board board;
        internal static Piece pawn;

        [SetUp]
        public void Init()
        {
            board = new Board();
            pawn = new Pawn(new Square(1,2), Side.White);
        }

        [Test()]
        [TestCase(1, 3)]
        [TestCase(3, 5)]
        [TestCase(4, 8)]
        [TestCase(8, 2)]
        [TestCase(3, 4)]
        public void MovePiece_NormalMove_PieceIsMoved(int expectedFile, int expectedRank)
        {
            int initialPositionAsIndex = Square.RankFileAsIndex(pawn.Position.File, pawn.Position.Rank);
            int expectedPositionAsIndex = Square.RankFileAsIndex(expectedFile, expectedRank);
            Movement move = new Movement(new Square(expectedFile, expectedRank), pawn);

            board.MovePiece(move);

            Assert.Multiple(() => {
                Assert.AreEqual(board.BasePieceList[expectedPositionAsIndex], pawn);
                Assert.AreEqual(board.BasePieceList[initialPositionAsIndex], Board.EmptyPiece);
                Assert.AreEqual(expectedFile, pawn.Position.File);
                Assert.AreEqual(expectedRank, pawn.Position.Rank);
                Assert.True(pawn.HasMoved);
            });
        }

        [Test()]
        public void MovePiece_SpecialMove_HandleAssocPieceCalled()
        {
            MockSpecialMove mockSpecialMove = new MockSpecialMove();

            board.MovePiece(mockSpecialMove);
        }
    }

    public class MockSpecialMove : SpecialMove
    {
        public MockSpecialMove() : base(new Square(1,1), BoardTests.pawn, BoardTests.pawn)
        {
        }

        public override void HandleAssociatedPiece(Board board)
        {
            Assert.Pass("HandleAssociatedPiece called");
        }
    }
}