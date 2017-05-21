using System;

namespace UnityChess
{
    public class Bishop : Piece
    {
        public override void GetValidMoves(Board board)
        {
            throw new NotImplementedException();
        }

        public Bishop(Square startingPosition, PieceType type) : base(startingPosition, type) { }
    }
}
