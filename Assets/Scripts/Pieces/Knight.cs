using System;

namespace UnityChess
{
    public class Knight : Piece
    {
        public override void UpdateValidMoves(Board board)
        {
            ValidMoves.Clear();
        }

        public Knight(Square startingPosition, PieceType type) : base(startingPosition, type) { }
        public Knight(Knight knightCopy) : base(knightCopy) { }

        public override BasePiece Clone()
        {
            return new Knight(this);
        }
    }
}
