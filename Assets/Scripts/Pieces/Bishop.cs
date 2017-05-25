using System;

namespace UnityChess
{
    public class Bishop : Piece
    {
        public override void UpdateValidMoves(Board board)
        {
            ValidMoves.Clear();
        }

        public Bishop(Square startingPosition, Side side) : base(startingPosition, side) { }
        public Bishop(Bishop bishopCopy) : base(bishopCopy) { }

        public override Piece Clone()
        {
            return new Bishop(this);
        }
    }
}