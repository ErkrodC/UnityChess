using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityChess
{
    /// <summary>
    /// Reresentation of a castling move; inherits from SpecialMove.
    /// </summary>
    public class CastlingMove : SpecialMove
    {
        /// <summary>
        /// Creates a new CastlingMove instance.
        /// </summary>
        /// <param name="end">Square on which the king will land on.</param>
        /// <param name="king"></param>
        /// <param name="rook"></param>
        public CastlingMove(Square end, King king, Rook rook) : base(end, king, rook) { }

        /// <summary>
        /// Handles moving the associated rook to the correct position on the board.
        /// </summary>
        /// <param name="board">Board on which the move is being made.</param>
        public override void HandleAssociatedPiece(Board board)
        {
            //queenside castling move
            if (AssociatedPiece.Position.File == 1)
            {
                board.MovePiece(new Movement(AssociatedPiece.Position.File + 3, AssociatedPiece.Position.Rank, AssociatedPiece));
            }
            //kingside castling move
            else if (AssociatedPiece.Position.File == 8)
            {
                board.MovePiece(new Movement(AssociatedPiece.Position.File - 2, AssociatedPiece.Position.Rank, AssociatedPiece));
            }
        }
    }
}