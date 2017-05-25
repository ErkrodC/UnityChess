using System;

namespace UnityChess
{
    public class Square
    {
        public int Rank { get; set; }
        public int File { get; set; }

        public Square(int rank, int file)
        {
            this.Rank = rank;
            this.File = file;
        }

        public Square(Square squareCopy)
        {
            this.Rank = squareCopy.Rank;
            this.File = squareCopy.File;
        }

        public Square(int oneDimensionalIndex)
        {
            this.File = oneDimensionalIndex % 10;
            this.Rank = 9 - ((oneDimensionalIndex - this.File) / 10 - 1);
        }

        public bool IsValid()
        {
            return ((1 <= File && File <= 8) && (1 <= Rank && Rank <= 8));
        }

        public bool OccupiedByPiece(Board board)
        {
            Object obj = board.BoardPosition[SquareAsIndex(this)];
            return (obj is Piece);
        }

        public static int SquareAsIndex(Square square)
        {
            return ((10 - square.Rank) * 10) + square.File;
        }

        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Square square = obj as Square;
            return (Rank == square.Rank && File == square.File);
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + Rank.GetHashCode();
            hash = (hash * 7) + File.GetHashCode();
            return hash;
        }
    }
}