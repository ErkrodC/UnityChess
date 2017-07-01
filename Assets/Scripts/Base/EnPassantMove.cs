using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityChess
{
    /// <summary>
    /// Reresentation of an en passant move; inherits from SpecialMove.
    /// </summary>
    public class EnPassantMove : SpecialMove
    {
        /// <summary>
        /// Creates a new EnPassantMove instance; inherits from SpecialMove.
        /// </summary>
        /// <param name="end">Square on which the attacking pawn will land on.</param>
        /// <param name="pawn">Pawn which is attacking.</param>
        /// <param name="capturedPawn">Pawn which is being capture via en passant.</param>
        public EnPassantMove(Square end, Pawn pawn, Pawn capturedPawn) : base(end, pawn, capturedPawn) { }

        /// <summary>
        /// Handles removing the captured pawn from the board.
        /// </summary>
        /// <param name="board">Board on which the move is being made.</param>
        public override void HandleAssociatedPiece(Board board)
        {
            board.BasePieceList[AssociatedPiece.Position.AsIndex()] = Board.EmptyPiece;

            //may be unnecessary since GC will/should destroy pieces removed from the board. Mostly here for safety.
            Piece.HasMoved = true;
            Piece.Position = null;
        }
    }
}