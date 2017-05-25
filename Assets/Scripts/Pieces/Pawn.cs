using System;
using System.Collections.Generic;

namespace UnityChess
{
    public class Pawn : Piece
    {
        public override void UpdateValidMoves(LinkedList<Board> boardList)
        {
            ValidMoves.Clear();
        }

        public Pawn(Square startingPosition, Side side) : base(startingPosition, side) { }
        public Pawn(Pawn pawnCopy) : base(pawnCopy) { }

        public override Piece Clone()
        {
            return new Pawn(this);
        }
    }
}