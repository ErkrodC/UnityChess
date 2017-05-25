using System;

namespace UnityChess
{
    public class Pawn : Piece
    {
        public override void UpdateValidMoves(Board board)
        {
            ValidMoves.Clear();
        }

        public Pawn(Square startingPosition, Side side) : base(startingPosition, side) { }
        public Pawn(Pawn pawnCopy) : base(pawnCopy) { }

        public override Piece Clone()
        {
            return new Pawn(this);
        }
    }
}