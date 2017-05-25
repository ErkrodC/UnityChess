using System;
using System.Collections.Generic;

namespace UnityChess
{
    public class Board
    {
        public List<BasePiece> BoardPosition { get; set; }
        public static Empty EmptyPiece = new Empty();
        public static Invalid InvalidPiece = new Invalid();

        //used for initial board
        public Board()
        {
            this.BoardPosition = new List<BasePiece>(120);
            this.SetStartingPosition();
            this.UpdateAllPiecesValidMoves();
        }

        //used for copying a board
        public Board(Board board)
        {
            this.SetBlankBoard();

            //creates deep copy (makes copy of each piece and deep copy of their respective ValidMoves lists) of board (list of BasePiece's)
            //this may be a memory hog since each Board has a list of Piece's, and each piece has a list of Movement's
            //avg number turns/Board's per game should be around ~80. usual max number of pieces per board is 32
            for (int i=0; i<120; i++)
            {
                if (board.BoardPosition[i].Type != PieceType.Invalid && board.BoardPosition[i].Type != PieceType.Empty)
                {
                    //clone is a deep copy for BasePiece derivatives, see specific piece class...
                    this.BoardPosition[i] = board.BoardPosition[i].Clone();
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
                this.BoardPosition[i] = new Pawn(new Square(i), PieceType.BlackPawn);
                this.BoardPosition[i + 50] = new Pawn(new Square(i + 50), PieceType.WhitePawn);
            }

            //Rows 3-6/Ranks 6-3, empty inbetween squares
            for (i = 41; i < 79; i++)
            {
                this.BoardPosition[i] = EmptyPiece;
            }

            //Rows 1 & 8/Ranks 8 & 1, back rows for both players
            this.BoardPosition[21] = new Rook(new Square(21), PieceType.BlackRook);
            this.BoardPosition[22] = new Knight(new Square(22), PieceType.BlackKnight);
            this.BoardPosition[23] = new Bishop(new Square(23), PieceType.BlackBishop);
            this.BoardPosition[24] = new Queen(new Square(24), PieceType.BlackQueen);
            this.BoardPosition[25] = new King(new Square(25), PieceType.BlackKing);
            this.BoardPosition[26] = new Bishop(new Square(26), PieceType.BlackBishop);
            this.BoardPosition[27] = new Knight(new Square(27), PieceType.BlackKnight);
            this.BoardPosition[28] = new Rook(new Square(28), PieceType.BlackRook);

            this.BoardPosition[91] = new Rook(new Square(91), PieceType.WhiteRook);
            this.BoardPosition[92] = new Knight(new Square(92), PieceType.WhiteKnight);
            this.BoardPosition[93] = new Bishop(new Square(93), PieceType.WhiteBishop);
            this.BoardPosition[94] = new Queen(new Square(94), PieceType.WhiteQueen);
            this.BoardPosition[95] = new King(new Square(95), PieceType.WhiteKing);
            this.BoardPosition[96] = new Bishop(new Square(96), PieceType.WhiteBishop);
            this.BoardPosition[97] = new Knight(new Square(97), PieceType.WhiteKnight);
            this.BoardPosition[98] = new Rook(new Square(98), PieceType.WhiteRook);
        }

        public void MovePiece(Movement move)
        {
            this.BoardPosition[Square.squareAsIndex(move.Piece.Position)] = EmptyPiece;
            this.BoardPosition[Square.squareAsIndex(move.End)] = move.Piece;

            move.Piece.HasMoved = true;
            move.Piece.Position = move.End;

            if (move is SpecialMove) { (move as SpecialMove).HandleAssociatedPiece(this); }
        }

        public void UpdateAllPiecesValidMoves()
        {
            foreach (BasePiece BP in this.BoardPosition)
            {
                if (BP is Piece) { (BP as Piece).UpdateValidMoves(this); }
            }
        }
    }
}