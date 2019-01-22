namespace UnityChess {
	public struct HalfMove {
		public readonly Piece Piece;
		public readonly Movement Move;
		public readonly bool CapturedPiece;
		public readonly bool CausedCheck;
		public readonly bool CausedStalemate;
		public readonly bool CausedCheckmate;

		public HalfMove(Piece piece, Movement move, bool capturedPiece, bool causedCheck, bool causedStalemate, bool causedCheckmate) {
			Piece = piece;
			Move = move;
			CapturedPiece = capturedPiece;
			CausedCheck = causedCheck;
			CausedCheckmate = causedCheckmate;
			CausedStalemate = causedStalemate;
		}
		
		// TODO handle promotion and ambiguous piece moves.
		public string ToAlgebraicNotation() {
			string moveText = "";
			string captureText = CapturedPiece ? "x" : "";
			string suffix = CausedCheckmate ? "#" :
			                CausedCheck     ? "+" : "";
			switch (Piece) {
				case Pawn _:
					if (CapturedPiece) moveText += $"{SquareUtil.FileIntToCharMap[Move.Start.File]}x";
					moveText += $"{SquareUtil.SquareToString(Move.End)}{suffix}";
					break;
				case Knight _:
					moveText += $"N{captureText}{SquareUtil.SquareToString(Move.End)}{suffix}";
					break;
				case Bishop _:
					moveText += $"B{captureText}{SquareUtil.SquareToString(Move.End)}{suffix}";
					break;
				case Rook _:
					moveText += $"R{captureText}{SquareUtil.SquareToString(Move.End)}{suffix}";
					break;
				case Queen _:
					moveText += $"Q{captureText}{SquareUtil.SquareToString(Move.End)}{suffix}";
					break;
				case King _:
					if (Move is CastlingMove) moveText += Move.End.File == 3 ? $"O-O-O{suffix}" : $"O-O{suffix}";
					else moveText += $"K{captureText}{SquareUtil.SquareToString(Move.End)}{suffix}";
					break;
			}

			return moveText;
		}
	}
}
