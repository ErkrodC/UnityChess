using System;

namespace UnityChess
{
    public class Pawn : Piece
    {
        public override void UpdateValidMoves(Board board)
        {
            ValidMoves.Clear();
        }

        public Pawn(Square startingPosition, PieceType type) : base(startingPosition, type) { }
        public Pawn(Pawn pawnCopy) : base(pawnCopy) { }

        public override BasePiece Clone()
        {
            return new Pawn(this);
        }
    }
}
