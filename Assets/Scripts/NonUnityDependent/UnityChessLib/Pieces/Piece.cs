using System.Collections.Generic;

namespace UnityChess {
	/// <summary>Base class for any chess piece.</summary>
	public abstract class Piece {
		public readonly Side Color;
		public readonly LegalMovesList LegalMoves;
		public Square Position;
		public bool HasMoved;
		protected int ID;

		protected Piece(Square startPosition, Side color) {
			Color = color;
			HasMoved = false;
			Position = startPosition;
			LegalMoves = new LegalMovesList();
		}

		protected Piece(Piece pieceCopy) {
			Color = pieceCopy.Color;
			HasMoved = pieceCopy.HasMoved;
			Position = pieceCopy.Position;
			LegalMoves = pieceCopy.LegalMoves.DeepCopy();
			ID = pieceCopy.ID;
		}

		public abstract Piece Clone();

		public abstract void UpdateValidMoves(Board board, LinkedList<Turn> previousMoves);

		// override object.Equals
		public override bool Equals(object obj) {
			if (obj == null || GetType() != obj.GetType()) {
				return false;
			}

			Piece piece = obj as Piece;
			// ReSharper disable once PossibleNullReferenceException
			return Color == piece.Color && ID == piece.ID;
		}

		// override object.GetHashCode
		public override int GetHashCode() {
			int hash = 13;
			hash = hash * 7 + Color.GetHashCode();
			hash = hash * 7 + ID.GetHashCode();
			return hash;
		}

		public override string ToString() {
			return $"{Color.ToString()} {GetType().ToString().Substring(11)}";
		}
	}
}