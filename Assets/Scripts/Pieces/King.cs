using System;
using System.Linq;
using System.Collections.Generic;

namespace UnityChess
{
    public class King : Piece
    {
        private static int instanceCounter = 0;

        public King(Square startingPosition, Side side) : base(startingPosition, side)
        {
            this.ID = ++instanceCounter;
        }

        private King(King kingCopy) : base(kingCopy)
        {
            this.ID = kingCopy.ID;
        }

        public override void UpdateValidMoves(Board board, LinkedList<Movement> previousMoves, Side turn)
        {
            ValidMoves.Clear();

            CheckSurroundingSquares(board, turn);
            CheckCastlingMoves(board, turn);
        }

        public override Piece Clone()
        {
            return new King(this);
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
                    if (testSquare.IsValid() && !testSquare.IsOccupiedBySide(board, this.Side) && Rules.MoveObeysRules(board, testMove, turn) && !testSquare.Equals(this.Side == Side.White ? board.BlackKing.Position : board.WhiteKing.Position))
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
                    board.GetBasePiece(Position.File + 3, Position.Rank),
                    //queenside corner square
                    board.GetBasePiece(Position.File - 4, Position.Rank)
                };

                foreach (Rook rook in cornerPieces.OfType<Rook>())
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

                            if (Rules.MoveObeysRules(board, inBtwnMoves[0], turn) && Rules.MoveObeysRules(board, inBtwnMoves[1], turn))
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