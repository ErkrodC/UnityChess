namespace UnityChess.Util {
	public static class PieceUtil {
		public static string GetPieceTextArt(PieceType pieceType, Side owner) {
			return pieceType switch {
				PieceType.Pawn when owner == Side.Black => "♟",
				PieceType.Pawn when owner == Side.White => "♙",
				PieceType.Knight when owner == Side.Black => "♞",
				PieceType.Knight when owner == Side.White => "♘",
				PieceType.Bishop when owner == Side.Black => "♝",
				PieceType.Bishop when owner == Side.White => "♗",
				PieceType.Rook when owner == Side.Black => "♜",
				PieceType.Rook when owner == Side.White => "♖",
				PieceType.Queen when owner == Side.Black => "♛",
				PieceType.Queen when owner == Side.White => "♕",
				PieceType.King when owner == Side.Black => "♚",
				PieceType.King when owner == Side.White => "♔",
				_ => ""
			};
		}
	}
}