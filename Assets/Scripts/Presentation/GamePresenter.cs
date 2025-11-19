using UnityChess.Application;
using UnityChess.Core;
using UnityChess.Presentation.ViewModel;

namespace UnityChess.Presentation {
	public class GamePresenter {
		private readonly GameVM _gameVM;
		private readonly GameManager _gameManager;

		public GamePresenter(GameManager gameManager, GameVM gameVM) {
			_gameManager = gameManager;
			_gameVM = gameVM;

			// From Application Layer To Presentation Layer
			_gameManager.newGameStarted += OnNewGameStarted;
			_gameManager.boardChanged += OnBoardChanged;
			_gameManager.turnChanged += OnTurnChanged;
			_gameManager.moveExecuted += OnMoveExecuted;
			_gameManager.gameResetToHalfMove += OnGameResetToHalfMove;
			_gameManager.gameEnded += OnGameEnded;

			// From Presentation Layer To Application Layer
			_gameVM.boardVM.onSquareClicked = OnSquareClicked;
			_gameVM.moveHistoryVM.onToBeginningClicked = OnToBeginningClicked;
			_gameVM.moveHistoryVM.onBackClicked = OnBackClicked;
			_gameVM.moveHistoryVM.onForwardClicked = OnForwardClicked;
			_gameVM.moveHistoryVM.onToEndClicked = OnToEndClicked;
			_gameVM.moveHistoryVM.onMoveClicked = OnMoveClicked;
			_gameVM.menuVM.onStartNewGameClicked = OnStartNewGameClicked;
			_gameVM.menuVM.onLoadFenClicked = OnLoadFenClicked;
		}

		#region Application Layer Event Handlers

		private void OnNewGameStarted(Board board) {
			PieceVM[,] pieceVMs = new PieceVM[8, 8];
			ConvertBoardToPieceTypes(board, pieceVMs);

			_gameVM.boardVM.currentBoard = pieceVMs;
			_gameVM.moveHistoryVM.moveEntries.Clear();
			_gameVM.moveHistoryVM.currentHalfMoveIndex = -1;
			_gameVM.menuVM.gameResult = string.Empty;
		}

		private void OnBoardChanged(Board board) {
			ConvertBoardToPieceTypes(board, _gameVM.boardVM.currentBoard);
		}

		private void OnTurnChanged(Side sideToMove) {
			_gameVM.boardVM.currentSideToMove = sideToMove;
		}

		private void OnMoveExecuted(HalfMove halfMove) {
			int halfMoveIndex = _gameVM.moveHistoryVM.currentHalfMoveIndex + 1;
			_gameVM.moveHistoryVM.currentHalfMoveIndex = halfMoveIndex;

			// White's move (even half-move index)
			if (halfMoveIndex % 2 == 0) {
				_gameVM.moveHistoryVM.moveEntries.Add(new MoveHistoryEntryVM {
					moveNumber = halfMoveIndex / 2 + 1,
					whiteMove = halfMove.ToAlgebraicNotation(),
					blackMove = null
				});
			} else { // Black's move (odd half-move index)
				var lastEntry = _gameVM.moveHistoryVM.moveEntries[^1];
				lastEntry.blackMove = halfMove.ToAlgebraicNotation();
			}
		}

		private void OnGameResetToHalfMove(Timeline<HalfMove> halfMoveTimeline) {
			// ER TODO update relevant view models with new half move timeline
			/*_gameVM.boardVM.currentBoard = board;
				_gameVM.boardVM.currentSideToMove = sideToMove;*/

			// ER TODO convert board to fen string
			//_gameVM.menuVM.fenString = currentFEN;
		}

		private void OnGameEnded(Board board) {
			// ER TODO convert board to fen string and set game result
			//_gameVM.menuVM.fenString = currentFEN;
			//_gameVM.menuVM.gameResult = result;
		}

		#endregion

		#region Presentation Layer Event Handlers

		private void OnSquareClicked(Square square) {
			// ER TODO: Implement square click logic - select piece, move piece, etc.
		}

		// Move history commands

		private void OnToBeginningClicked() {
			// ER TODO: Reset to beginning of game
		}

		private void OnBackClicked() {
			// ER TODO: Go back one move
		}

		private void OnForwardClicked() {
			// ER TODO: Go forward one move
		}

		private void OnToEndClicked() {
			// ER TODO: Go to end of game
		}

		private void OnMoveClicked(int halfMoveIndex) {
			// ER TODO: Jump to specific move
		}

		// Menu commands

		private void OnStartNewGameClicked() {
			_gameManager.StartNewGame();
		}

		private void OnLoadFenClicked(string fen) {
			// ER TODO: Load FEN string
		}

		#endregion

		#region Helpers

		private static void ConvertBoardToPieceTypes(Board board, PieceVM[,] boardPieceTypes) {
			for (int file = 1; file <= 8; file++)
			for (int rank = 1; rank <= 8; rank++) {
				Piece piece = board[file, rank];

				if (piece == null) {
					boardPieceTypes[file - 1, rank - 1] = null;
					continue;
				}

				PieceType pieceType = piece switch {
					Pawn => PieceType.Pawn,
					Rook => PieceType.Rook,
					Knight => PieceType.Knight,
					Bishop => PieceType.Bishop,
					Queen => PieceType.Queen,
					King => PieceType.King,
					_ => throw new System.ArgumentException($"Unknown piece type: {piece.GetType().Name}")
				};

				boardPieceTypes[file - 1, rank - 1] = new PieceVM {
					type = pieceType,
					side = piece.Owner
				};
			}
		}

		#endregion
	}
}