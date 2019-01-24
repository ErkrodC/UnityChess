using System;

namespace UnityChess {
	/// <summary>Representation of a standard chess game including a history of moves made.</summary>
	public class Game {
		public Mode Mode { get; }
		public Side CurrentTurnSide { get; private set; }
		public int HalfMoveCount => PreviousMoves.HeadIndex;
		public GameConditions StartingConditions { get; }
		public Timeline<Board> BoardTimeline { get; }
		public Timeline<HalfMove> PreviousMoves { get; }

		private readonly Timeline<Square> enPassantCaptureSquareTimeline;

		/// <summary>Creates a Game instance of a given mode with a standard starting Board.</summary>
		/// <param name="mode">Describes which players are human or AI.</param>
		/// <param name="startingConditions">Conditions at the time the board was set up.</param>
		public Game(Mode mode, GameConditions startingConditions) {
			Mode = mode;
			CurrentTurnSide = Side.White;
			StartingConditions = startingConditions;
			BoardTimeline = new Timeline<Board>();
			PreviousMoves = new Timeline<HalfMove>();
			enPassantCaptureSquareTimeline = new Timeline<Square>();
			
			BoardTimeline.AddNext(new Board());
			enPassantCaptureSquareTimeline.AddNext(Square.Invalid);
			UpdateAllPiecesLegalMoves(BoardTimeline.Current, enPassantCaptureSquareTimeline.Current, Side.White);
		}

		private Board LatestBoard => BoardTimeline.Current;
		
		/// <summary>Executes passed move and switches sides; also adds move to history.</summary>
		public void ExecuteTurn(Movement move) {
			//create new copy of previous current board, and execute the move on it
			Board boardBeforeMove = LatestBoard;
			Square enPassantEligibleSquare = boardBeforeMove[move.Start] is Pawn pawn && Math.Abs(move.End.Rank - move.Start.Rank) == 2 ?
				                          new Square(move.End, 0, pawn.Color == Side.White ? -1 : 1) :
				                          Square.Invalid;
			enPassantCaptureSquareTimeline.AddNext(enPassantEligibleSquare);

			Board resultingBoard = new Board(LatestBoard);
			resultingBoard.MovePiece(move);

			BoardTimeline.AddNext(resultingBoard);

			CurrentTurnSide = CurrentTurnSide.Complement();
			
			UpdateAllPiecesLegalMoves(resultingBoard, enPassantCaptureSquareTimeline.Current, CurrentTurnSide);

			bool capturedPiece = boardBeforeMove[move.End] != null || move is EnPassantMove;
			bool causedCheckmate = Rules.IsPlayerCheckmated(resultingBoard, CurrentTurnSide);
			bool causedStalemate = Rules.IsPlayerStalemated(resultingBoard, CurrentTurnSide);
			bool causedCheck = Rules.IsPlayerInCheck(resultingBoard, CurrentTurnSide) && !causedCheckmate;
			PreviousMoves.AddNext(new HalfMove(boardBeforeMove[move.Start], move, capturedPiece, causedCheck, causedStalemate , causedCheckmate));
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
			BoardTimeline.HeadIndex = halfMoveIndex + 1;
			enPassantCaptureSquareTimeline.HeadIndex = halfMoveIndex + 1;
			PreviousMoves.HeadIndex = halfMoveIndex;
			CurrentTurnSide = halfMoveIndex % 2 == 0 ? Side.Black : Side.White;
			
			UpdateAllPiecesLegalMoves(BoardTimeline.Current, enPassantCaptureSquareTimeline.Current, CurrentTurnSide);
		}

		private static void UpdateAllPiecesLegalMoves(Board board, Square enPassantEligibleSquare, Side turn) {
			for (int file = 1; file <= 8; file++)
				for (int rank = 1; rank <= 8; rank++) {
					Piece piece = board[file, rank];
					if (piece == null) continue;
						if (piece.Color == turn) piece.UpdateLegalMoves(board, enPassantEligibleSquare);
						else piece.LegalMoves.Clear();
				}
		}
	}
}