using System;
using System.Collections.Generic;

namespace UnityChess
{
    public class King : Piece
    {
        public override void UpdateValidMoves(LinkedList<Board> boardList)
        {
            ValidMoves.Clear();
        }

        public King(Square startingPosition, Side side) : base(startingPosition, side) { }
        public King(King kingCopy) : base(kingCopy) { }

        public override Piece Clone()
        {
            return new King(this);
        }
    }
}