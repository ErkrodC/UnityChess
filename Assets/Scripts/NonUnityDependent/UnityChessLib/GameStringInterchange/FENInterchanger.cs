using System;

namespace UnityChess {
	public class FENInterchanger : IGameStringInterchanger {
		public string Export(Game game) {
			Board currentBoard = game.BoardTimeline.Current;
			GameConditions endingConditions = game.StartingConditions.GetEndingGameConditions(currentBoard, game.PreviousMoves.GetStartToCurrent());
			
			string[] rankStrings = new string[8];
			for (int rank = 1; rank <= 8; rank++) {
				int emptySpaceCount = 0;
				int rankStringsIndex = 7 - (rank - 1);
				rankStrings[rankStringsIndex] = "";
				for (int file = 1; file <= 8; file++) {
					Piece piece = currentBoard[file, rank];
					if (piece == null) {
						emptySpaceCount++;
						if (file == 8) {
							rankStrings[rankStringsIndex] += emptySpaceCount;
							emptySpaceCount = 0;
						}
						continue;
					}
					
					if (emptySpaceCount > 0){
						rankStrings[rankStringsIndex] += emptySpaceCount;
						emptySpaceCount = 0;
					}

					bool useCaps = piece.Color == Side.White;
					switch (piece) {
						case Bishop _:
							rankStrings[rankStringsIndex] += useCaps ? "B" : "b";
							break;
						case King _:
							rankStrings[rankStringsIndex] += useCaps ? "K" : "k";
							break;
						case Knight _:
							rankStrings[rankStringsIndex] += useCaps ? "N" : "n";
							break;
						case Pawn _:
							rankStrings[rankStringsIndex] += useCaps ? "P" : "p";
							break;
						case Queen _:
							rankStrings[rankStringsIndex] += useCaps ? "Q" : "q";
							break;
						case Rook _:
							rankStrings[rankStringsIndex] += useCaps ? "R" : "r";
							break;
					}
				}
			}

			string toMoveString = endingConditions.WhiteToMove ? "w" : "b";
			
			string castlingInfoString =
				endingConditions.WhiteCanCastleKingside || endingConditions.WhiteCanCastleQueenside || endingConditions.BlackCanCastleKingside|| endingConditions.BlackCanCastleQueenside ?
					$"{(endingConditions.WhiteCanCastleKingside ? "K" : "")}{(endingConditions.WhiteCanCastleQueenside ? "Q" : "")}{(endingConditions.BlackCanCastleKingside ? "k" : "")}{(endingConditions.BlackCanCastleQueenside ? "q" : "")}" :
					"-";

			string enPassantSquareString = endingConditions.EnPassantSquare.IsValid ? SquareUtil.SquareToString(endingConditions.EnPassantSquare) : "-";
			
			string fen = $"{string.Join("/", rankStrings)} {toMoveString} {castlingInfoString} {enPassantSquareString} {endingConditions.HalfMoveClock} {endingConditions.TurnNumber}";
			return fen;
		}

		// TODO implement
		public Game Import(string fen) {
			throw new NotImplementedException();
		}
	}
}