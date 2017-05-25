using System;

namespace UnityChess
{
    //because the board is being represented in code as a 10x12 grid (as a 1-D array, namely BoardPosition),
    //invalid is used to describe those squares that are outside of the 8x8 center (i.e. the chess board)
    public class InvalidPiece
    {
        public InvalidPiece() { }
    }
}