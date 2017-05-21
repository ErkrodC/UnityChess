using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityChess.Base
{
    public abstract class BasePiece
    {
        public PieceType Type { get; set; }

        public BasePiece() { }
    }
}
