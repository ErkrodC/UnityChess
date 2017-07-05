using System;
using System.Collections.Generic;

namespace UnityChess
{
    public class Bishop : Piece
    {
        public Bishop(Square startingPosition, Side side) : base(startingPosition, side) { }

        private Bishop(Bishop bishopCopy) : base(bishopCopy) { }

        public override void UpdateValidMoves(Board board, LinkedList<Movement> previousMoves, Side turn)
        {
            ValidMoves.Clear();

            CheckDiagonalDirections(board, turn);
        }

        private void CheckDiagonalDirections(Board board, Side turn)
        {
            Square testSquare = new Square(this.Position);
            Movement testMove = new Movement(testSquare, this);

            foreach (int i in new int[] { -1, 1 })
            {
                foreach (int j in new int[] { -1, 1 })
                {
                    testSquare.CopyPosition(this.Position);
                    testSquare.AddVector(i, j);

                    while (testSquare.IsValid())
                    {
                        if (testSquare.IsOccupied(board))
                        {
                            if (!testSquare.IsOccupiedByFriendly(board, this.Side) && CheckRules.ObeysCheckRules(board, testMove, turn))
                            {
                                ValidMoves.Add(new Movement(testMove));
                            }

                            break;
                        }
                        else if (CheckRules.ObeysCheckRules(board, testMove, turn))
                        {
                            ValidMoves.Add(new Movement(testMove));
                        }

                        testSquare.AddVector(i, j);
                    }
                }
            }
        }

        public override Piece Clone()
        {
            return new Bishop(this);
        }
    }
}