using System;
using System.Collections.Generic;

namespace UnityChess
{
    public class Knight : Piece
    {
        public override void UpdateValidMoves(LinkedList<Board> boardList)
        {
            ValidMoves.Clear();
        }

        public Knight(Square startingPosition, Side side) : base(startingPosition, side) { }
        public Knight(Knight knightCopy) : base(knightCopy) { }

        public override Piece Clone()
        {
            return new Knight(this);
        }
    }
}