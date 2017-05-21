using System;

namespace UnityChess.AI
{
    public class TreeNode
    {
        public Board Board { get; set; }
        public int Depth { get; set; }

        public TreeNode(Board board, int depth)
        {
            this.Board = board;
            this.Depth = depth;
        }
    }
}
