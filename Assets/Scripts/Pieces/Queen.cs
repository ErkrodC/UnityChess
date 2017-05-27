using System;
using System.Collections.Generic;

namespace UnityChess
{
    public class Queen : Piece
    {
        public Queen(Square startingPosition, Side side) : base(startingPosition, side) { }

        private Queen(Queen queenCopy) : base(queenCopy) { }

        public override void UpdateValidMoves(LinkedList<Board> boardList, Side turn)
        {
            ValidMoves.Clear();
        }

        public override Piece Clone()
        {
            return new Queen(this);
        }
    }
}