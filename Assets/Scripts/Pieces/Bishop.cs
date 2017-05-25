using System;
using System.Collections.Generic;

namespace UnityChess
{
    public class Bishop : Piece
    {
        public override void UpdateValidMoves(LinkedList<Board> boardList)
        {
            Board board = boardList.Last.Value;
            ValidMoves.Clear();

            Movement testMove = new Movement(new Square(this.Position), this);
            Square square = testMove.End;
            square.Rank -= 1;
            square.File -= 1;
            while(square.IsValid() && !square.IsOccupied(board) && Rules.DoesMoveCauseCheck(board, testMove, this.Side) && !Rules.DoesMoveCauseCheck(board, testMove, this.Side))
            {
                ValidMoves.Add(new Movement(testMove));
                square.Rank -= 1;
                square.File -= 1;
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