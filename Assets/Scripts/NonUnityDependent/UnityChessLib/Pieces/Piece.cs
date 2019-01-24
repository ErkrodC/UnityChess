using System;

namespace UnityChess {
	/// <summary>Base class for any chess piece.</summary>
	public abstract class Piece {
		public readonly Side Color;
		public readonly LegalMovesList LegalMoves;
		public Square Position;
		public bool HasMoved;

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
		}

		public abstract void UpdateLegalMoves(Board board, Square enPassantEligibleSquare);

		public Piece DeepCopy() {
			Type derivedType = GetType();
			return (Piece) derivedType.GetConstructor(new []{derivedType})?.Invoke(new object[]{this}); // copy constructor call
		}

		public override string ToString() => $"{Color} {GetType().Name}";
	}
}