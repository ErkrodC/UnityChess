using System;

namespace UnityChess.AI
{
    public class Rules
    {
        public static bool IsPlayerInCheck(Board board, Side turn)
        {
            return false;
        }

        public static bool IsPlayerCheckmated(Board board, Side turn)
        {
            return false;
        }

        public static bool DoesMoveRemoveCheck(Board board, Movement move)
        {
            return false;
        }

        public static bool IsMoveLegal(Board board, Movement move, Side turn)
        {
            return false;
        }
    }
}
