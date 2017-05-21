using System;

namespace UnityChess
{
    public class Queen : Piece
    {
        public override void GetValidMoves(Board board)
        {
            throw new NotImplementedException();
        }

        public Queen(Square startingPosition, PieceType type) : base(startingPosition, type) { }
    }
}
