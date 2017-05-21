using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityChess.Base
{
    public class Movement
    {
        public Square End { get; set; }
        public Piece Piece { get; set; }

        public Movement(Square end, Piece piece)
        {
            this.End = end;
            this.Piece = piece;
        }
    }
}
