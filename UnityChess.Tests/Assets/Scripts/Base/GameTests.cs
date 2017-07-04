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
        LinkedList<Movement> dummyPreviousMoves;

        [SetUp()]
        public void Init()
        {
            board = new Board();
            board.SetBlankBoard();

            dummyPreviousMoves = new LinkedList<Movement>();
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

            Game.UpdateAllPiecesValidMoves(board, dummyPreviousMoves, Side.White);

            mockPiece.Verify(piece => piece.UpdateValidMoves(board, dummyPreviousMoves, Side.White), Times.Exactly(numberOfPieces));
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

        public override void UpdateValidMoves(Board board, LinkedList<Movement> previousMoves, Side turn)
        {
            return;
        }
    }
}