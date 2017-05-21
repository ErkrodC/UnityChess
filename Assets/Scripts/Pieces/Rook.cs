using System;

namespace UnityChess
{
    public class Rook : Piece
    {
        public override void GetValidMoves(Board board)
        {
            throw new NotImplementedException();
        }

        public Rook(Square startingPosition, PieceType type) : base(startingPosition, type) { }
    }
}
