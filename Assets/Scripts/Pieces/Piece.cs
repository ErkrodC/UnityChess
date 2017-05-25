using System;
using System.Collections.Generic;

namespace UnityChess
{
    public abstract class Piece
    {
        public Side Side { get; set; }
        public Square Position { get; set; }
        public bool HasMoved { get; set; }
        public List<Square> ValidMoves { get; set; }

        public Piece(Square startPosition, Side side)
        {
            this.Side = side;
            this.HasMoved = false;
            this.Position = startPosition;
            this.ValidMoves = new List<Square>();
        }

        public Piece(Piece pieceCopy)
        {
            this.Side = pieceCopy.Side;
            this.HasMoved = pieceCopy.HasMoved;
            this.Position = new Square(pieceCopy.Position);
            //deep copy of valid moves list
            this.ValidMoves = pieceCopy.ValidMoves.ConvertAll<Square>(square => new Square(pieceCopy.Position));
        }

        public abstract Piece Clone();

        public abstract void UpdateValidMoves(Board board);
    }
}