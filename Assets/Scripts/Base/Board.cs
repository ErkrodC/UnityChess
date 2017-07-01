using System;
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

        /*
        Boards will be represented as a 10x12 grid in a one dimensional list.
        The 8x8 chessboard is in the center, viewed from above with white on the side closer to index 0.

               0   1  2  3  ...  7  8   9
              10  11 12 13  ... 17 18   19                                  Compass:           (0, -1)
                |---------------------|               file    (1,1)                             White
              20| 21 22 23 ...        | 29      <------------             (1,-1) White-Kingside   |     White-Queenside (-1,-1)
              30| 31 32 33 ...        | 39                  | R                                \  |  /
               .|          .          | .                   | a       (1,0) Kingside--------------*------------------Queenside (-1, 0)
               .|            .        | .                   | n                                /  |  \
               .|              .   98 | 99                  | k            (1,1) Black-Kingside   |     Black-Queenside (-1,1)
                |---------------------|                     V                                   Black
             100  101 102 ... 107 108  109                                                      (0,1)
             110  111 112 ... 117 118  119
        */
        public List<BasePiece> BasePieceList { get; set; }

        /// <summary>
        /// Creates a Board with initial chess game position.
        /// </summary>
        public Board()
        {
            this.BasePieceList = new List<BasePiece>(Enumerable.Range(0, 120).Select(i => (BasePiece)null));
            this.SetStartingPosition();
        }

        /// <summary>
        ///  Creates a deep copy of the passed Board.
        /// </summary>
        public Board(Board board)
        {
            this.SetBlankBoard();

            //creates deep copy (makes copy of each piece and deep copy of their respective ValidMoves lists) of board (list of BasePiece's)
            //this may be a memory hog since each Board has a list of Piece's, and each piece has a list of Movement's
            //avg number turns/Board's per game should be around ~80. usual max number of pieces per board is 32
            // TODO optimize this method
            foreach (Piece piece in BasePieceList.OfType<Piece>())
            {
                BasePieceList[piece.Position.AsIndex()] = piece.Clone();
            }
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
                this.BasePieceList[i] = new Pawn(new Square(i), Side.Black);
                this.BasePieceList[i + 50] = new Pawn(new Square(i + 50), Side.White);
            }

            //Rows 1 & 8/Ranks 8 & 1, back rows for both players
            this.BasePieceList[21] = new Rook(new Square(21), Side.Black);
            this.BasePieceList[22] = new Knight(new Square(22), Side.Black);
            this.BasePieceList[23] = new Bishop(new Square(23), Side.Black);
            this.BasePieceList[24] = new Queen(new Square(24), Side.Black);
            this.BasePieceList[25] = new King(new Square(25), Side.Black);
            this.BasePieceList[26] = new Bishop(new Square(26), Side.Black);
            this.BasePieceList[27] = new Knight(new Square(27), Side.Black);
            this.BasePieceList[28] = new Rook(new Square(28), Side.Black);

            this.BasePieceList[91] = new Rook(new Square(91), Side.White);
            this.BasePieceList[92] = new Knight(new Square(92), Side.White);
            this.BasePieceList[93] = new Bishop(new Square(93), Side.White);
            this.BasePieceList[94] = new Queen(new Square(94), Side.White);
            this.BasePieceList[95] = new King(new Square(95), Side.White);
            this.BasePieceList[96] = new Bishop(new Square(96), Side.White);
            this.BasePieceList[97] = new Knight(new Square(97), Side.White);
            this.BasePieceList[98] = new Rook(new Square(98), Side.White);
        }

        /// <summary>
        /// Used to execute a move.
        /// </summary>
        public void MovePiece(Movement move)
        {
            this.BasePieceList[move.Piece.Position.AsIndex()] = EmptyPiece;
            this.BasePieceList[move.End.AsIndex()] = move.Piece;

            move.Piece.HasMoved = true;
            move.Piece.Position = move.End;

            if (move is SpecialMove) { (move as SpecialMove).HandleAssociatedPiece(this); }
        }
    }
}