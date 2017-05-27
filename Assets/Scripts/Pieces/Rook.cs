using System;
using System.Collections.Generic;

namespace UnityChess
{
    public class Rook : Piece
    {
        public Rook(Square startingPosition, Side side) : base(startingPosition, side) { }

        private Rook(Rook rookCopy) : base(rookCopy) { }

        public override void UpdateValidMoves(LinkedList<Board> boardList, Side turn)
        {
            ValidMoves.Clear();
        }

        public override Piece Clone()
        {
            return new Rook(this);
        }
    }
}