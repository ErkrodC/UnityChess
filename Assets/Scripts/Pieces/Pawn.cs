using System;
using System.Collections.Generic;

namespace UnityChess
{
    public class Pawn : Piece
    {
        public Pawn(Square startingPosition, Side side) : base(startingPosition, side) { }

        private Pawn(Pawn pawnCopy) : base(pawnCopy) { }

        public override void UpdateValidMoves(LinkedList<Board> boardList, Side turn)
        {
            ValidMoves.Clear();
        }

        public override Piece Clone()
        {
            return new Pawn(this);
        }
    }
}