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

            CheckKnightCircleSquares(board, turn);
        }

        private void CheckKnightCircleSquares(Board board, Side turn)
        {
            Square testSqaure = new Square(this.Position);
            Movement testMove = new Movement(testSqaure, this);

            for (int i = -2; i <= 2; i++)
            {
                if (i == 0) { continue; }
                foreach (int j in (Math.Abs(i) == 2 ? new int[] {-1, 1 } : new int[] {-2, 2 } ))
                {
                    testSqaure.CopyPosition(this.Position);
                    testSqaure.AddVector(i, j);

                    if (testSqaure.IsValid() && !testSqaure.IsOccupiedByFriendly(board, turn) && CheckRules.ObeysCheckRules(board, testMove, turn))
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