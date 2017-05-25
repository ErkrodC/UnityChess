using System;
using System.Collections.Generic;

namespace UnityChess
{
    public abstract class Piece : BasePiece
    {
        public Square Position { get; set; }
        public bool HasMoved { get; set; }
        public List<Square> ValidMoves;

        public Piece(Square startPosition, PieceType type) : base(type)
        {
            this.HasMoved = false;
            this.Position = startPosition;
            this.ValidMoves = new List<Square>();
        }

        public Piece(Piece pieceCopy) : base(pieceCopy.Type)
        {
            this.HasMoved = pieceCopy.HasMoved;
            this.Position = new Square(pieceCopy.Position);
            //deep copy of valid moves list
            this.ValidMoves = pieceCopy.ValidMoves.ConvertAll<Square>(square => new Square(pieceCopy.Position));
        }

        public abstract void UpdateValidMoves(Board board);
    }
}
