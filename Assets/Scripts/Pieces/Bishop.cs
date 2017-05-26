using System;
using System.Collections.Generic;

namespace UnityChess
{
    public class Bishop : Piece
    {
        public override void UpdateValidMoves(LinkedList<Board> boardList)
        {
            ValidMoves.Clear();

            //search for open/valid/legal squares along white-queenside direction starting from current position.
            //Stop when reached a occupied/invalid square, or square in which a move to that square would be illegal.
            Board board = boardList.Last.Value;
            Square testSquare = new Square(this.Position);
            testSquare.AddVector(-1, -1);
            Movement testMove = new Movement(testSquare, this);

            while(testSquare.IsValid() && !testSquare.IsOccupied(board) && Rules.DoesMoveCauseCheck(board, testMove, this.Side) && !Rules.DoesMoveCauseCheck(board, testMove, this.Side))
            {
                //creates a snapshot of the move and adds to ValidMoves. does not assign testSquare or testMove to any members of newly created Movement (and subsequent Square)
                ValidMoves.Add(new Movement(testMove));
                testSquare.AddVector(-1, -1);
            }

            //white-kingside direction
            testSquare.CopyPosition(this.Position);
            testSquare.AddVector(1, -1);
            while (testSquare.IsValid() && !testSquare.IsOccupied(board) && Rules.DoesMoveCauseCheck(board, testMove, this.Side) && !Rules.DoesMoveCauseCheck(board, testMove, this.Side))
            {
                ValidMoves.Add(new Movement(testMove));
                testSquare.AddVector(1, -1);
            }

            //black-kingside direction
            testSquare.CopyPosition(this.Position);
            testSquare.AddVector(1, 1);
            while (testSquare.IsValid() && !testSquare.IsOccupied(board) && Rules.DoesMoveCauseCheck(board, testMove, this.Side) && !Rules.DoesMoveCauseCheck(board, testMove, this.Side))
            {
                ValidMoves.Add(new Movement(testMove));
                testSquare.AddVector(1, 1);
            }

            //black-queenside direction
            testSquare.CopyPosition(this.Position);
            testSquare.AddVector(-1, 1);
            while (testSquare.IsValid() && !testSquare.IsOccupied(board) && Rules.DoesMoveCauseCheck(board, testMove, this.Side) && !Rules.DoesMoveCauseCheck(board, testMove, this.Side))
            {
                ValidMoves.Add(new Movement(testMove));
                testSquare.AddVector(-1, 1);
            }
        }

        public Bishop(Square startingPosition, Side side) : base(startingPosition, side) { }
        public Bishop(Bishop bishopCopy) : base(bishopCopy) { }

        public override Piece Clone()
        {
            return new Bishop(this);
        }
    }
}