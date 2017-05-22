using System;

namespace UnityChess
{
    public abstract class BasePiece
    {
        public PieceType Type { get; set; }

        public BasePiece() { }

        public abstract BasePiece Clone();
    }
}
