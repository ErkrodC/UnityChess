using System;

namespace UnityChess
{
    public class Movement
    {
        public Square End { get; set; }
        public Piece Piece { get; set; }

        public Movement(Square end, Piece piece)
        {
            this.End = end;
            this.Piece = piece;
        }

        public Movement(int file, int rank, Piece piece)
        {
            this.End = new Square(file, rank);
            this.Piece = piece;
        }

        public Movement(Movement moveCopy)
        {
            this.End = new Square(moveCopy.End);
            this.Piece = moveCopy.Piece;
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Movement move = obj as Movement;
            return (End.Equals(move.End) && Piece == move.Piece);
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + End.GetHashCode();
            hash = (hash * 7) + Piece.GetHashCode();
            return hash;
        }
    }
}