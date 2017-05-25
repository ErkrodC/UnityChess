using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityChess
{
    class PromotionMove : SpecialMove
    {
        public PromotionMove(Square end, Piece piece, Piece associatedPiece) : base(end, piece, associatedPiece) { }

        public override void HandleAssociatedPiece(Board board)
        {
            throw new NotImplementedException();
        }
    }
}
