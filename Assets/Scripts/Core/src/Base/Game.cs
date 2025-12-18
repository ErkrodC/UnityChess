using System.Collections.Generic;
using UnityChess.Util;

namespace UnityChess.Core {
	/// <summary>Representation of a standard chess game including a history of moves made.</summary>
	public class Game {
		public Timeline<GameConditions> ConditionsTimeline { get; }
		public Timeline<Board> BoardTimeline { get; }
		public Timeline<HalfMove> HalfMoveTimeline { get; }
		public Timeline<Dictionary<Piece, Dictionary<(Square, Square), Movement>>> LegalMovesTimeline { get; }

		/// <summary>Creates a Game instance of a given mode with a standard starting Board.</summary>
		public Game() : this(GameConditions.NormalStartingConditions, Board.StartingPositionPieces) { }

		public Game(GameConditions startingConditions, params (Square, Piece)[] squarePiecePairs) {
			Board startingBoard = new Board(squarePiecePairs);
			BoardTimeline = new Timeline<Board> { startingBoard };
			HalfMoveTimeline = new Timeline<HalfMove>();
			ConditionsTimeline = new Timeline<GameConditions> { startingConditions };
			LegalMovesTimeline = new Timeline<Dictionary<Piece, Dictionary<(Square, Square), Movement>>> {
				CalculateLegalMovesForPosition(startingBoard, startingConditions)
			};
		}

		/// <summary>Executes passed move and switches sides; also adds move to history.</summary>
		public bool TryExecuteMove(Square start, Square end, out HalfMove latestHalfMove) {
			if (!TryGetLegalMove(start, end, out Movement validatedMove)) {
				latestHalfMove = default;
				return false;
			}

			//create new copy of previous current board, and execute the move on it
			Board boardBeforeMove = BoardTimeline.Head;
			Board resultingBoard = new Board(boardBeforeMove);
			resultingBoard.MovePiece(validatedMove);
			BoardTimeline.AddNext(resultingBoard);

			GameConditions conditionsBeforeMove = ConditionsTimeline.Head;
			Side updatedSideToMove = conditionsBeforeMove.SideToMove.Complement();
			bool causedCheck = Rules.IsPlayerInCheck(resultingBoard, updatedSideToMove);
			bool capturedPiece = boardBeforeMove[validatedMove.End] != null || validatedMove is EnPassantMove;

			latestHalfMove = new HalfMove(boardBeforeMove[validatedMove.Start], validatedMove, capturedPiece, causedCheck);
			GameConditions resultingGameConditions = conditionsBeforeMove.CalculateEndingConditions(boardBeforeMove, latestHalfMove);
			ConditionsTimeline.AddNext(resultingGameConditions);

			Dictionary<Piece, Dictionary<(Square, Square), Movement>> legalMovesByPiece
				= CalculateLegalMovesForPosition(resultingBoard, resultingGameConditions);

			int numLegalMoves = GetNumLegalMoves(legalMovesByPiece);

			LegalMovesTimeline.AddNext(legalMovesByPiece);

			latestHalfMove.SetGameEndBools(
				Rules.IsPlayerStalemated(resultingBoard, updatedSideToMove, numLegalMoves),
				Rules.IsPlayerCheckmated(resultingBoard, updatedSideToMove, numLegalMoves)
			);
			HalfMoveTimeline.AddNext(latestHalfMove);

			return true;
		}

		public bool TryGetLegalMove(Square startSquare, Square endSquare, out Movement move) {
			move = null;

			Board board = BoardTimeline.Head;
			Dictionary<Piece, Dictionary<(Square, Square), Movement>> legalMovesByPiece = LegalMovesTimeline.Head;
			return board != null
			       && legalMovesByPiece != null
			       && board[startSquare] is { } movingPiece
			       && legalMovesByPiece.TryGetValue(movingPiece, out Dictionary<(Square, Square), Movement> movesByStartEndSquares)
			       && movesByStartEndSquares.TryGetValue((startSquare, endSquare), out move);
		}

		public ICollection<Movement> GetLegalMovesForPiece(Piece movingPiece) {
			ICollection<Movement> legalMoves = null;

			Dictionary<Piece, Dictionary<(Square, Square), Movement>> legalMovesByPiece = LegalMovesTimeline.Head;

			if (movingPiece != null
			    && legalMovesByPiece != null
			    && legalMovesByPiece.TryGetValue(movingPiece, out Dictionary<(Square, Square), Movement> movesByStartEndSquares)
			    && movesByStartEndSquares != null
			) {
				legalMoves = movesByStartEndSquares.Values;
			}

			return legalMoves;
		}

		public bool ResetGameToHalfMoveIndex(int halfMoveIndex) {
			if (HalfMoveTimeline.HeadIndex == -1) {
				return false;
			}

			BoardTimeline.HeadIndex = halfMoveIndex + 1;
			ConditionsTimeline.HeadIndex = halfMoveIndex + 1;
			LegalMovesTimeline.HeadIndex = halfMoveIndex + 1;
			HalfMoveTimeline.HeadIndex = halfMoveIndex;

			return true;
		}

		internal static int GetNumLegalMoves(Dictionary<Piece, Dictionary<(Square, Square), Movement>> legalMovesByPiece) {
			int result = 0;

			if (legalMovesByPiece != null) {
				foreach (Dictionary<(Square, Square), Movement> movesByStartEndSquares in legalMovesByPiece.Values) {
					result += movesByStartEndSquares.Count;
				}
			}

			return result;
		}

		internal static Dictionary<Piece, Dictionary<(Square, Square), Movement>> CalculateLegalMovesForPosition(
			Board board,
			GameConditions gameConditions
		) {
			Dictionary<Piece, Dictionary<(Square, Square), Movement>> result = null;

			for (int file = 0; file < 8; file++)
			for (int rank = 0; rank < 8; rank++) {
				if (board[file, rank] is Piece piece
				    && piece.Owner == gameConditions.SideToMove
				    && piece.CalculateLegalMoves(board, gameConditions, new Square(file, rank)) is
					    { } movesByStartEndSquares
				   ) {
					if (result == null) {
						result = new Dictionary<Piece, Dictionary<(Square, Square), Movement>>();
					}

					result[piece] = movesByStartEndSquares;
				}
			}

			return result;
		}
	}
}