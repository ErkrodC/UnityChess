using System;
using System.Collections.Generic;

namespace UnityChess
{
    public class Knight : Piece
    {
        public Knight(Square startingPosition, Side side) : base(startingPosition, side) { }

        private Knight(Knight knightCopy) : base(knightCopy) { }

        public override void UpdateValidMoves(Board board, LinkedList<Movement> previousMoves, Side turn)
        {
            ValidMoves.Clear();

            CheckKnightCircleSquares(board, turn);
        }

        private void CheckKnightCircleSquares(Board board, Side turn)
        {
            Square testSquare = new Square(this.Position);
            Movement testMove = new Movement(testSquare, this);

            for (int i = -2; i <= 2; i++)
            {
                if (i == 0) { continue; }
                foreach (int j in (Math.Abs(i) == 2 ? new int[] {-1, 1 } : new int[] {-2, 2 } ))
                {
                    testSquare.CopyPosition(this.Position);
                    testSquare.AddVector(i, j);

                    if (testSquare.IsValid() && !testSquare.IsOccupiedBySide(board, this.Side) && Rules.MoveObeysRules(board, testMove, turn) && !testSquare.Equals(this.Side == Side.White ? board.BlackKing.Position : board.WhiteKing.Position))
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