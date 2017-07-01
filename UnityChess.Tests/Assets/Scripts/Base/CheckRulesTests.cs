using NUnit.Framework;
using UnityChess;

namespace UnityChess.CheckRulesTests
{
    [TestFixture()]
    [Author("Eric Rodriguez")]
    public class CheckRulesTests
    {
        Bishop bishop;
        King king;
        Knight knight;
        Pawn pawn;
        Queen queen;
        Rook rook;

        [Test()]
        public void ObeysCheckRules_StateUnderTest_ExpectedBehavior()
        {
            Assert.Fail();
        }

        [Test()]
        public void IsPlayerCheckmated_StateUnderTest_ExpectedBehavior()
        {
            Assert.Fail();
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