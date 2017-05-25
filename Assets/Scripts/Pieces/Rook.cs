using System;
using System.Collections.Generic;

namespace UnityChess
{
    public class Rook : Piece
    {
        public override void UpdateValidMoves(LinkedList<Board> boardList)
        {
            ValidMoves.Clear();
        }

        public Rook(Square startingPosition, Side side) : base(startingPosition, side) { }
        public Rook(Rook rookCopy) : base(rookCopy) { }

        public override Piece Clone()
        {
            return new Rook(this);
        }
    }
}