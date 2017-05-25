using System;
using System.Collections.Generic;

namespace UnityChess
{
    public abstract class Piece
    {
        public Side Side { get; set; }
        public Square Position { get; set; }
        public bool HasMoved { get; set; }
        public List<Movement> ValidMoves { get; set; }

        public Piece(Square startPosition, Side side)
        {
            this.Side = side;
            this.HasMoved = false;
            this.Position = startPosition;
            this.ValidMoves = new List<Movement>();
        }

        public Piece(Piece pieceCopy)
        {
            this.Side = pieceCopy.Side;
            this.HasMoved = pieceCopy.HasMoved;
            this.Position = new Square(pieceCopy.Position);
            //deep copy of valid moves list
            this.ValidMoves = pieceCopy.ValidMoves.ConvertAll<Movement>(move => new Movement(new Square(pieceCopy.Position), this));
        }

        public abstract Piece Clone();

        public abstract void UpdateValidMoves(LinkedList<Board> boardList);

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Piece piece = obj as Piece;
            return (Side == piece.Side && HasMoved == piece.HasMoved && Position == piece.Position);
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + Side.GetHashCode();
            hash = (hash * 7) + Position.GetHashCode();
            hash = (hash * 7) + HasMoved.GetHashCode();
            return hash;
        }
    }
}