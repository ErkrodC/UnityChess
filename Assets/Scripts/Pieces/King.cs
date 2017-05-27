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
                    if (testSquare.IsValid() && !testSquare.IsOccupied(board) && !Rules.DoesMoveCauseCheck(board, testMove, turn) && Rules.DoesMoveRemoveCheck(boardList, testMove, turn))
                    {
                        ValidMoves.Add(new Movement(testMove));
                    }
                }
            }

            //check for kingside castling move
            Square inBtwnSquare1 = new Square(this.Position.File + 1, this.Position.Rank);
            Square inBtwnSquare2 = new Square(this.Position.File + 2, this.Position.Rank);
            Square potentialRookSquare = new Square(this.Position.File + 3, this.Position.Rank);
            if (!this.HasMoved && inBtwnSquare1.IsValid() && inBtwnSquare2.IsValid() && potentialRookSquare.IsValid())
            {
                Object potentialRook = board.BoardPosition[potentialRookSquare.AsIndex()];
                Rook rook = potentialRook is Rook ? potentialRook as Rook : null;
                if (rook != null)
                {
                    if (!rook.HasMoved && rook.Side == turn && !inBtwnSquare1.IsOccupied(board) && !inBtwnSquare2.IsOccupied(board))
                    {
                        Movement checkMove1 = new Movement(inBtwnSquare1, this);
                        Movement checkMove2 = new Movement(inBtwnSquare2, this);
                        if (!Rules.DoesMoveCauseCheck(board, checkMove1, turn) && !Rules.DoesMoveCauseCheck(board, checkMove2, turn) &&
                            Rules.DoesMoveRemoveCheck(boardList, checkMove1, turn) && Rules.DoesMoveRemoveCheck(boardList, checkMove2, turn))
                        {
                            ValidMoves.Add(new CastlingMove(new Square(inBtwnSquare2), this, rook));
                        }
                    }
                }
            }

            //check for queenside castling move
            inBtwnSquare1 = new Square(this.Position.File - 1, this.Position.Rank);
            inBtwnSquare2 = new Square(this.Position.File - 2, this.Position.Rank);
            Square inBtwnSquare3 = new Square(this.Position.File - 3, this.Position.Rank);
            potentialRookSquare.SetPosition(this.Position.File - 4, this.Position.Rank);
            if (!this.HasMoved && inBtwnSquare1.IsValid() && inBtwnSquare2.IsValid() && inBtwnSquare3.IsValid() && potentialRookSquare.IsValid())
            {
                Object potentialRook = board.BoardPosition[potentialRookSquare.AsIndex()];
                Rook rook = potentialRook is Rook ? potentialRook as Rook : null;
                if (rook != null)
                {
                    if (!rook.HasMoved && rook.Side == turn && !inBtwnSquare1.IsOccupied(board) && !inBtwnSquare2.IsOccupied(board) && !inBtwnSquare3.IsOccupied(board))
                    {
                        Movement checkMove1 = new Movement(inBtwnSquare1, this);
                        Movement checkMove2 = new Movement(inBtwnSquare2, this);
                        Movement checkMove3 = new Movement(inBtwnSquare3, this);
                        if (!Rules.DoesMoveCauseCheck(board, checkMove1, turn) && !Rules.DoesMoveCauseCheck(board, checkMove2, turn) && !Rules.DoesMoveCauseCheck(board, checkMove3, turn) &&
                            Rules.DoesMoveRemoveCheck(boardList, checkMove1, turn) && Rules.DoesMoveRemoveCheck(boardList, checkMove2, turn) && Rules.DoesMoveRemoveCheck(boardList, checkMove3, turn))
                        {
                            ValidMoves.Add(new CastlingMove(new Square(inBtwnSquare2), this, rook));
                        }
                    }
                }
            }
        }

        public override Piece Clone()
        {
            return new King(this);
        }
    }
}