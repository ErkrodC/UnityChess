﻿using System;
using System.Collections.Generic;

namespace UnityChess
{
    public class Rook : Piece
    {
        private static int instanceCounter = 0;

        public Rook(Square startingPosition, Side side) : base(startingPosition, side)
        {
            this.ID = ++instanceCounter;
        }

        private Rook(Rook rookCopy) : base(rookCopy)
        {
            this.ID = rookCopy.ID;
        }

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
                            if (!testSquare.IsOccupiedBySide(board, this.Side) && Rules.MoveObeysRules(board, testMove, turn) && !testSquare.Equals(this.Side == Side.White ? board.BlackKing.Position : board.WhiteKing.Position))
                            {
                                ValidMoves.Add(new Movement(testMove));
                            }

                            break;
                        }
                        else if (Rules.MoveObeysRules(board, testMove, turn) && !testSquare.Equals(this.Side == Side.White ? board.BlackKing.Position : board.WhiteKing.Position))
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