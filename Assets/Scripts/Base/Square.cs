using System;

namespace UnityChess
{
    public class Square
    {
        public int File { get; set; }
        public int Rank { get; set; }

        public Square(int file, int rank)
        {
            this.File = file;
            this.Rank = rank;
        }

        public Square(Square squareCopy)
        {
            this.File = squareCopy.File;
            this.Rank = squareCopy.Rank;
        }

        public Square(int oneDimensionalIndex)
        {
            this.File = oneDimensionalIndex % 10;
            this.Rank = 9 - ((oneDimensionalIndex - this.File) / 10 - 1);
        }

        public void AddVector(int file, int rank)
        {
            this.File += file;
            this.Rank += rank;
        }

        public void CopyPosition(Square square)
        {
            this.File = square.File;
            this.Rank = square.Rank;
        }

        public void SetPosition(int file, int rank)
        {
            this.File = file;
            this.Rank = rank;
        }

        public bool IsValid()
        {
            return ((1 <= File && File <= 8) && (1 <= Rank && Rank <= 8));
        }

        public bool IsOccupied(Board board)
        {
            Object obj = board.BoardPosition[this.AsIndex()];
            return (obj is Piece);
        }

        public int AsIndex()
        {
            return ((10 - this.Rank) * 10) + this.File;
        }

        public static int RankFileAsIndex(int file, int rank)
        {
            return ((10 - rank) * 10) + file;
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