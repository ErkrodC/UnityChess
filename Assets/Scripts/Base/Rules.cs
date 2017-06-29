using System;
using System.Collections.Generic;

namespace UnityChess
{
    /// <summary>
    /// Contains methods for checking legality of moves and board positions.
    /// </summary>
    public static class Rules
    {
        /// <summary>
        /// Checks whether a move is legal on a given board/turn.
        /// </summary>
        /// <param name="turn">Side of the player whose turn it currently is.</param>
        public static bool IsMoveLegal(Board board, Movement move, Side turn)
        {
            ////if (!move.End.IsValid()) { return false; }
            //if (!move.Piece.ValidMoves.Contains(move.End)) { return false; }
            ////if (!DoesMoveRemoveCheck(board, move, turn)) { return false; }
            ////if (DoesMoveCauseCheck(board, move, turn)) { return false; }

            ////return true;
            // TODO determine if this method is done, and clean if so
            return move.Piece.ValidMoves.Contains(move);
        }

        /// <summary>
        /// Checks if the player of the given side has been checkmated.
        /// </summary>
        public static bool IsPlayerCheckmated(Board board, Side side)
        {
            if (!IsPlayerInCheck(board, side)) { return false; }

            // TODO implement rest of this method

            return true;
        }

        /// <summary>
        /// Checks if the player of the given side is in check.
        /// </summary>
        public static bool IsPlayerInCheck(Board board, Side side)
        {
            // TODO implement method

            return false;
        }

        /// <summary>
        /// Checks if a move made by the player of the given side relieves him of the threat of check, if it exists.
        /// </summary>
        public static bool DoesMoveRemoveCheck(Board board, Movement move, Side side)
        {
            // TODO implement method

            return false;
        }

        /// <summary>
        /// Checks if a move made by the player of the given side exposes his king to the threat of check.
        /// </summary>
        public static bool DoesMoveCauseCheck(Board board, Movement move, Side side)
        {
            // TODO implement method
            return false;
        }
    }
}