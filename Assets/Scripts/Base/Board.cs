using System;
using System.Collections.Generic;

namespace UnityChess
{
    public class Board
    {
        public static EmptyPiece EmptyPiece = new EmptyPiece();
        public static InvalidPiece InvalidPiece = new InvalidPiece();

        /*
        Boards will be represented as a 10x12 grid in a one dimensional list.
        The 8x8 chessboard is in the center.

               0   1  2  3  ...  7  8   9
              10  11 12 13  ... 17 18   19
                |---------------------|
              20| 21 22 23 ...        | 29
              30| 31 32 33 ...        | 39
               .|          .          | .
               .|            .        | .
               .|              .   98 | 99
                |---------------------|
             100  101 102 ... 107 108  109
             110  111 112 ... 117 118  119
        */
        public List<Object> BoardPosition { get; set; }

        //used for initial board
        public Board()
        {
            BoardPosition = new List<Object>(120);
            SetStartingPosition();
        }

        //used for copying a board
        public Board(Board board)
        {
            this.SetBlankBoard();

            //creates deep copy (makes copy of each piece and deep copy of their respective ValidMoves lists) of board (list of BasePiece's)
            //this may be a memory hog since each Board has a list of Piece's, and each piece has a list of Movement's
            //avg number turns/Board's per game should be around ~80. usual max number of pieces per board is 32
            foreach (Object obj in BoardPosition)
            {
                if (obj is Piece)
                {
                    BoardPosition[Square.SquareAsIndex((obj as Piece).Position)] = (obj as Piece).Clone();
                }
            }
        }

        public void SetBlankBoard()
        {
            int i;
            //Will start by setting all squares as invalid, then change to other Piecetypes as necessary
            for (i = 0; i < 120; i++)
            {
                this.BoardPosition[i] = InvalidPiece;
            }

            //empty board squares
            for (i = 21; i < 99; i++)
            {
                this.BoardPosition[i] = EmptyPiece;
            }
        }

        public void SetStartingPosition()
        {
            int i;
            //Will start by setting all squares as invalid, then change to other Piecetypes as necessary
            for (i = 0; i < 120; i++)
            {
                this.BoardPosition[i] = InvalidPiece;
            }

            //Row 2/Rank 7 and Row 7/Rank 2, both rows of pawns
            for (i = 31; i < 39; i++)
            {
                this.BoardPosition[i] = new Pawn(new Square(i), Side.Black);
                this.BoardPosition[i + 50] = new Pawn(new Square(i + 50), Side.White);
            }

            //Rows 3-6/Ranks 6-3, empty inbetween squares
            for (i = 41; i < 79; i++)
            {
                this.BoardPosition[i] = EmptyPiece;
            }

            //Rows 1 & 8/Ranks 8 & 1, back rows for both players
            this.BoardPosition[21] = new Rook(new Square(21), Side.Black);
            this.BoardPosition[22] = new Knight(new Square(22), Side.Black);
            this.BoardPosition[23] = new Bishop(new Square(23), Side.Black);
            this.BoardPosition[24] = new Queen(new Square(24), Side.Black);
            this.BoardPosition[25] = new King(new Square(25), Side.Black);
            this.BoardPosition[26] = new Bishop(new Square(26), Side.Black);
            this.BoardPosition[27] = new Knight(new Square(27), Side.Black);
            this.BoardPosition[28] = new Rook(new Square(28), Side.Black);

            this.BoardPosition[91] = new Rook(new Square(91), Side.White);
            this.BoardPosition[92] = new Knight(new Square(92), Side.White);
            this.BoardPosition[93] = new Bishop(new Square(93), Side.White);
            this.BoardPosition[94] = new Queen(new Square(94), Side.White);
            this.BoardPosition[95] = new King(new Square(95), Side.White);
            this.BoardPosition[96] = new Bishop(new Square(96), Side.White);
            this.BoardPosition[97] = new Knight(new Square(97), Side.White);
            this.BoardPosition[98] = new Rook(new Square(98), Side.White);
        }

        public void MovePiece(Movement move)
        {
            this.BoardPosition[Square.SquareAsIndex(move.Piece.Position)] = EmptyPiece;
            this.BoardPosition[Square.SquareAsIndex(move.End)] = move.Piece;

            move.Piece.HasMoved = true;
            move.Piece.Position = move.End;

            if (move is SpecialMove) { (move as SpecialMove).HandleAssociatedPiece(this); }
        }
    }
}