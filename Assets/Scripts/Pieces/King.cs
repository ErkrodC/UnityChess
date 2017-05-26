using System;
using System.Collections.Generic;

namespace UnityChess
{
    public class King : Piece
    {
        public override void UpdateValidMoves(LinkedList<Board> boardList)
        {
            ValidMoves.Clear();

            Board board = boardList.Last.Value;
            Square testSquare = new Square(this.Position);
            Movement testMove = new Movement(testSquare, this);
            
            //check squares surrounding king
            for (int i = -1; i<=1; i++)
            {
                for (int j=-1; j<=1; j++)
                {
                    //skip square that king is on
                    if (i == 0 && j == 0) { continue; }

                    testSquare.CopyPosition(this.Position);
                    testSquare.AddVector(i, j);
                    if (testSquare.IsValid() && !testSquare.IsOccupied(board) && Rules.DoesMoveCauseCheck(board, testMove, this.Side) && !Rules.DoesMoveCauseCheck(board, testMove, this.Side))
                    {
                        ValidMoves.Add(new Movement(testMove));
                    }
                }
            }

            //check for kingside castling move
            Square inBtwnSquare1 = new Square(this.Position.File + 1, this.Position.Rank);
            Square inBtwnSquare2 = new Square(this.Position.File + 2, this.Position.Rank);
            Object potentialRook = board.BoardPosition[Square.RankFileAsIndex(this.Position.File + 3, this.Position.Rank)];
            if (potentialRook is Rook && !this.HasMoved)
            {
                Piece rook = potentialRook as Rook;
                if (!rook.HasMoved && !inBtwnSquare1.IsOccupied(board) && !inBtwnSquare2.IsOccupied(board))
                {
                    ValidMoves.Add(new CastlingMove(new Square(inBtwnSquare2), this, rook));
                }
            }

            //check for queenside castling move
            inBtwnSquare1 = new Square(this.Position.File - 1, this.Position.Rank);
            inBtwnSquare2 = new Square(this.Position.File - 2, this.Position.Rank);
            Square inBtwnSquare3 = new Square(this.Position.File - 3, this.Position.Rank);
            potentialRook = board.BoardPosition[Square.RankFileAsIndex(this.Position.File - 4, this.Position.Rank)];
            if (potentialRook is Rook && !this.HasMoved)
            {
                Piece rook = potentialRook as Rook;
                if (!rook.HasMoved && !inBtwnSquare1.IsOccupied(board) && !inBtwnSquare2.IsOccupied(board) && !inBtwnSquare3.IsOccupied(board))
                {
                    ValidMoves.Add(new CastlingMove(new Square(inBtwnSquare2), this, rook));
                }
            }
        }

        public King(Square startingPosition, Side side) : base(startingPosition, side) { }
        public King(King kingCopy) : base(kingCopy) { }

        public override Piece Clone()
        {
            return new King(this);
        }
    }
}