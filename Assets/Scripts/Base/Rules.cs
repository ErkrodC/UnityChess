using System;

namespace UnityChess
{
    public class Rules
    {
        public static bool IsMoveLegal(Board board, Movement move, Side turn)
        {
            if (!move.End.IsValid()) { return false; }
            if (!move.Piece.ValidMoves.Contains(move.End)) { return false; }
            if (!DoesMoveRemoveCheck(board, move, turn)) { return false; }
            if (DoesMoveCauseCheck(board, move, turn)) { return false; }

            return true;
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

        public static bool DoesMoveRemoveCheck(Board board, Movement move, Side turn)
        {
            if (!IsPlayerInCheck(board, turn)) { return true; }

            Board TestBoard = new Board(board);
            TestBoard.MovePiece(move);
            TestBoard.UpdateAllPiecesValidMoves();

            return !IsPlayerInCheck(TestBoard, turn);
        }

        public static bool DoesMoveCauseCheck(Board board, Movement move, Side turn)
        {

            return false;
        }
    }
}