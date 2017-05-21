using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityChess.Base
{
    //because the board is being represented in code as a 10x12 grid, invalid is used to describe those squares that are outside of the 8x8 center (i.e. the chess board)
    public class Invalid : BasePiece
    {
        public Invalid() { this.Type = PieceType.Invalid; }
    }
}
