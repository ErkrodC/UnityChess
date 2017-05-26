using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityChess
{
    class EnPassantMove : SpecialMove
    {
        public EnPassantMove(Square end, Piece piece, Piece capturedPawn) : base(end, piece, capturedPawn) { }

        public override void HandleAssociatedPiece(Board board)
        {
            throw new NotImplementedException();
        }
    }
}