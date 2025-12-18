using System;

namespace UnityChess.Core {
	/// <summary>Representation of a castling move; inherits from SpecialMove.</summary>
	public class CastlingMove : SpecialMove {
		public readonly Square RookSquare;

		/// <summary>Creates a new CastlingMove instance.</summary>
		/// <param name="kingPosition">Position of the king to be castled.</param>
		/// <param name="end">Square on which the king will land on.</param>
		/// <param name="rookSquare">The square of the rook associated with the castling move.</param>
		public CastlingMove(Square kingPosition, Square end, Square rookSquare) : base(kingPosition, end) {
			RookSquare = rookSquare;
		}

		/// <summary>Handles moving the associated rook to the correct position on the board.</summary>
		/// <param name="board">Board on which the move is being made.</param>
		public override void HandleAssociatedPiece(Board board) {
			if (board[RookSquare] is Rook rook) {
				board[RookSquare] = null;
				board[GetRookEndSquare()] = rook;
			} else {
				throw new ArgumentException(
					$"{nameof(CastlingMove)}.{nameof(HandleAssociatedPiece)}:\n"
					+ $"No {nameof(Rook)} found at {nameof(RookSquare)}"
				);
			}
		}

		public Square GetRookEndSquare() {
			int rookFileOffset = RookSquare.File switch {
				0 => 3,
				7 => -2,
				_ => throw new ArgumentException(
					$"{nameof(RookSquare)}.{nameof(RookSquare.File)} is invalid"
				)
			};

			return RookSquare + new Square(rookFileOffset, 0);
		}
	}
}