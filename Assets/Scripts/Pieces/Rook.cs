using System;

namespace UnityChess
{
    public class Rook : Piece
    {
        public override void UpdateValidMoves(Board board)
        {
            ValidMoves.Clear();
        }

        public Rook(Square startingPosition, PieceType type) : base(startingPosition, type) { }
        public Rook(Rook rookCopy) : base(rookCopy) { }

        public override BasePiece Clone()
        {
            return new Rook(this);
        }
    }
}
