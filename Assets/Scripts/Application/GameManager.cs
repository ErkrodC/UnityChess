using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityChess.Core;

namespace UnityChess.Application {
	public class GameManager {
		public event Action<Board> NewGameStarted;
		public event Action<Board> GameEnded;
		public event Action<Timeline<HalfMove>> GameResetToHalfMove;
		public event Action<Board, HalfMove> MoveExecuted;
		public event Action<PromotionInteraction> ElectionRequested;

		// ER TODO probably Move to presentation, SideToMove is not a _game concept
		public Side SideToMove => _game.ConditionsTimeline.Head.SideToMove;

		// ER TODO probably Move to presentation, FullMoveNumber is not a _game concept
		public int FullMoveNumber {
			get {
				Side startingSide = _game.ConditionsTimeline[0].SideToMove;
				int latestHalfMoveIndex = _game.HalfMoveTimeline.HeadIndex;

				return startingSide switch {
					Side.White => latestHalfMoveIndex / 2 + 1,
					Side.Black => (latestHalfMoveIndex + 1) / 2 + 1,
					_ => -1
				};
			}
		}

		private Game _game;
		private FENSerializer _fenSerializer;
		private PGNSerializer _pgnSerializer;
		private GameSerializationType _selectedSerializationType = GameSerializationType.FEN;
		private Dictionary<GameSerializationType, IGameSerializer> _serializersByType;
		private PromotionInteraction _currentPromotionInteraction;
		private IUCIEngine _uciEngine;
		private bool _isWhiteAI;
		private bool _isBlackAI;

		public void Start() {
			_serializersByType = new Dictionary<GameSerializationType, IGameSerializer> {
				[GameSerializationType.FEN] = new FENSerializer(),
				[GameSerializationType.PGN] = new PGNSerializer()
			};

			StartNewGame();
		}

		private void OnDestroy() {
			_uciEngine?.ShutDown();
		}

#if AI_TEST
		public async void StartNewGame(bool isWhiteAI = true, bool isBlackAI = true) {
#else
		public async void StartNewGame(bool isWhiteAI = false, bool isBlackAI = false) {
#endif
			_game = new Game();
			_isWhiteAI = isWhiteAI;
			_isBlackAI = isBlackAI;

			if (isWhiteAI || isBlackAI) {
				if (_uciEngine == null) {
					_uciEngine = new MockUCIEngine();
					_uciEngine.Start();
				}

				await _uciEngine.SetupNewGame(_game);
				NewGameStarted?.Invoke(_game.BoardTimeline.Head);

				if (isWhiteAI) {
					Movement bestMove = await _uciEngine.GetBestMove(10_000);
					DoAIMove(bestMove);
				}
			} else {
				NewGameStarted?.Invoke(_game.BoardTimeline.Head);
			}
		}

		public string SerializeGame() {
			return _serializersByType.TryGetValue(_selectedSerializationType, out IGameSerializer serializer)
				? serializer?.Serialize(_game)
				: null;
		}

		public void LoadGame(string serializedGame) {
			_game = _serializersByType[_selectedSerializationType].Deserialize(serializedGame);
			NewGameStarted?.Invoke(_game.BoardTimeline.Head);
		}

		public void ResetGameToHalfMoveIndex(int halfMoveIndex) {
			if (!_game.ResetGameToHalfMoveIndex(halfMoveIndex)) { return; }

			_currentPromotionInteraction?.TryCancel();
			GameResetToHalfMove?.Invoke(_game.HalfMoveTimeline);
		}

		public async Task<bool> TryExecuteMoveAsync(Square startSquare, Square endSquare) {
			if (!_game.TryGetLegalMove(startSquare, endSquare, out Movement move)) {
				return false;
			}

			if (move is PromotionMove promotionMove) {
				bool promotionReady = await ElectPieceAsync(promotionMove);
				if (!promotionReady) { return false; }
			}

			if (!_game.TryExecuteMove(startSquare, endSquare, out HalfMove latestHalfMove)) {
				return false;
			}

			MoveExecuted?.Invoke(_game.BoardTimeline.Head, latestHalfMove);

			if (latestHalfMove.CausedCheckmate || latestHalfMove.CausedStalemate) {
				GameEnded?.Invoke(_game.BoardTimeline.Head);
			}

			return true;

			// ER TODO do AI
			/*bool gameIsOver = _game.HalfMoveTimeline.TryGetCurrent(out HalfMove lastHalfMove)
				&& lastHalfMove.CausedStalemate || lastHalfMove.CausedCheckmate;
			if (!gameIsOver
			    && (SideToMove == Side.White && _isWhiteAI
			        || SideToMove == Side.Black && _isBlackAI)
			   ) {
				Movement bestMove = await _uciEngine.GetBestMove(10_000);
				DoAIMove(bestMove);
			}*/
		}

		private async Task<bool> ElectPieceAsync(PromotionMove moveNeedingPiece) {
			_currentPromotionInteraction?.TryCancel();
			_currentPromotionInteraction?.Dispose();

			using PromotionInteraction promotionInteraction = new(moveNeedingPiece);
			_currentPromotionInteraction = promotionInteraction;

			ElectionRequested?.Invoke(promotionInteraction);

			try {
				ElectedPiece choice = await promotionInteraction.Task;
				moveNeedingPiece.SetPromotionPiece(PromotionUtil.GeneratePromotionPiece(choice, SideToMove));
				return true;
			} catch (OperationCanceledException) {
				return false;
			} finally {
				if (ReferenceEquals(_currentPromotionInteraction, promotionInteraction)) {
					_currentPromotionInteraction = null;
				}
			}
		}

		private void DoAIMove(Movement move) {
			// ER TODO remove, app layer should not know about presentation
			/*GameObject movedPiece = BoardManager.Instance.GetPieceGOAtPosition(Move.Start);
			GameObject endSquareGO = BoardManager.Instance.GetSquareGOByPosition(Move.End);
			OnPieceMoved(
				Move.Start,
				movedPiece.transform,
				endSquareGO.transform,
				(Move as PromotionMove)?.PromotionPiece
			);*/
		}
	}
}