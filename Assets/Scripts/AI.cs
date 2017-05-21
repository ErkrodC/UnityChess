using System;
using ChessRefactor.Base;

namespace ChessRefactor.AI
{
    public class Node
    {
        public Board Board { get; set; }
        public int Depth { get; set; }

        public Node(Board board, int depth)
        {
            this.Board = board;
            this.Depth = depth;
        }
    }

    public class MovementValue
    {
        public Movement Move { get; set; }
        public double Value { get; set; }

        public MovementValue(Movement move, double value)
        {
            this.Move = move;
            this.Value = value;
        }
    }
}
