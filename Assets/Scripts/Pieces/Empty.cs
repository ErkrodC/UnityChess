using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityChess.Base
{
    //empty is used to describe a valid square on the board without a piece on it
    public class Empty : BasePiece
    {
        public Empty() { this.Type = PieceType.Empty; }
    }
}
