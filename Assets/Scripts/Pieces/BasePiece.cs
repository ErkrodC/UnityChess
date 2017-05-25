using System;

namespace UnityChess
{
    public abstract class BasePiece
    {
        public PieceType Type { get; set; }

        public BasePiece(PieceType type) { this.Type = type; }

        public abstract BasePiece Clone();
    }
}
