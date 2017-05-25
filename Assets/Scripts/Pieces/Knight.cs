using System;

namespace UnityChess
{
    public class Knight : Piece
    {
        public override void UpdateValidMoves(Board board)
        {
            ValidMoves.Clear();
        }

        public Knight(Square startingPosition, Side side) : base(startingPosition, side) { }
        public Knight(Knight knightCopy) : base(knightCopy) { }

        public override Piece Clone()
        {
            return new Knight(this);
        }
    }
}