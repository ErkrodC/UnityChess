using System;

namespace UnityChess
{
    public class Queen : Piece
    {
        public override void UpdateValidMoves(Board board)
        {
            ValidMoves.Clear();
        }

        public Queen(Square startingPosition, Side side) : base(startingPosition, side) { }
        public Queen(Queen queenCopy) : base(queenCopy) { }

        public override Piece Clone()
        {
            return new Queen(this);
        }
    }
}