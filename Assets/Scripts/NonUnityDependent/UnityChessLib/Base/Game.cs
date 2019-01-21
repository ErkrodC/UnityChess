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
			BoardHistory = new History<Board>();
			BoardHistory.AddLast(new Board());
			PreviousMoves = new History<Turn>();

			UpdateAllPiecesValidMoves(BoardHistory.Last, PreviousMoves, Side.White);
		}

		public Side CurrentTurnSide { get; private set; }
		public int TurnCount { get; private set; }
		public Mode Mode { get; }
		public History<Board> BoardHistory { get; }
		public History<Turn> PreviousMoves { get; }

		private Board LatestBoard => BoardHistory.Last;
		
		/// <summary>Executes passed move and switches sides; also adds move to history.</summary>
		public void ExecuteTurn(Movement move) {
			//create new copy of previous current board, and execute the move on it
			Board boardBeforeMove = LatestBoard;
			Board resultingBoard = new Board(LatestBoard);
			resultingBoard.MovePiece(move);

			BoardHistory.AddLast(resultingBoard);

			TurnCount++;
			CurrentTurnSide = CurrentTurnSide.Complement();
			UpdateAllPiecesValidMoves(resultingBoard, PreviousMoves, CurrentTurnSide);
			
			bool causedCheckmate = Rules.IsPlayerCheckmated(resultingBoard, CurrentTurnSide);
			bool causedStalemate = Rules.IsPlayerStalemated(resultingBoard, CurrentTurnSide);
			bool causedCheck = Rules.IsPlayerInCheck(resultingBoard, CurrentTurnSide) && !causedCheckmate;
			PreviousMoves.AddLast(new Turn(boardBeforeMove[move.Start], move, boardBeforeMove[move.End] != null, causedCheck, causedStalemate , causedCheckmate));
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

		public static void UpdateAllPiecesValidMoves(Board board, History<Turn> previousMoves, Side turn) {
			for (int file = 1; file <= 8; file++)
				for (int rank = 1; rank <= 8; rank++) {
					Piece piece = board[file, rank];
					if (piece == null) continue;
					
					if (piece.Color == turn) piece.UpdateValidMoves(board, previousMoves);
					else piece.LegalMoves.Clear();
				}
		}

		public void ResetGameToTurn(int turnIndex) {
			BoardHistory.HeadIndex = turnIndex + 1;
			PreviousMoves.HeadIndex = turnIndex;
			TurnCount = turnIndex;
			CurrentTurnSide = TurnCount % 2 == 0 ? Side.Black : Side.White;
			
			UpdateAllPiecesValidMoves(BoardHistory.Last, PreviousMoves, CurrentTurnSide);
		}
	}
}