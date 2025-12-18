using System.Collections.Generic;
using UnityChess.Util;

namespace UnityChess.Core {
	public class FENSerializer : IGameSerializer {
		public string Serialize(Game game) {
			GameConditions currentConditions = game.ConditionsTimeline.Head;
			Square currentEnPassantSquare = currentConditions.EnPassantSquare;

			return
				$"{CalculateBoardString(game.BoardTimeline.Head)}"
				+ $" {(currentConditions.SideToMove == Side.White ? "w" : "b")}"
				+ $" {CalculateCastlingInfoString(currentConditions)}"
				+ $" {(currentEnPassantSquare.IsValid() ? SquareUtil.SquareToString(currentEnPassantSquare) : "-")}"
				+ $" {currentConditions.HalfMoveClock}"
				+ $" {currentConditions.TurnNumber}";
		}

		public Game Deserialize(string fen) {
			string[] split = fen.Split(" ");
			string castlingInfoString = split[2];

			return new Game(
				new GameConditions(
					sideToMove: split[1] == "w" ? Side.White : Side.Black,
					whiteCanCastleKingside: castlingInfoString.Contains("K"),
					whiteCanCastleQueenside: castlingInfoString.Contains("Q"),
					blackCanCastleKingside: castlingInfoString.Contains("k"),
					blackCanCastleQueenside: castlingInfoString.Contains("q"),
					enPassantSquare: split[3] == "-" ? Square.Invalid : new Square(split[3]),
					halfMoveClock: split.Length >= 5 ? int.Parse(split[4]) : 0,
					turnNumber: split.Length >= 6 ? int.Parse(split[5]) : 0
				),
				GetPieces(split[0])
			);
		}

		private static (Square, Piece)[] GetPieces(string boardString) {
			List<(Square, Piece)> result = new List<(Square, Piece)>();

			string[] rankStrings = boardString.Split("/");
			for (int i = 0; i < rankStrings.Length; ++i) {
				string rankString = rankStrings[i];

				int file = 0;
				foreach (char character in rankString) {
					if (int.TryParse(character.ToString(), out int emptySpaces)) {
						file += emptySpaces;
						continue;
					}

					result.Add((
						new Square(file++, 7 - i),
						GetPieceFromFENSymbol(
							character.ToString()
						)
					));
				}
			}

			return result.ToArray();
		}

		private static string CalculateBoardString(Board currentBoard) {
			string[] rankStrings = new string[8];
			for (int rank = 0; rank < 8; rank++) {
				int emptySquareCount = 0;
				int rankStringsIndex = 7 - rank;
				rankStrings[rankStringsIndex] = "";
				for (int file = 0; file < 8; file++) {
					Piece piece = currentBoard[file, rank];
					if (piece == null) {
						emptySquareCount++;

						if (file == 7) { // reached end of rank, append empty square count to rankString
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

		private static Piece GetPieceFromFENSymbol(string character) {
			string loweredSymbol = character.ToLower();
			Side side = loweredSymbol == character
				? Side.Black
				: Side.White;

			return loweredSymbol switch {
				"b" => new Bishop(side),
				"k" => new King(side),
				"n" => new Knight(side),
				"p" => new Pawn(side),
				"q" => new Queen(side),
				"r" => new Rook(side),
				_ => null
			};
		}

		private static string GetFENPieceSymbol(Piece piece) {
			string result = piece switch {
				Bishop => "B",
				King => "K",
				Knight => "N",
				Pawn => "P",
				Queen => "Q",
				Rook => "R",
				_ => "U"
			};

			return piece switch {
				{ Owner: Side.Black } => result.ToLower(),
				_ => result
			};
		}

		private static string CalculateCastlingInfoString(GameConditions currentGameConditions) {
			bool anyCastlingAvailable = currentGameConditions.WhiteCanCastleKingside || currentGameConditions.WhiteCanCastleQueenside || currentGameConditions.BlackCanCastleKingside || currentGameConditions.BlackCanCastleQueenside;
			string castlingInfoString = "";

			if (anyCastlingAvailable) {
				(bool WhiteCanCastleKingside, string)[] castleFlagSymbolPairs = {
					(currentGameConditions.WhiteCanCastleKingside, "K"),
					(currentGameConditions.WhiteCanCastleQueenside, "Q"),
					(currentGameConditions.BlackCanCastleKingside, "k"),
					(currentGameConditions.BlackCanCastleQueenside, "q")
				};

				foreach ((bool canCastle, string fenCastleSymbol) in castleFlagSymbolPairs)
					if (canCastle) castlingInfoString += fenCastleSymbol;
			} else castlingInfoString = "-";

			return castlingInfoString;
		}
	}
}