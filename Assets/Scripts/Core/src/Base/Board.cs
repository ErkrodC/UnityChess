using System;
using System.Collections.Generic;
using UnityChess.Util;

namespace UnityChess.Core {
	/// <summary>An 8x8 matrix representation of a chessboard.</summary>
	public class Board {
		private readonly Piece[,] boardMatrix;
		private readonly Dictionary<Side, Square?> currentKingSquareBySide = new Dictionary<Side, Square?> {
			[Side.White] = null,
			[Side.Black] = null
		};

		public Piece this[Square position] {
			get {
				if (position.IsValid()) return boardMatrix[position.File, position.Rank];
				throw new ArgumentOutOfRangeException($"Position was out of range: {position}");
			}

			set {
				if (position.IsValid()) boardMatrix[position.File, position.Rank] = value;
				else throw new ArgumentOutOfRangeException($"Position was out of range: {position}");
			}
		}

		public Piece this[int file, int rank] {
			get => this[new Square(file, rank)];
			set => this[new Square(file, rank)] = value;
		}

		/// <summary>Creates a Board given the passed square-piece pairs.</summary>
		public Board(params (Square, Piece)[] squarePiecePairs) {
			boardMatrix = new Piece[8, 8];

			foreach ((Square position, Piece piece) in squarePiecePairs) {
				this[position] = piece;
			}
		}

		/// <summary>Creates a deep copy of the passed Board.</summary>
		public Board(Board board) {
			// TODO optimize this method
			// Creates deep copy (makes copy of each piece and deep copy of their respective ValidMoves lists) of board (list of BasePiece's)
			// this may be a memory hog since each Board has a list of Piece's, and each piece has a list of Movement's
			// avg number turns/Board's per game should be around ~80. usual max number of pieces per board is 32
			boardMatrix = new Piece[8, 8];
			for (int file = 0; file < 8; file++) {
				for (int rank = 0; rank < 8; rank++) {
					Piece pieceToCopy = board[file, rank];
					if (pieceToCopy == null) { continue; }

					this[file, rank] = pieceToCopy.DeepCopy();
				}
			}
		}

		public void ClearBoard() {
			for (int file = 0; file < 8; file++) {
				for (int rank = 0; rank < 8; rank++) {
					this[file, rank] = null;
				}
			}

			currentKingSquareBySide[Side.White] = null;
			currentKingSquareBySide[Side.Black] = null;
		}

		public static readonly (Square, Piece)[] StartingPositionPieces = {
			(new Square("a1"), new Rook(Side.White)),
			(new Square("b1"), new Knight(Side.White)),
			(new Square("c1"), new Bishop(Side.White)),
			(new Square("d1"), new Queen(Side.White)),
			(new Square("e1"), new King(Side.White)),
			(new Square("f1"), new Bishop(Side.White)),
			(new Square("g1"), new Knight(Side.White)),
			(new Square("h1"), new Rook(Side.White)),

			(new Square("a2"), new Pawn(Side.White)),
			(new Square("b2"), new Pawn(Side.White)),
			(new Square("c2"), new Pawn(Side.White)),
			(new Square("d2"), new Pawn(Side.White)),
			(new Square("e2"), new Pawn(Side.White)),
			(new Square("f2"), new Pawn(Side.White)),
			(new Square("g2"), new Pawn(Side.White)),
			(new Square("h2"), new Pawn(Side.White)),

			(new Square("a8"), new Rook(Side.Black)),
			(new Square("b8"), new Knight(Side.Black)),
			(new Square("c8"), new Bishop(Side.Black)),
			(new Square("d8"), new Queen(Side.Black)),
			(new Square("e8"), new King(Side.Black)),
			(new Square("f8"), new Bishop(Side.Black)),
			(new Square("g8"), new Knight(Side.Black)),
			(new Square("h8"), new Rook(Side.Black)),

			(new Square("a7"), new Pawn(Side.Black)),
			(new Square("b7"), new Pawn(Side.Black)),
			(new Square("c7"), new Pawn(Side.Black)),
			(new Square("d7"), new Pawn(Side.Black)),
			(new Square("e7"), new Pawn(Side.Black)),
			(new Square("f7"), new Pawn(Side.Black)),
			(new Square("g7"), new Pawn(Side.Black)),
			(new Square("h7"), new Pawn(Side.Black)),
		};

		public void MovePiece(Movement move) {
			if (this[move.Start] is not { } pieceToMove) {
				throw new ArgumentException($"No piece was found at the given position: {move.Start}");
			}

			this[move.Start] = null;
			this[move.End] = pieceToMove;

			if (pieceToMove is King) {
				currentKingSquareBySide[pieceToMove.Owner] = move.End;
			}

			(move as SpecialMove)?.HandleAssociatedPiece(this);
		}

		internal bool IsOccupiedAt(Square position) => this[position] != null;

		internal bool IsOccupiedBySideAt(Square position, Side side) => this[position] is Piece piece && piece.Owner == side;

		public Square GetKingSquare(Side player) {
			if (currentKingSquareBySide[player] == null) {
				for (int file = 0; file < 8; file++) {
					for (int rank = 0; rank < 8; rank++) {
						if (this[file, rank] is King king) {
							currentKingSquareBySide[king.Owner] = new Square(file, rank);
						}
					}
				}
			}

			return currentKingSquareBySide[player] ?? Square.Invalid;
		}

		public string ToTextArt() {
			string result = string.Empty;

			for (int rank = 7; rank >= 0; --rank) {
				for (int file = 0; file < 8; ++file) {
					Piece piece = this[file, rank];
					result += piece.ToTextArt();
					result += file != 7
						? "|"
						: $"\t {rank + 1}";
				}

				result += "\n";
			}

			result += "a b c d e f g h";

			return result;
		}
	}
}