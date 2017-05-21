using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityChess.Base
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
