using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityChess.Base
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

        public Square(int oneDimensionalIndex)
        {
            this.File = oneDimensionalIndex % 10;
            this.Rank = 9 - ((oneDimensionalIndex - this.File) / 10 - 1);
        }

        public static int squareAsIndex(Square square)
        {
            return ((10 - square.Rank) * 10) + square.File;
        }
    }
}
