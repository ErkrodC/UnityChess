using System.Collections.Generic;

namespace UnityChess {
	/// <summary>Representation of a standard chess game including a history of moves made.</summary>
	public class Game {
		/// <summary>Creates a Game instance of a given mode with a standard starting Board.</summary>
		/// <param name="mode">Describes which players are human or AI.</param>
		public Game(Mode mode) {
			CurrentTurnSide = Side.White;
			TurnCount = 0;
			Mode = mode;
			BoardList = new LinkedList<Board>();
			BoardList.AddLast(new Board());
			PreviousMoves = new LinkedList<Turn>();

			UpdateAllPiecesValidMoves(BoardList.Last.Value, PreviousMoves, Side.White);
		}

		public Side CurrentTurnSide { get; private set; }
		public int TurnCount { get; set; }
		public Mode Mode { get; }
		public LinkedList<Board> BoardList { get; }
		public LinkedList<Turn> PreviousMoves { get; }

		private Board LatestBoard => BoardList.Last.Value;
		
		/// <summary>Executes passed move and switches sides; also adds move to history.</summary>
		public void ExecuteTurn(Movement move) {
			//create new copy of previous current board, and execute the move on it
			PreviousMoves.AddLast(new Turn(LatestBoard[move.Start], move));
			Board resultingBoard = new Board(LatestBoard);
			resultingBoard.MovePiece(move);

			BoardList.AddLast(resultingBoard);

			TurnCount++;
			CurrentTurnSide = CurrentTurnSide.Complement();
			UpdateAllPiecesValidMoves(resultingBoard, PreviousMoves, CurrentTurnSide);
		}

		/// <summary>Checks whether a move is legal on a given board/turn.</summary>
		/// <param name="baseMove">The base move used to search in the appropriate piece's ValidMovesList.</param>
		/// <param name="foundValidMove">The move (potentially a special move) found in the pieces ValidMovesList that corresponds to the passed in baseMove</param>
		public bool MoveIsLegal(Movement baseMove, out Movement foundValidMove) {
			Piece movingPiece = LatestBoard[baseMove.Start];
			if (movingPiece == null) {
				foundValidMove = null;
				return false;
			}

			bool actualMoveFound = movingPiece.LegalMoves.FindLegalMoveUsingBaseMove(baseMove, out Movement foundMove);
			foundValidMove = foundMove;
			return movingPiece.Color == CurrentTurnSide && actualMoveFound;
		}

		public static void UpdateAllPiecesValidMoves(Board board, LinkedList<Turn> previousMoves, Side turn) {
			for (int file = 1; file <= 8; file++)
				for (int rank = 1; rank <= 8; rank++) {
					Piece piece = board[file, rank];
					if (piece == null) continue;
					
					if (piece.Color == turn) piece.UpdateValidMoves(board, previousMoves);
					else piece.LegalMoves.Clear();
				}
		}
	}
}