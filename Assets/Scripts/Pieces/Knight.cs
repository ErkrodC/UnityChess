using System;

namespace UnityChess
{
    public class Knight : Piece
    {
        public override void GetValidMoves(Board board)
        {
            throw new NotImplementedException();
        }

        public Knight(Square startingPosition, PieceType type) : base(startingPosition, type) { }
    }
}
