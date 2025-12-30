using System;

namespace UnityChess.Core {
	/// <summary>Representation of a promotion move; inherits from SpecialMove.</summary>
	public class PromotionMove : SpecialMove {
		public Piece PromotionPiece { get; private set; }

		/// <summary>Creates a new PromotionMove instance; inherits from SpecialMove.</summary>
		/// <param name="pawnPosition">Position of the promoting pawn.</param>
		/// <param name="end">Square which the promoting pawn is landing on.</param>
		public PromotionMove(Square pawnPosition, Square end) : base(pawnPosition, end) { }

		/// <summary>Handles replacing the promoting pawn with the elected promotion piece.</summary>
		/// <param name="board">Board on which the move is being made.</param>
		public override void HandleAssociatedPiece(Board board) {
			if (PromotionPiece == null) {
				throw new ArgumentNullException(
					$"{nameof(HandleAssociatedPiece)}:\n"
					+ $"{nameof(PromotionMove)}.{nameof(PromotionPiece)} was null.\n"
					+ $"You must first call {nameof(PromotionMove)}.{nameof(SetPromotionPiece)}"
					+ $" before it can be executed."
				);
			}
			
			board[End] = PromotionPiece;
		}

		public void SetPromotionPiece(Piece promotionPiece) {
			PromotionPiece = promotionPiece;
		}
	}
}