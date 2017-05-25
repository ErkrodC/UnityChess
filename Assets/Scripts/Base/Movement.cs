using System;

namespace UnityChess
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