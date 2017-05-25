using System;
using System.Collections.Generic;

namespace UnityChess
{
    public class Queen : Piece
    {
        public override void UpdateValidMoves(LinkedList<Board> boardList)
        {
            ValidMoves.Clear();
        }

        public Queen(Square startingPosition, Side side) : base(startingPosition, side) { }
        public Queen(Queen queenCopy) : base(queenCopy) { }

        public override Piece Clone()
        {
            return new Queen(this);
        }
    }
}