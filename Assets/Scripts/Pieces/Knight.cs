using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityChess.Base
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
