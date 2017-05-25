using System;

namespace UnityChess
{
    public class King : Piece
    {
        public override void UpdateValidMoves(Board board)
        {
            ValidMoves.Clear();
        }

        public King(Square startingPosition, Side side) : base(startingPosition, side) { }
        public King(King kingCopy) : base(kingCopy) { }

        public override Piece Clone()
        {
            return new King(this);
        }
    }
}