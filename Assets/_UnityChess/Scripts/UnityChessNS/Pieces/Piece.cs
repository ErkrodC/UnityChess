using System.Collections.Generic;

namespace UnityChess {
	/// <summary>
	///     Base class for any chess piece.
	/// </summary>
	public abstract class Piece : BasePiece {

		public Piece(Square startPosition, Side side) {
			Side = side;
			HasMoved = false;
			Position = startPosition;
			ValidMoves = new List<Movement>();
		}

		public Piece(Piece pieceCopy) {
			Side = pieceCopy.Side;
			HasMoved = pieceCopy.HasMoved;
			Position = new Square(pieceCopy.Position);
			//deep copy of valid moves list
			ValidMoves = pieceCopy.ValidMoves.ConvertAll(move => new Movement(move));
			ID = pieceCopy.ID;
		}

		public Side Side { get; set; }
		public Square Position { get; set; }
		public bool HasMoved { get; set; }
		public List<Movement> ValidMoves { get; set; }
		public int ID { get; protected set; }

		public abstract Piece Clone();

		public abstract void UpdateValidMoves(Board board, LinkedList<Movement> previousMoves, Side turn);

		// override object.Equals
		public override bool Equals(object obj) {
			if (obj == null || GetType() != obj.GetType()) {
				return false;
			}

			Piece piece = obj as Piece;
			// ReSharper disable once PossibleNullReferenceException
			return Side == piece.Side && ID == piece.ID;
		}

		// override object.GetHashCode
		public override int GetHashCode() {
			int hash = 13;
			hash = hash * 7 + Side.GetHashCode();
			hash = hash * 7 + ID.GetHashCode();
			return hash;
		}

		public override string ToString() {
			return string.Format("{0} {1}", Side.ToString(), GetType().ToString().Substring(11));
		}
	}
}