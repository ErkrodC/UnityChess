using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityChess
{
    class CastlingMove : SpecialMove
    {
        public CastlingMove(Square end, Piece piece, Piece rook) : base(end, piece, rook) { }

        public override void HandleAssociatedPiece(Board board)
        {
            //queenside castling move
            if (AssociatedPiece.Position.File == 1)
            {
                board.MovePiece(new Movement(AssociatedPiece.Position.File + 3, AssociatedPiece.Position.Rank, AssociatedPiece));
            }
            //kingside castling move
            else
            {
                board.MovePiece(new Movement(AssociatedPiece.Position.File - 2, AssociatedPiece.Position.Rank, AssociatedPiece));
            }
        }
    }
}