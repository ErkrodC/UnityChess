using System;
using System.Collections.Generic;
using ChessRefactor.Base;

namespace ChessRefactor.Pieces
{
    public enum PieceType
    {
        Empty = 0,
        Invalid = 42,
        WhitePawn = 1,
        WhiteKnight = 2,
        WhiteBishop = 3,
        WhiteRook = 4,
        WhiteQueen = 5,
        WhiteKing = 6,
        BlackPawn = -1,
        BlackKnight = -2,
        BlackBishop = -3,
        BlackRook = -4,
        BlackQueen = -5,
        BlackKing = -6
    }

    public abstract class BasePiece
    {
        public PieceType Type { get; set; }

        public BasePiece() { }
    }

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

    public class Pawn : Piece
    {
        public override void GetValidMoves(Board board)
        {
            throw new NotImplementedException();
        }

        public Pawn(Square startingPosition, PieceType type) : base(startingPosition, type) { }
    }

    public class King : Piece
    {
        public override void GetValidMoves(Board board)
        {
            throw new NotImplementedException();
        }

        public King(Square startingPosition, PieceType type) : base(startingPosition, type) { }
    }

    public class Queen : Piece
    {
        public override void GetValidMoves(Board board)
        {
            throw new NotImplementedException();
        }

        public Queen(Square startingPosition, PieceType type) : base(startingPosition, type) { }
    }

    public class Rook : Piece
    {
        public override void GetValidMoves(Board board)
        {
            throw new NotImplementedException();
        }

        public Rook(Square startingPosition, PieceType type) : base(startingPosition, type) { }
    }

    public class Bishop : Piece
    {
        public override void GetValidMoves(Board board)
        {
            throw new NotImplementedException();
        }

        public Bishop(Square startingPosition, PieceType type) : base(startingPosition, type) { }
    }

    public class Knight : Piece
    {
        public override void GetValidMoves(Board board)
        {
            throw new NotImplementedException();
        }

        public Knight(Square startingPosition, PieceType type) : base(startingPosition, type) { }
    }

    //empty is used to describe a valid square on the board without a piece on it
    public class Empty : BasePiece
    {
        public Empty() { this.Type = PieceType.Empty; }
    }

    //because the board is being represented in code as a 10x12 grid, invalid is used to describe those squares that are outside of the 8x8 center (i.e. the chess board)
    public class Invalid : BasePiece
    {
        public Invalid() { this.Type = PieceType.Invalid; }
    }
}
