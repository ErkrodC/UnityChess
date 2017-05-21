using System;
using System.Collections.Generic;

namespace UnityChess
{
    public abstract class Piece : BasePiece
    {
        public Square Position { get; set; }
        public bool HasMoved { get; set; }
        public List<Square> ValidMoves;

        public abstract void GetValidMoves(Board board);

        public Piece(Square startPosition, PieceType type)
        {
            this.HasMoved = false;
            this.Position = startPosition;
            this.Type = type;
            this.ValidMoves = new List<Square>();

            Update(Game.CurrentBoardNode.Value);
        }

        public void Update(Board board)
        {
            ValidMoves.Clear();
            GetValidMoves(board);
        }
    }
}
