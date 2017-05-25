using System;
using System.Collections.Generic;

namespace UnityChess
{
    public class Game
    {
        public Side CurrentTurn { get; set; }
        public int TurnCount { get; set; }
        public ModeType Mode { get; set; }
        public LinkedList<Board> BoardList { get; set; }
        public List<Movement> PreviousMoves { get; set; }

        public Game(ModeType mode)
        {
            this.CurrentTurn = Side.White;
            this.TurnCount = 0;
            this.Mode = mode;
            this.BoardList = new LinkedList<Board>();
            this.BoardList.AddLast(new Board());
            this.PreviousMoves = new List<Movement>();

            UpdateAllPiecesValidMoves(this.BoardList);
        }

        //adds a new instance of a Board that represents the board achieved my making a certain move.
        public void ExecuteTurn(Movement move)
        {
            //validate move is legal
            //if (!isLegal(move)) return; //call to gui method which notifies user made invalid move

            //create new copy of previous current board, and execute the move on it
            Board CurrentBoard = new Board(BoardList.Last.Value);
            CurrentBoard.MovePiece(move);

            //this needs to be here since en passant's legality depends on past moves, therefore requiring BoardList
            UpdateAllPiecesValidMoves(BoardList);

            //add new current board to history of boards(linked list)
            this.BoardList.AddLast(CurrentBoard);

            //save move is history of moves, increment turn count
            this.PreviousMoves[TurnCount] = move;
            this.TurnCount++;

            //switch sides (toggle CurrentTurn)
            CurrentTurn = CurrentTurn == Side.White ? Side.Black : Side.White;
        }

        public static void UpdateAllPiecesValidMoves(LinkedList<Board> boardList)
        {
            Board currentBoard = boardList.Last.Value;
            foreach (Object obj in currentBoard.BoardPosition)
            {
                if (obj is Piece) { (obj as Piece).UpdateValidMoves(boardList); }
            }
        }
    }
}