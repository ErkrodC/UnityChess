using System;
using System.Collections.Generic;

namespace UnityChess
{
    /// <summary>
    /// Representation of a standard chess game including a history of moves made.
    /// </summary>
    public class Game
    {
        public Side CurrentTurn { get; set; }
        public int TurnCount { get; set; }
        public Mode Mode { get; set; }
        public LinkedList<Board> BoardList { get; set; }
        public List<Movement> PreviousMoves { get; set; }

        /// <summary>
        /// Creates a new human versus human Game instance.
        /// </summary>
        public Game() : this(Mode.HvH) {}

        /// <summary>
        /// Creates a Game instance of a given mode with a standard starting Board.
        /// </summary>
        /// <param name="mode">Describes which players are human or AI.</param>
        public Game(Mode mode)
        {
            this.CurrentTurn = Side.White;
            this.TurnCount = 0;
            this.Mode = mode;
            this.BoardList = new LinkedList<Board>();
            this.BoardList.AddLast(new Board());
            this.PreviousMoves = new List<Movement>();

            UpdateAllPiecesValidMoves(this.BoardList, Side.White);
            CheckRules.KingsInitialized = false;
        }

        /// <summary>
        /// Executes passed move and switches sides; also adds move to history.
        /// </summary>
        public void ExecuteTurn(Movement move)
        {
            // NOTE may be safe to assume move being passed is a legal move since it should be getting grabbed from a Piece's ValidMoves list
            if (!move.IsLegal(CurrentTurn))
            {
                // PSEUDO call to gui method which notifies user made invalid move
                return;
            }
            

            //create new copy of previous current board, and execute the move on it
            Board resultingBoard = new Board(BoardList.Last.Value);
            resultingBoard.MovePiece(move);

            this.BoardList.AddLast(resultingBoard);
            this.PreviousMoves.Add(move);
            UpdateAllPiecesValidMoves(BoardList, CurrentTurn);

            TurnCount++;
            CurrentTurn = CurrentTurn == Side.White ? Side.Black : Side.White;
        }

        public static void UpdateAllPiecesValidMoves(LinkedList<Board> boardList, Side turn)
        {
            Board currentBoard = boardList.Last.Value;
            foreach (BasePiece BP in currentBoard.BoardPosition)
            {
                if (BP is Piece) { (BP as Piece).UpdateValidMoves(boardList, turn); }
            }
        }
    }
}