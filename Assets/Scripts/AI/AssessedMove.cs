using UnityChess.Base;

namespace UnityChess.AI
{
    public class AssessedMove
    {
        public Movement Move { get; set; }
        public int Value { get; set; }

        public AssessedMove(Movement move, int value)
        {
            this.Move = move;
            this.Value = value;
        }
    }
}
