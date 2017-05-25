using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityChess
{
    public abstract class SpecialMove : Movement
    {
        public Piece AssociatedPiece { get; set; }

        public SpecialMove(Square end, Piece piece, Piece associatedPiece) : base(end, piece)
        {
            this.AssociatedPiece = associatedPiece;
        }

        public abstract void HandleAssociatedPiece(Board board);
    }
}