using System;
using System.Collections.Generic;

namespace UnityChess
{
    public class Knight : Piece
    {
        public Knight(Square startingPosition, Side side) : base(startingPosition, side) { }

        private Knight(Knight knightCopy) : base(knightCopy) { }

        public override void UpdateValidMoves(LinkedList<Board> boardList, Side turn)
        {
            ValidMoves.Clear();

            Board board = boardList.Last.Value;

            for (int file = -2; file <= 2; file++)
            {
                if (file == 0) { continue; }

                int complement = Math.Abs(file) == 1 ? 2 : 1;
                for (int rank = -1 * complement; rank <= complement; rank += 2 * complement)
                {
                    Square testSqaure = new Square(file, rank);
                    Movement testMove = new Movement(testSqaure, this);

                    if (testSqaure.IsValid() && !testSqaure.IsOccupied(board) && !Rules.DoesMoveCauseCheck(board, testMove, turn) && Rules.DoesMoveRemoveCheck(boardList, testMove, turn))
                    {
                        ValidMoves.Add(new Movement(testMove));
                    }
                }
            }
        }

        public override Piece Clone()
        {
            return new Knight(this);
        }
    }
}