﻿using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace UnityChess
{
    /// <summary>
    /// A 120-length, 1-D representation of a chessboard.
    /// </summary>
    public class Board
    {
        public static EmptyPiece EmptyPiece = new EmptyPiece();
        public static InvalidPiece InvalidPiece = new InvalidPiece();
        public King WhiteKing { get; set; }
        public King BlackKing { get; set; }

        /*
        Boards will be represented as a 10x12 grid in a one dimensional list.
        The 8x8 chessboard is in the center, viewed from above with white on the side closer to index 0.

             110  111 112 ... 117 118  119                                  Compass:             (0, 1)
             100  101 102 ... 107 108  109                                                        Black
                |---------------------|               ^                    (-1, 1) Black-Queenside  |   Black-Kingside (1, 1)
               .|              .   98 | 99          R |                                          \  |  /
               .|            .        | .           a |               (-1,0) Queenside--------------*------------------Kingside (1, 0)
               .|          .          | .           n |                                          /  |  \
              30| 31 32 33 ...        | 39          k |                  (-1, -1) White-Queenside   |   White-Kingside (1, -1)
              20| 21 22 23 ...        | 29            ------------>                               White
                |---------------------|         (1,1)       file                                 (0, -1)
              10  11 12 13  ... 17 18   19
               0   1  2  3  ...  7  8   9
        */
        public List<BasePiece> BasePieceList { get; set; }

        /// <summary>
        /// Creates a Board with initial chess game position.
        /// </summary>
        public Board()
        {
            this.BasePieceList = Enumerable.Range(0, 120).Select(i => (BasePiece)null).ToList();
            this.SetStartingPosition();
        }

        /// <summary>
        ///  Creates a deep copy of the passed Board.
        /// </summary>
        public Board(Board board)
        {
            this.BasePieceList = Enumerable.Range(0, 120).Select(i => (BasePiece)null).ToList();
            this.SetBlankBoard();

            //creates deep copy (makes copy of each piece and deep copy of their respective ValidMoves lists) of board (list of BasePiece's)
            //this may be a memory hog since each Board has a list of Piece's, and each piece has a list of Movement's
            //avg number turns/Board's per game should be around ~80. usual max number of pieces per board is 32
            // TODO optimize this method
            foreach (Piece piece in board.BasePieceList.OfType<Piece>())
            {
                BasePieceList[piece.Position.AsIndex()] = piece.Clone();
            }

            initKings();
        }

        /// <summary>
        /// Used to remove all pieces from the board.
        /// </summary>
        public void SetBlankBoard()
        {
            for (int i = 1; i <= 8; i++)
                for (int j = 1; j <= 8; j++)
                {
                    BasePieceList[Square.RankFileAsIndex(i, j)] = EmptyPiece;
                }

            for (int i = 0; i <= 19; i++)
            {
                BasePieceList[i] = InvalidPiece;
                BasePieceList[i + 100] = InvalidPiece;
            }

            for (int i = 20; i <= 90; i += 10)
            {
                BasePieceList[i] = InvalidPiece;
                BasePieceList[i + 9] = InvalidPiece;
            }

            WhiteKing = null;
            BlackKing = null;
        }

        /// <summary>
        /// Used to reset the Board to initial chess game position.
        /// </summary>
        public void SetStartingPosition()
        {

            SetBlankBoard();

            //Row 2/Rank 7 and Row 7/Rank 2, both rows of pawns
            for (int i = 31; i <= 38; i++)
            {
                this.BasePieceList[i] = new Pawn(new Square(i), Side.White);
                this.BasePieceList[i + 50] = new Pawn(new Square(i + 50), Side.Black);
            }

            //Rows 1 & 8/Ranks 8 & 1, back rows for both players
            this.BasePieceList[21] = new Rook(new Square(21), Side.White);
            this.BasePieceList[22] = new Knight(new Square(22), Side.White);
            this.BasePieceList[23] = new Bishop(new Square(23), Side.White);
            this.BasePieceList[24] = new Queen(new Square(24), Side.White);
            this.BasePieceList[25] = new King(new Square(25), Side.White);
            this.BasePieceList[26] = new Bishop(new Square(26), Side.White);
            this.BasePieceList[27] = new Knight(new Square(27), Side.White);
            this.BasePieceList[28] = new Rook(new Square(28), Side.White);

            this.BasePieceList[91] = new Rook(new Square(91), Side.Black);
            this.BasePieceList[92] = new Knight(new Square(92), Side.Black);
            this.BasePieceList[93] = new Bishop(new Square(93), Side.Black);
            this.BasePieceList[94] = new Queen(new Square(94), Side.Black);
            this.BasePieceList[95] = new King(new Square(95), Side.Black);
            this.BasePieceList[96] = new Bishop(new Square(96), Side.Black);
            this.BasePieceList[97] = new Knight(new Square(97), Side.Black);
            this.BasePieceList[98] = new Rook(new Square(98), Side.Black);

            WhiteKing = BasePieceList[25] as King;
            WhiteKing = BasePieceList[95] as King;
        }

        /// <summary>
        /// Used to execute a move.
        /// </summary>
        public void MovePiece(Movement move)
        {
            this.BasePieceList[move.Piece.Position.AsIndex()] = EmptyPiece;
            this.BasePieceList[move.End.AsIndex()] = move.Piece;

            move.Piece.HasMoved = true;
            move.Piece.Position.CopyPosition(move.End);

            if (move is SpecialMove) { (move as SpecialMove).HandleAssociatedPiece(this); }
        }

        public void initKings()
        {
            WhiteKing = (King)BasePieceList.Single(bp => bp is King && (bp as King).Side == Side.White);//kings.Find(k => k.Side == Side.White);
            BlackKing = (King)BasePieceList.Single(bp => bp is King && (bp as King).Side == Side.Black);//kings.Find(k => k.Side == Side.Black);
        }

        // NOTE add methods to handle adding/removing pieces from board?
    }
}