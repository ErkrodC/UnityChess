using System;
using System.Collections.Generic;

namespace UnityChess
{
    public class Rules
    {
        public static bool IsMoveLegal(Board board, Movement move, Side turn)
        {
            ////if (!move.End.IsValid()) { return false; }
            //if (!move.Piece.ValidMoves.Contains(move.End)) { return false; }
            ////if (!DoesMoveRemoveCheck(board, move, turn)) { return false; }
            ////if (DoesMoveCauseCheck(board, move, turn)) { return false; }

            ////return true;

            return move.Piece.ValidMoves.Contains(move);
        }

        public static bool IsPlayerCheckmated(Board board, Side turn)
        {
            if (!IsPlayerInCheck(board, turn)) { return false; }

            return true;
        }

        public static bool IsPlayerInCheck(Board board, Side turn)
        {
            return false;
        }

        public static bool DoesMoveRemoveCheck(LinkedList<Board> boardList, Movement move, Side turn)
        {
            Board board = boardList.Last.Value;
            if (!IsPlayerInCheck(board, turn)) { return true; }

            LinkedList<Board> testBList = new LinkedList<Board>();
            testBList.AddLast(boardList.Last.Previous);
            testBList.AddLast(boardList.Last);

            Board testBoard = new Board(board);
            testBoard.MovePiece(move);
            testBList.AddLast(testBoard);


            Game.UpdateAllPiecesValidMoves(testBList);

            return !IsPlayerInCheck(testBoard, turn);
        }

        public static bool DoesMoveCauseCheck(Board board, Movement move, Side turn)
        {

            return false;
        }
    }
}