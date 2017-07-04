using System;
using System.Collections.Generic;

namespace UnityChess
{
    public class King : Piece
    {
        public King(Square startingPosition, Side side) : base(startingPosition, side) { }

        private King(King kingCopy) : base(kingCopy) { }

        public override void UpdateValidMoves(LinkedList<Board> boardList, Side turn)
        {
            ValidMoves.Clear();

            Board board = boardList.Last.Value;

            CheckSurroundingSquares(board, turn);
            CheckCastlingMoves(board, turn);
        }

        public override Piece Clone()
        {
            return new King(this);
        }

        /// <summary>
        /// Checks whether this King instance is under threat of check.
        /// </summary>
        public bool AmInCheck(Board board)
        {
            foreach (BasePiece basePiece in board.BasePieceList)
            {
                if (basePiece is Piece)
                {
                    Piece piece = basePiece as Piece;
                    if (piece.Side != this.Side)
                    {
                        if (IsPieceCheckingMe(board, piece)) { return true; }
                    }
                }
            }

            return false;
        }

        private bool IsPieceCheckingMe(Board board, Piece piece)
        {
            if (piece.Side == this.Side) { return false; }

            foreach (Movement move in piece.ValidMoves)
            {
                if (move.End.Equals(this.Position)) { return true; }
            }

            return false;
        }

        private void CheckSurroundingSquares(Board board, Side turn)
        {
            Square testSquare = new Square(this.Position);
            Movement testMove = new Movement(testSquare, this);

            for (int i = -1; i <= 1; i++) {
                for (int j = -1; j <= 1; j++)
                {
                    if (i == 0 && j == 0) { continue; }

                    testSquare.CopyPosition(this.Position);
                    testSquare.AddVector(i, j);
                    if (testSquare.IsValid() && !testSquare.IsOccupiedByFriendly(board, turn) && CheckRules.ObeysCheckRules(board, testMove, turn))
                    {
                        ValidMoves.Add(new Movement(testMove));
                    }
                }
            }
        }

        private void CheckCastlingMoves(Board board, Side turn)
        {
            if (!this.HasMoved)
            {
                bool kingSideCheck = true;
                List<Square> inBtwnSquares = new List<Square>();
                List<Movement> inBtwnMoves = new List<Movement>();
                List<BasePiece> cornerPieces = new List<BasePiece>
                {
                    //kingside corner square
                    board.BasePieceList[Square.RankFileAsIndex(this.Position.File + 3, this.Position.Rank)],
                    //queenside corner square
                    board.BasePieceList[Square.RankFileAsIndex(this.Position.File - 4, this.Position.Rank)]
                };

                foreach (Rook rook in cornerPieces.FindAll(bp => bp is Rook).ConvertAll<Rook>(bp => bp as Rook))
                {                    
                    if (!rook.HasMoved && rook.Side == this.Side)
                    {
                        inBtwnSquares.Add(new Square(this.Position.File + 1 * (kingSideCheck ? 1 : -1), this.Position.Rank));
                        inBtwnSquares.Add(new Square(this.Position.File + 2 * (kingSideCheck ? 1 : -1), this.Position.Rank));
                        if (!kingSideCheck) { inBtwnSquares.Add(new Square(this.Position.File - 3, this.Position.Rank)); }

                        if (!inBtwnSquares[0].IsOccupied(board) && !inBtwnSquares[1].IsOccupied(board) && (kingSideCheck ? true : !inBtwnSquares[2].IsOccupied(board)))
                        {
                            inBtwnMoves.Add(new Movement(inBtwnSquares[0], this));
                            inBtwnMoves.Add(new Movement(inBtwnSquares[1], this));

                            if (CheckRules.ObeysCheckRules(board, inBtwnMoves[0], turn) && CheckRules.ObeysCheckRules(board, inBtwnMoves[1], turn))
                            {
                                ValidMoves.Add(new CastlingMove(new Square(inBtwnSquares[1]), this, rook));
                            }

                            inBtwnMoves.Clear();
                        }

                        inBtwnSquares.Clear();
                    }

                    kingSideCheck = false;
                }
            }
        }
    }
}