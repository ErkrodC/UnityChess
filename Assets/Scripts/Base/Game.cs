using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityChess.Base
{
    public class Game
    {
        public Side CurrentTurn { get; set; }
        public int TurnCount { get; set; }
        public ModeType Mode { get; set; }
        public BoardList BList;
        public List<Movement> PreviousMoves;
        public static LinkedListNode<Board> CurrentBoardNode = new LinkedListNode<Board>(new Board());

        public Game(ModeType mode)
        {
            this.CurrentTurn = Side.White;
            this.TurnCount = 0;
            this.Mode = mode;
            this.BList = new BoardList(this.TurnCount);
            this.BList.AddLastBoard(CurrentBoardNode);
            this.PreviousMoves = new List<Movement>();
        }

        public void ExecuteMoveAndAddToBList(Movement move)
        {
            this.BList.AddLastBoard(new LinkedListNode<Board>(new Board(CurrentBoardNode.Value, move)));
            this.TurnCount++;
            this.PreviousMoves[TurnCount] = move;
        }
    }
}
