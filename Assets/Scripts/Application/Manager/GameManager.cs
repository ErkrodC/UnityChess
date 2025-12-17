using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityChess.Core;
using UnityChess.DependencyInjection;
using UnityChess.Util;

namespace UnityChess.Application {
	public class GameManager : IManager {
		public event Action<Board> newGameStarted;
		public event Action<Board> gameEnded;
		public event Action<Board, Timeline<HalfMove>> gameResetToHalfMove;
		public event Action<Board, Timeline<HalfMove>> moveExecuted;
		public event Action<PromotionInteraction> electionRequested;
		public int currentHalfMoveIndex => _game.HalfMoveTimeline.HeadIndex;
		public int halfMoveTimelineCount => _game.HalfMoveTimeline.Count;

		private Side _sideToMove => _game.ConditionsTimeline.Head.SideToMove;
		private Game _game;
		private FENSerializer _fenSerializer;
		private PGNSerializer _pgnSerializer;
		private readonly GameSerializationType _selectedSerializationType = GameSerializationType.FEN;
		private readonly Dictionary<GameSerializationType, IGameSerializer> _serializersByType;
		private PromotionInteraction _currentPromotionInteraction;
		private IUCIEngine _uciEngine;
		private bool _isWhiteAI;
		private bool _isBlackAI;

		public GameManager() {
			_serializersByType = new Dictionary<GameSerializationType, IGameSerializer> {
				[GameSerializationType.FEN] = new FENSerializer(),
				[GameSerializationType.PGN] = new PGNSerializer()
			};
		}

		~GameManager() {
			_uciEngine?.ShutDown();
		}

#if AI_TEST
		public async Task StartNewGame(bool isWhiteAI = true, bool isBlackAI = true) {
#else
		public async Task StartNewGame(bool isWhiteAI = false, bool isBlackAI = false) {
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
				newGameStarted?.Invoke(_game.BoardTimeline.Head);

				if (isWhiteAI) {
					Movement bestMove = await _uciEngine.GetBestMove(10_000);
					DoAIMove(bestMove);
				}
			} else {
				newGameStarted?.Invoke(_game.BoardTimeline.Head);
			}
		}

		public string SerializeGame() {
			return _serializersByType.TryGetValue(_selectedSerializationType, out IGameSerializer serializer)
				? serializer?.Serialize(_game)
				: null;
		}

		public void LoadGame(string serializedGame) {
			_game = _serializersByType[_selectedSerializationType].Deserialize(serializedGame);
			newGameStarted?.Invoke(_game.BoardTimeline.Head);
		}

		public void ResetGameToHalfMoveIndex(int halfMoveIndex) {
			if (!_game.ResetGameToHalfMoveIndex(halfMoveIndex)) { return; }

			_currentPromotionInteraction?.TryCancel();
			gameResetToHalfMove?.Invoke(_game.BoardTimeline.Head, _game.HalfMoveTimeline);
		}

		public async Task<bool> TryExecuteMoveAsync(Square startSquare, Square endSquare) {
			if (!_game.TryGetLegalMove(startSquare, endSquare, out Movement move)) {
				return false;
			}

			if (move is PromotionMove promotionMove) {
				bool promotionReady = await ElectPieceAsync(_sideToMove, promotionMove);
				if (!promotionReady) { return false; }
			}

			if (!_game.TryExecuteMove(startSquare, endSquare, out HalfMove latestHalfMove)) {
				return false;
			}

			moveExecuted?.Invoke(_game.BoardTimeline.Head, _game.HalfMoveTimeline);

			if (latestHalfMove.CausedCheckmate || latestHalfMove.CausedStalemate) {
				gameEnded?.Invoke(_game.BoardTimeline.Head);
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

		private async Task<bool> ElectPieceAsync(Side requestingSide, PromotionMove moveNeedingPiece) {
			_currentPromotionInteraction?.TryCancel();
			_currentPromotionInteraction?.Dispose();

			using PromotionInteraction promotionInteraction = new(requestingSide);
			_currentPromotionInteraction = promotionInteraction;

			electionRequested?.Invoke(promotionInteraction);

			try {
				ElectedPiece choice = await promotionInteraction.task;
				moveNeedingPiece.SetPromotionPiece(PromotionUtil.GeneratePromotionPiece(choice, _sideToMove));
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