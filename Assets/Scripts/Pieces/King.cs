using System;

namespace UnityChess
{
    public class King : Piece
    {
        public override void UpdateValidMoves(Board board)
        {
            ValidMoves.Clear();
        }

        public King(Square startingPosition) : base(startingPosition, type) { }
        public King(King kingCopy) : base(kingCopy) { }

        public override BasePiece Clone()
        {
            return new King(this);
        }
    }
}
