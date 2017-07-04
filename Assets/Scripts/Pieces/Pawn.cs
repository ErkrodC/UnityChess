using System;
using System.Collections.Generic;

namespace UnityChess
{
    public class Pawn : Piece
    {
        public Pawn(Square startingPosition, Side side) : base(startingPosition, side) { }

        private Pawn(Pawn pawnCopy) : base(pawnCopy) { }

        public override void UpdateValidMoves(LinkedList<Board> boardList, Side turn)
        {
            ValidMoves.Clear();

            Board board = boardList.Last.Value;

            CheckForwardMovingSquares(board, turn);
            CheckAttackingSquares(board, turn);
            CheckEnPassantCaptures(boardList, turn);
        }

        private void CheckForwardMovingSquares(Board board, Side turn)
        {
            Square testSquare = new Square(this.Position);
            Movement testMove = new Movement(testSquare, this);

            testSquare.AddVector(0, this.Side == Side.White ? 1 : -1);
            if (!testSquare.IsOccupied(board) && CheckRules.ObeysCheckRules(board, testMove, turn))
            {
                ValidMoves.Add(new Movement(testMove));

                if (!this.HasMoved)
                {
                    testSquare.AddVector(0, this.Side == Side.White ? 1 : -1);
                    if (!testSquare.IsOccupied(board) && CheckRules.ObeysCheckRules(board, testMove, turn))
                    {
                        ValidMoves.Add(new Movement(testMove));
                    }
                }
            }
        }

        private void CheckAttackingSquares(Board board, Side turn)
        {

        }

        private void CheckEnPassantCaptures(LinkedList<Board> boardList, Side turn)
        {

        }

        public override Piece Clone()
        {
            return new Pawn(this);
        }
    }
}