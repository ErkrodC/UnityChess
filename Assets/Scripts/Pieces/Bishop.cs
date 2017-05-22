using System;

namespace UnityChess
{
    public class Bishop : Piece
    {
        public override void UpdateValidMoves(Board board)
        {
            ValidMoves.Clear();
        }

        public Bishop(Square startingPosition, PieceType type) : base(startingPosition, type) { }
        public Bishop(Bishop bishopCopy) : base(bishopCopy) { }

        public override BasePiece Clone()
        {
            return new Bishop(this);
        }
    }
}
