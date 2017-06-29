using System;

namespace UnityChess
{
    /// <summary>
    /// Representation of a move, namely a piece and its end square.
    /// </summary>
    public class Movement
    {
        public Square End { get; set; }
        public Piece Piece { get; set; }

        /// <summary>
        /// Creates a new Movement.
        /// </summary>
        /// <param name="end">Square which the piece will land on.</param>
        /// <param name="piece">Piece being moved.</param>
        public Movement(Square end, Piece piece)
        {
            this.End = end;
            this.Piece = piece;
        }

        //Used to improve readability
        internal Movement(int file, int rank, Piece piece)
        {
            this.End = new Square(file, rank);
            this.Piece = piece;
        }

        /// <summary>
        /// Copy constructor.
        /// </summary>
        internal Movement(Movement move)
        {
            this.End = new Square(move.End);
            this.Piece = move.Piece;
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