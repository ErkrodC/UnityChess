using System.Collections.Generic;
using System.Linq;

namespace UnityChess {
	/// <summary>Representation of a standard chess game including a history of moves made.</summary>
	public class Game {
		/// <summary>Creates a Game instance of a given mode with a standard starting Board.</summary>
		/// <param name="mode">Describes which players are human or AI.</param>
		public Game(Mode mode) {
			CurrentTurn = Side.White;
			TurnCount = 0;
			Mode = mode;
			BoardList = new LinkedList<Board>();
			BoardList.AddLast(new Board());
			PreviousMoves = new LinkedList<Movement>();

			UpdateAllPiecesValidMoves(BoardList.Last.Value, PreviousMoves, Side.White);
		}

		public Side CurrentTurn { get; private set; }
		public int TurnCount { get; set; }
		public Mode Mode { get; }
		public LinkedList<Board> BoardList { get; }
		public LinkedList<Movement> PreviousMoves { get; }
		
		/// <summary>Executes passed move and switches sides; also adds move to history.</summary>
		public void ExecuteTurn(Movement move) {
			//create new copy of previous current board, and execute the move on it
			Board resultingBoard = new Board(BoardList.Last.Value);
			resultingBoard.MovePiece(move);

			BoardList.AddLast(resultingBoard);
			PreviousMoves.AddLast(move);
			UpdateAllPiecesValidMoves(resultingBoard, PreviousMoves, CurrentTurn);

			TurnCount++;
			CurrentTurn = CurrentTurn.Complement();
		}

		private static void UpdateAllPiecesValidMoves(Board board, LinkedList<Movement> previousMoves, Side turn) {
			foreach (Piece piece in board.BasePieceList.OfType<Piece>())
				piece.UpdateValidMoves(board, previousMoves, turn);
		}
	}
}