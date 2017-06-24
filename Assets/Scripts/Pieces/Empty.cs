﻿using System;

namespace UnityChess
{
    //empty is used to describe a valid square on the board without a piece on it
    public class Empty : BasePiece
    {
        public Empty() : base(PieceType.Empty) { }

        public override BasePiece Clone()
        {
            return this;
        }
    }
}
