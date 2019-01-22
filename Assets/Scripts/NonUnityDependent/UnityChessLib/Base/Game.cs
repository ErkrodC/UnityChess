namespace UnityChess {
	/// <summary>Representation of a standard chess game including a history of moves made.</summary>
	public class Game {
		public Mode Mode { get; }
		public Side CurrentTurnSide { get; private set; }
		public int HalfMoveCount => PreviousMoves.HeadIndex;
		public GameConditions StartingConditions { get; }
		public History<Board> BoardHistory { get; }
		public History<HalfMove> PreviousMoves { get; }

		/// <summary>Creates a Game instance of a given mode with a standard starting Board.</summary>
		/// <param name="mode">Describes which players are human or AI.</param>
		/// <param name="startingConditions">Conditions at the time the board was set up.</param>
		public Game(Mode mode, GameConditions startingConditions) {
			Mode = mode;
			CurrentTurnSide = Side.White;
			StartingConditions = startingConditions;
			BoardHistory = new History<Board>();
			PreviousMoves = new History<HalfMove>();

			BoardHistory.AddLast(new Board());
			UpdateAllPiecesValidMoves(BoardHistory.Last, PreviousMoves, Side.White);
		}

		private Board LatestBoard => BoardHistory.Last;
		
		/// <summary>Executes passed move and switches sides; also adds move to history.</summary>
		public void ExecuteTurn(Movement move) {
			//create new copy of previous current board, and execute the move on it
			Board boardBeforeMove = LatestBoard;
			Board resultingBoard = new Board(LatestBoard);
			resultingBoard.MovePiece(move);

			BoardHistory.AddLast(resultingBoard);

			CurrentTurnSide = CurrentTurnSide.Complement();
			UpdateAllPiecesValidMoves(resultingBoard, PreviousMoves, CurrentTurnSide);
			
			bool causedCheckmate = Rules.IsPlayerCheckmated(resultingBoard, CurrentTurnSide);
			bool causedStalemate = Rules.IsPlayerStalemated(resultingBoard, CurrentTurnSide);
			bool causedCheck = Rules.IsPlayerInCheck(resultingBoard, CurrentTurnSide) && !causedCheckmate;
			PreviousMoves.AddLast(new HalfMove(boardBeforeMove[move.Start], move, boardBeforeMove[move.End] != null, causedCheck, causedStalemate , causedCheckmate));
		}

		/// <summary>Checks whether a move is legal on a given board/turn.</summary>
		/// <param name="baseMove">The base move used to search in the appropriate piece's ValidMovesList.</param>
		/// <param name="foundLegalMove">The move (potentially a special move) found in the pieces ValidMovesList that corresponds to the passed in baseMove</param>
		public bool MoveIsLegal(Movement baseMove, out Movement foundLegalMove) {
			Piece movingPiece = LatestBoard[baseMove.Start];
			if (movingPiece == null) {
				foundLegalMove = null;
				return false;
			}

			bool actualMoveFound = movingPiece.LegalMoves.FindLegalMoveUsingBaseMove(baseMove, out Movement foundMove);
			foundLegalMove = foundMove;
			return movingPiece.Color == CurrentTurnSide && actualMoveFound;
		}

		public void ResetGameToHalfMoveIndex(int halfMoveIndex) {
			BoardHistory.HeadIndex = halfMoveIndex + 1;
			PreviousMoves.HeadIndex = halfMoveIndex;
			CurrentTurnSide = halfMoveIndex % 2 == 0 ? Side.Black : Side.White;
			
			UpdateAllPiecesValidMoves(BoardHistory.Last, PreviousMoves, CurrentTurnSide);
		}

		private static void UpdateAllPiecesValidMoves(Board board, History<HalfMove> previousMoves, Side turn) {
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