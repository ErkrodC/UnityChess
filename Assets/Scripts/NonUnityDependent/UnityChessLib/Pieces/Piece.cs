using System.Collections.Generic;

namespace UnityChess {
	/// <summary>Base class for any chess piece.</summary>
	public abstract class Piece : BasePiece {
		public readonly Side PieceOwner;
		public readonly ValidMovesList ValidMoves;
		public Square Position;
		public bool HasMoved;
		protected int ID;

		protected Piece(Square startPosition, Side pieceOwner) {
			PieceOwner = pieceOwner;
			HasMoved = false;
			Position = startPosition;
			ValidMoves = new ValidMovesList();
		}

		protected Piece(Piece pieceCopy) {
			PieceOwner = pieceCopy.PieceOwner;
			HasMoved = pieceCopy.HasMoved;
			Position = new Square(pieceCopy.Position);
			ValidMoves = pieceCopy.ValidMoves.DeepCopy();
			ID = pieceCopy.ID;
		}

		public abstract Piece Clone();

		public abstract void UpdateValidMoves(Board board, LinkedList<Turn> previousMoves, Side turn);

		// override object.Equals
		public override bool Equals(object obj) {
			if (obj == null || GetType() != obj.GetType()) {
				return false;
			}

			Piece piece = obj as Piece;
			// ReSharper disable once PossibleNullReferenceException
			return PieceOwner == piece.PieceOwner && ID == piece.ID;
		}

		// override object.GetHashCode
		public override int GetHashCode() {
			int hash = 13;
			hash = hash * 7 + PieceOwner.GetHashCode();
			hash = hash * 7 + ID.GetHashCode();
			return hash;
		}

		public override string ToString() {
			return $"{PieceOwner.ToString()} {GetType().ToString().Substring(11)}";
		}
	}
}