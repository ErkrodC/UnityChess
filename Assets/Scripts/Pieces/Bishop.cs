using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityChess.Base
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
