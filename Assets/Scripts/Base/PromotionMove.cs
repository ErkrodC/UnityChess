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
        /// <param name="end">Square which the promoting pawn is landing on.</param>
        /// <param name="pawn">The promoting pawn.</param>
        /// <param name="election">The piece to promote the promoting pawn to.</param>
        public PromotionMove(Square end, Pawn pawn, ElectedPiece election) : base(end, pawn, ParseElection(election, end, pawn.Side)) { }

        private static Piece ParseElection(ElectedPiece election, Square end, Side side)
        {
            switch (election)
            {
                case ElectedPiece.Knight:
                    Knight knight = new Knight(end, side);
                    knight.HasMoved = true;
                    knight.Position = end;
                    return knight;
                case ElectedPiece.Bishop:
                    Bishop bishop = new Bishop(end, side);
                    bishop.HasMoved = true;
                    bishop.Position = end;
                    return bishop;
                case ElectedPiece.Rook:
                    Rook rook = new Rook(end, side);
                    rook.HasMoved = true;
                    rook.Position = end;
                    return rook;
                case ElectedPiece.Queen:
                    Queen queen = new Queen(end, side);
                    queen.HasMoved = true;
                    queen.Position = end;
                    return queen;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Handles replacing the promoting pawn with the elected promotion piece.
        /// </summary>
        /// <param name="board">Board on which the move is being made.</param>
        public override void HandleAssociatedPiece(Board board)
        {
            board.BoardPosition[End.AsIndex()] = AssociatedPiece;
        }
    }
}