using System;

namespace UnityChess {
	public class FENInterchanger : IGameStringInterchanger {
		public string Export(Game game) {
			Board currentBoard = game.BoardTimeline.Current;
			GameConditions currentGameConditions = game.StartingConditions.CalculateEndingGameConditions(currentBoard, game.PreviousMoves.GetStartToCurrent());

			return ConvertCurrentGameStateToFEN(currentBoard, currentGameConditions);
		}
		
		// TODO implement
		public Game Import(string fen) {
			throw new NotImplementedException();
		}

		private static string ConvertCurrentGameStateToFEN(Board currentBoard, GameConditions currentGameConditions) {
			string toMoveString = currentGameConditions.WhiteToMove ? "w" : "b";

			bool areAnyCastlingsAvailable = currentGameConditions.WhiteCanCastleKingside || currentGameConditions.WhiteCanCastleQueenside || currentGameConditions.BlackCanCastleKingside || currentGameConditions.BlackCanCastleQueenside;
			string castlingInfoString = areAnyCastlingsAvailable ?
				$"{(currentGameConditions.WhiteCanCastleKingside ? "K" : "")}{(currentGameConditions.WhiteCanCastleQueenside ? "Q" : "")}{(currentGameConditions.BlackCanCastleKingside ? "k" : "")}{(currentGameConditions.BlackCanCastleQueenside ? "q" : "")}" :
				"-";

			string enPassantSquareString = currentGameConditions.EnPassantSquare.IsValid ?
				SquareUtil.SquareToString(currentGameConditions.EnPassantSquare) :
				"-";

			return $"{CalculateRankStrings(currentBoard)} {toMoveString} {castlingInfoString} {enPassantSquareString} {currentGameConditions.HalfMoveClock} {currentGameConditions.TurnNumber}";
		}

		private static string CalculateRankStrings(Board currentBoard) {
			string[] rankStrings = new string[8];
			for (int rank = 1; rank <= 8; rank++) {
				int emptySquareCount = 0;
				int rankStringsIndex = 7 - (rank - 1);
				rankStrings[rankStringsIndex] = "";
				for (int file = 1; file <= 8; file++) {
					Piece piece = currentBoard[file, rank];
					if (piece == null) {
						emptySquareCount++;

						if (file == 8) { // reached end of rank, append empty square count to rankString
							rankStrings[rankStringsIndex] += emptySquareCount;
							emptySquareCount = 0;
						}
					} else {
						if (emptySquareCount > 0) { // found piece, append empty square count to rankString
							rankStrings[rankStringsIndex] += emptySquareCount;
							emptySquareCount = 0;
						}

						rankStrings[rankStringsIndex] += GetFENPieceSymbol(piece);
					}
				}
			}

			return string.Join("/", rankStrings);
		}

		private static string GetFENPieceSymbol(Piece piece) {
			bool useCaps = piece.Color == Side.White;
			switch (piece) {
				case Bishop _: return useCaps ? "B" : "b";
				case King _: return useCaps ? "K" : "k";
				case Knight _: return useCaps ? "N" : "n";
				case Pawn _: return useCaps ? "P" : "p";
				case Queen _: return useCaps ? "Q" : "q";
				case Rook _: return useCaps ? "R" : "r";
				default: throw new NullReferenceException();
			}
		}

	}
}