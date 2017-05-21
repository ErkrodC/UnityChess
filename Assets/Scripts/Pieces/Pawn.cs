using System;

namespace UnityChess
{
    public class Pawn : Piece
    {
        public override void GetValidMoves(Board board)
        {
            throw new NotImplementedException();
        }

        public Pawn(Square startingPosition, PieceType type) : base(startingPosition, type) { }
    }
}
