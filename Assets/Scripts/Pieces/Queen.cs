using System;

namespace UnityChess
{
    public class Queen : Piece
    {
        public override void UpdateValidMoves(Board board)
        {
            ValidMoves.Clear();
        }

        public Queen(Square startingPosition, PieceType type) : base(startingPosition, type) { }
        public Queen(Queen queenCopy) : base(queenCopy) { }

        public override BasePiece Clone()
        {
            return new Queen(this);
        }
    }
}
