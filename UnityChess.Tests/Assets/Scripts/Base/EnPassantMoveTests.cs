using NUnit.Framework;
using UnityChess;

namespace UnityChess.EnPassantMoveTests
{
    [TestFixture()]
    [Author("Eric Rodriguez")]
    public class EnPassantMoveTests
    {
        Board board;

        [SetUp()]
        public void Init()
        {
            board = new Board();
            board.SetBlankBoard();
        }

        [Test()]
        public void HandleAssociatedPiece_EnPassantMove_AssocPawnIsRemoved()
        {
            Pawn pawn = new Pawn(new Square(1, 2), Side.White);
            board.PlacePiece(pawn);
            MockEnPassantMove mepm = new MockEnPassantMove(pawn);

            mepm.HandleAssociatedPiece(board);

            Assert.AreNotEqual(board.GetPiece(pawn.Position), pawn);
        }
    }

    public class MockEnPassantMove : EnPassantMove
    {
        public MockEnPassantMove(Pawn pawn) : base(new Square(7, 1), new Pawn(new Square(5, 1), Side.White), pawn)
        {
        }
    }
}