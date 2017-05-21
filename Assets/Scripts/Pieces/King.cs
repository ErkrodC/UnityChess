using System;

namespace UnityChess
{
    public class King : Piece
    {
        public override void GetValidMoves(Board board)
        {
            throw new NotImplementedException();
        }

        public King(Square startingPosition, PieceType type) : base(startingPosition, type) { }
    }
}
