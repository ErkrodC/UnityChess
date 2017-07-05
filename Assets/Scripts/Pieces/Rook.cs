using System;
using System.Collections.Generic;

namespace UnityChess
{
    public class Rook : Piece
    {
        public Rook(Square startingPosition, Side side) : base(startingPosition, side) { }

        private Rook(Rook rookCopy) : base(rookCopy) { }

        public override void UpdateValidMoves(Board board, LinkedList<Movement> previousMoves, Side turn)
        {
            ValidMoves.Clear();

            CheckCardinalDirections(board, turn);
        }

        private void CheckCardinalDirections(Board board, Side turn)
        {
            Square testSquare = new Square(this.Position);
            Movement testMove = new Movement(testSquare, this);

            foreach (int i in new int[] { -1, 0, 1 })
            {
                foreach (int j in (Math.Abs(i) == 1 ? new int[] { 0 } : new int[] { -1, 1 }))
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
            return new Rook(this);
        }
    }
}