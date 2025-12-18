using System;
using System.Collections.Generic;

namespace UnityChess.Core {
	public struct HalfMove {
		public readonly Piece Piece;
		public readonly Movement Move;
		public readonly bool CapturedPiece;
		public readonly bool CausedCheck;
		public bool CausedStalemate { get; private set; }
		public bool CausedCheckmate { get; private set; }
		
		private static readonly Dictionary<Type, string> pieceTypeToANSymbolMap = new Dictionary<Type, string> {
			{ typeof(Pawn), "" },
			{ typeof(Knight), "N" },
			{ typeof(Bishop), "B" },
			{ typeof(Rook), "R" },
			{ typeof(Queen), "Q" },
			{ typeof(King), "K" },		
		};

		public HalfMove(Piece piece, Movement move, bool capturedPiece, bool causedCheck) {
			Piece = piece;
			Move = move;
			CapturedPiece = capturedPiece;
			CausedCheck = causedCheck;
			CausedCheckmate = default;
			CausedStalemate = default;
		}

		public void SetGameEndBools(bool causedStalemate, bool causedCheckmate) {
			CausedCheckmate = causedCheckmate;
			CausedStalemate = causedStalemate;
		}
		
		// TODO handle ambiguous piece moves.
		public string ToAlgebraicNotation() {
			string pieceSymbol = Piece is Pawn && CapturedPiece
				? SquareUtil.FileIndexToCharMap[Move.Start.File]
				: pieceTypeToANSymbolMap[Piece.GetType()];

			string capture = CapturedPiece ? "x" : string.Empty;
			string endSquare = SquareUtil.SquareToString(Move.End);
			string suffix = CausedCheckmate
				? "#"
				: CausedCheck
					? "+"
					: string.Empty;

			string moveText;
			switch (Piece) {
				case King when Move is CastlingMove: {
					moveText = Move.End.File == 3 ? $"O-O-O{suffix}" : $"O-O{suffix}";
					break;
				}
				case Pawn: {
					string promotionPiece = Move is PromotionMove promotionMove
						? $"={pieceTypeToANSymbolMap[promotionMove.PromotionPiece.GetType()]}"
						: string.Empty;

					moveText = $"{pieceSymbol}{capture}{endSquare}{promotionPiece}{suffix}";
					break;
				}
				default: {
					moveText = $"{pieceSymbol}{capture}{endSquare}{suffix}";
					break;
				}
			}

			return moveText;
		}
	}
}
