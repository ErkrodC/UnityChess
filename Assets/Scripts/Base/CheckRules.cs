using System;
using System.Linq;
using System.Collections.Generic;

namespace UnityChess
{
    /// <summary>
    /// Contains methods for checking legality of moves and board positions.
    /// </summary>
    public static class CheckRules
    {
        private static King whiteKing;
        private static King blackKing;
        public static bool KingsInitialized { get; set; }

        /// <summary>
        /// Determines whether the move obeys all checking rules.
        /// </summary>
        /// <param name="turn">The side of the player whose turn it currently is.</param>
        /// <returns></returns>
        public static bool ObeysCheckRules(Board board, Movement move, Side turn)
        {
            return !DoesMoveCauseCheck(board, move, turn) && DoesMoveRemoveCheck(board, move, turn);
        }

        /// <summary>
        /// Checks if the player of the given side has been checkmated.
        /// </summary>
        public static bool IsPlayerCheckmated(Board board, Side side)
        {
            return IsPlayerStalemated(board, side) && IsPlayerInCheck(board, side);
        }

        /// <summary>
        /// Checks if the player of the given side has been stalemated.
        /// </summary>
        public static bool IsPlayerStalemated(Board board, Side side)
        {
            int sumOfValidMoves = 0;

            foreach (Piece p in board.BasePieceList.OfType<Piece>().ToList().FindAll(p => p.Side == side))
            {
                sumOfValidMoves += p.ValidMoves.Count;
            }

            return sumOfValidMoves == 0;
        }

        /// <summary>
        /// Checks if the player of the given side is in check.
        /// </summary>
        public static bool IsPlayerInCheck(Board board, Side side)
        {
            if (!KingsInitialized)
            {
                initKings(board, out whiteKing, out blackKing);
                KingsInitialized = true;
            }

            return IsPlayerInCheck(board, side, whiteKing, blackKing);
        }

        private static bool IsPlayerInCheck(Board board, Side side, King whiteK, King blackK)
        {
            if (side == Side.Black)
            {
                return blackK.AmInCheck(board);
            }
            else
            {
                return whiteK.AmInCheck(board);
            }
        }

        /// <summary>
        /// Checks if a move made by the player of the given side relieves him of the threat of check, if it exists.
        /// </summary>
        public static bool DoesMoveRemoveCheck(Board board, Movement move, Side side)
        {
            if (!IsPlayerInCheck(board, side)) { return true; }

            Board resultingBoard = new Board(board);
            resultingBoard.MovePiece(new Movement(move.End, move.Piece.Clone()));

            return !IsPlayerInCheck(resultingBoard, side);
        }

        /// <summary>
        /// Checks if a move made by the player of the given side exposes his king to the threat of check.
        /// </summary>
        public static bool DoesMoveCauseCheck(Board board, Movement move, Side side)
        {
            King interWhiteKing;
            King interBlackKing;

            Board resultingBoard = new Board(board);
            resultingBoard.MovePiece(new Movement(move.End, move.Piece.Clone()));

            initKings(resultingBoard, out interWhiteKing, out interBlackKing);

            return IsPlayerInCheck(resultingBoard, side, interWhiteKing, interBlackKing);
        }

        private static void initKings(Board board, out King whiteKing, out King blackKing)
        {
            List<King> kings = board.BasePieceList.OfType<King>().ToList();
            whiteKing = kings.Find(k => k.Side == Side.White);
            blackKing = kings.Find(k => k.Side == Side.Black);
        }
    }
}