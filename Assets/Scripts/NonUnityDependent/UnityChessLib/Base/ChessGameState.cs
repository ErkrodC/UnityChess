namespace UnityChess {
	public struct ChessGameState {
		public Square EnPassantCaptureSquare {
			get {
				ulong rank2 = flags & Rank2Mask;
				ulong rank7 = flags & Rank7Mask;
				
				if (rank2 != 0) {
					int file = 0;					
					rank2 >>= 8;
					while (rank2 != 0) {
						rank2 >>= 1;
						file++;
					}
					return new Square(file, 2);
				}

				if (rank7 != 0) {
					int file = 0;
					rank7 >>= 48;
					while (rank7 != 0) {
						rank7 >>= 1;
						file++;
					}
					
					return new Square(file, 7);
				}
				
				return Square.Invalid;
			}
		}

		private const ulong Rank1Mask = 0xFF << 0;
		private const ulong Rank2Mask = 0xFF << 8;
		private const ulong Rank3Mask = 0xFF << 16;
		private const ulong Rank4Mask = unchecked((ulong) (0xFF << 24));
		private const ulong Rank5Mask = 0xFF << 32;
		private const ulong Rank6Mask = 0xFF << 40;
		private const ulong Rank7Mask = 0xFF << 48;
		private const ulong Rank8Mask = unchecked((ulong) (0xFF << 56));

		private const ulong File1Mask = 0x0101_0101_0101_0101;
		private const ulong File2Mask = 0x0202_0202_0202_0202;
		private const ulong File3Mask = 0x0404_0404_0404_0404;
		private const ulong File4Mask = 0x0808_0808_0808_0808;
		private const ulong File5Mask = 0x1010_1010_1010_1010;
		private const ulong File6Mask = 0x2020_2020_2020_2020;
		private const ulong File7Mask = 0x4040_4040_4040_4040;
		private const ulong File8Mask = 0x8080_8080_8080_8080;

		private readonly ulong whitePieces;
		private readonly ulong pawns;
		private readonly ulong knights;
		private readonly ulong bishops;
		private readonly ulong rooks;
		private readonly ulong kings;
		private readonly ulong flags;
	
		public ChessGameState(ChessGameState otherState) {
			whitePieces = otherState.whitePieces;
			pawns = otherState.pawns;
			knights = otherState.knights;
			bishops = otherState.bishops;
			rooks = otherState.rooks;
			kings = otherState.kings;
			flags = otherState.flags;
		}

		public bool MoveIsLegal(Movement move) {
			switch (GetPieceType(move.Start)) {
				case PieceType.None:
					return false;
				case PieceType.Pawn:
					switch (move) {
						case EnPassantMove enPassantMove:
							break;
						case PromotionMove promotionMove:
							break;
					}
					break;
				case PieceType.Knight:
					break;
				case PieceType.Bishop:
					break;
				case PieceType.Rook:
					break;
				case PieceType.Queen:
					break;
				case PieceType.King:
					break;
			}
		}

		private PieceType GetPieceType(Square position) {
			if (pawns & position.Mask != 0) return PieceType.Pawn;
			if (knights & position.Mask != 0) return PieceType.Knight;
			if (bishops & position.Mask != 0) return (rooks & position.Mask != 0) ? PieceType.Queen : PieceType.Bishop;
			if (rooks & position.Mask != 0) return (bishops & position.Mask != 0) ? PieceType.Queen : PieceType.Rook;
			if (kings & position.Mask != 0) return PieceType.King;
			return PieceType.None;
		}

		private OccupyStatus GetOccupyStatus(Square position) {
			PieceType pieceTypeAtPosition = GetPieceType(position);
			if (pieceTypeAtPosition == PieceType.None) return OccupyStatus.None;

			return whitePieces & position.Mask != 0 ? OccupyStatus.White : OccupyStatus.Black;
		}

		private enum OccupyStatus {
			None,
			White,
			Black
		}
	
		private enum PieceType : sbyte {
			None = -1,
			Pawn = 0,
			Knight = 1,
			Bishop = 2,
			Rook = 3,
			Queen = 4,
			King = 5
		}
	}
}