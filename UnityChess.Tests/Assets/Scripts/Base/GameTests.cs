using NUnit.Framework;
using UnityChess;
using Moq;
using System.Collections.Generic;
using System;

namespace UnityChess.GameTests
{
    [TestFixture()]
    [Author("Eric Rodriguez")]
    public class GameTests
    {
        Board board;
        LinkedList<Board> dummyBList;

        [SetUp()]
        public void Init()
        {
            board = new Board();
            board.SetBlankBoard();

            dummyBList = new LinkedList<Board>();

            dummyBList.AddLast(board);
        }

        [Test()]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(10)]
        [TestCase(40)]
        [TestCase(64)]
        public void UpdateAllPiecesValidMoves_PiecesOnBoard_UpdateValidMovesCalled(int numberOfPieces)
        {
            Mock<MockPiece> mockPiece = new Mock<MockPiece>();
            PopulateBoard(board, numberOfPieces, mockPiece);

            Game.UpdateAllPiecesValidMoves(dummyBList, Side.White);

            mockPiece.Verify(piece => piece.UpdateValidMoves(dummyBList, Side.White), Times.Exactly(numberOfPieces));
        }

        public void PopulateBoard(Board board, int numberOfPieces, Mock<MockPiece> mockPiece)
        {
            numberOfPieces = numberOfPieces > 98 ? 98 : numberOfPieces;

            for (int i = 0; i < numberOfPieces; i++)
            {
                board.BasePieceList[i + 21] = mockPiece.Object;
            }
        }
    }

    public class MockPiece : Piece
    {
        public MockPiece() : base(new Square(21), Side.White) { }

        public override Piece Clone()
        {
            return this;
        }

        public override void UpdateValidMoves(LinkedList<Board> boardList, Side turn)
        {
            return;
        }
    }
}