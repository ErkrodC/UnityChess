namespace UnityChess {
    public abstract class NewSpecialMove : Movement {
        protected internal NewPiece AssociatedPiece;

        protected NewSpecialMove(Square piecePosition, Square end, NewPiece associatedPiece) : base(piecePosition, end) {
            AssociatedPiece = associatedPiece;
        }

        public abstract void HandleAssociatedPiece(ChessGameState gameState);
    }
}