using System;

namespace UnityChess
{
    public class Rook : Piece
    {
        public override void UpdateValidMoves(Board board)
        {
            ValidMoves.Clear();
        }

        public Rook(Square startingPosition, Side side) : base(startingPosition, side) { }
        public Rook(Rook rookCopy) : base(rookCopy) { }

        public override Piece Clone()
        {
            return new Rook(this);
        }
    }
}