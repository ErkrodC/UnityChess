using NUnit.Framework;
using UnityChess;

namespace UnityChess.CheckRulesTests
{
    [TestFixture()]
    [Author("Eric Rodriguez")]
    public class CheckRulesTests
    {
        Board board;

        [SetUp()]
        public void Init()
        {
            board = new Board();
            board.SetBlankBoard();
        }

        [Test()]
        public void IsPlayerCheckmated_PlayerIsCheckmated_TrueExpected()
        {
            bool expected = true;
            Rook whiteRook1 = new Rook(new Square(8, 8), Side.White);
            Rook whiteRook2 = new Rook(new Square(8, 7), Side.White);
            King blackKing = new King(new Square(1, 8), Side.Black);
            King whiteKing = new King(new Square(1, 1), Side.White);
            board.BasePieceList[whiteRook1.Position.AsIndex()] = whiteRook1;
            board.BasePieceList[whiteRook2.Position.AsIndex()] = whiteRook2;
            board.BasePieceList[blackKing.Position.AsIndex()] = blackKing;

            bool actual = CheckRules.IsPlayerCheckmated(board, Side.Black);

            Assert.AreEqual(expected, actual);
        }

        [Test()]
        public void IsPlayerStalemated_StateUnderTest_ExpectedBehavior()
        {
            // TODO write test after finishing all pieces' updateValidMoves methods
            Assert.Fail();
        }

        [Test()]
        public void IsPlayerInCheck_StateUnderTest_ExpectedBehavior()
        {
            Assert.Fail();
        }

        [Test()]
        public void DoesMoveRemoveCheck_StateUnderTest_ExpectedBehavior()
        {
            Assert.Fail();
        }

        [Test()]
        public void DoesMoveCauseCheck_StateUnderTest_ExpectedBehavior()
        {
            Assert.Fail();
        }
    }
}