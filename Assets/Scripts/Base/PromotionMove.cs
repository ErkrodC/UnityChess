using System;

namespace UnityChess
{
    /// <summary>
    /// Reresentation of a promotion move; inherits from SpecialMove.
    /// </summary>
    public class PromotionMove : SpecialMove
    {
        /// <summary>
        /// Creates a new PromotionMove instance; inherits from SpecialMove.
        /// </summary>
        /// <param name="end">Square on which the pawn is promoting.</param>
        /// <param name="pawn">Pawn which is being promoted.</param>
        /// <param name="bishop">Bishop instance that the pawn is promoting to.</param>
        public PromotionMove(Square end, Pawn pawn, Bishop bishop) : base(end, pawn, bishop) { }

        /// <summary>
        /// Creates a new PromotionMove instance; inherits from SpecialMove.
        /// </summary>
        /// <param name="end">Square on which the pawn is promoting.</param>
        /// <param name="pawn">Pawn which is being promoted.</param>
        /// <param name="knight">Knight instance that the pawn is promoting to.</param>
        public PromotionMove(Square end, Pawn pawn, Knight knight) : base(end, pawn, knight) { }

        /// <summary>
        /// Creates a new PromotionMove instance; inherits from SpecialMove.
        /// </summary>
        /// <param name="end">Square on which the pawn is promoting.</param>
        /// <param name="pawn">Pawn which is being promoted.</param>
        /// <param name="queen">Queen instance that the pawn is promoting to.</param>
        public PromotionMove(Square end, Pawn pawn, Queen queen) : base(end, pawn, queen) { }

        /// <summary>
        /// Creates a new PromotionMove instance; inherits from SpecialMove.
        /// </summary>
        /// <param name="end">Square on which the pawn is promoting.</param>
        /// <param name="pawn">Pawn which is being promoted.</param>
        /// <param name="rook">Rook instance that the pawn is promoting to.</param>
        public PromotionMove(Square end, Pawn pawn, Rook rook) : base(end, pawn, rook) { } 

        /// <summary>
        /// Handles replacing the promoting pawn with the elected piece.
        /// </summary>
        /// <param name="board">Board on which the move is being made.</param>
        public override void HandleAssociatedPiece(Board board)
        {
            // TODO implement method
            throw new NotImplementedException();
        }
    }
}