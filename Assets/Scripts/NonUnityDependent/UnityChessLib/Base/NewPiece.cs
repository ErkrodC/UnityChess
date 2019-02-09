using System;

namespace UnityChess {
    /// <summary>Base class for any chess piece.</summary>
    public abstract class NewPiece {
        public readonly Side Color;
        public readonly LegalMovesList LegalMoves;
        public Square Position;
        public bool HasMoved;

        protected NewPiece(Square startPosition, Side color) {
            Color = color;
            HasMoved = false;
            Position = startPosition;
            LegalMoves = new LegalMovesList();
        }

        protected NewPiece(NewPiece pieceCopy) {
            Color = pieceCopy.Color;
            HasMoved = pieceCopy.HasMoved;
            Position = pieceCopy.Position;
            LegalMoves = pieceCopy.LegalMoves.DeepCopy();
        }

        public abstract void UpdateLegalMoves(ChessGameState gameState);

        public NewPiece DeepCopy() {
            Type derivedType = GetType();
            return (NewPiece) derivedType.GetConstructor(new []{derivedType})?.Invoke(new object[]{this}); // copy constructor call
        }

        public override string ToString() => $"{Color} {GetType().Name}";
    }
}