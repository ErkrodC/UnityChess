using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityChess.Application.Service;
using UnityChess.Core;
using UnityChess.DependencyInjection;
using UnityChess.Core.Util;

namespace UnityChess.Application {
	public class GameManager : IManager, IUpdateable {
		public event Action<Board> newGameStarted;
		public event Action<Board> gameEnded;
		public event Action<Board, Timeline<HalfMove>> gameResetToHalfMove;
		public event Action<Board, Timeline<HalfMove>> moveExecuted;
		public int currentHalfMoveIndex => _game.HalfMoveTimeline.HeadIndex;
		public int halfMoveTimelineCount => _game.HalfMoveTimeline.Count;

		private Side _sideToMove => _game.ConditionsTimeline.Head.SideToMove;
		private IPlayerService _currentPlayer => _sideToMove == Side.White ? _whitePlayer : _blackPlayer;
		private IPlayerService _whitePlayer;
		private IPlayerService _blackPlayer;
		private readonly Dictionary<GameSerializationType, IGameSerializer> _serializersByType;
		private Game _game;
		private bool _isGameRunning;
		private bool _moveRequestPending;
		private GameSerializationType _selectedSerializationType = GameSerializationType.FEN;
		private readonly FENSerializer _fenSerializer = new();
		private PGNSerializer _pgnSerializer;

		public GameManager() {
			_serializersByType = new Dictionary<GameSerializationType, IGameSerializer> {
				[GameSerializationType.FEN] = new FENSerializer(),
				[GameSerializationType.PGN] = new PGNSerializer()
			};
		}

		public void Update(float deltaTime) {
			if (!_isGameRunning) { return; }

			if (!_moveRequestPending) { RequestNextMove(); }
		}

		public void StartNewGame(IPlayerService whitePlayer, IPlayerService blackPlayer) {
			_whitePlayer = whitePlayer;
			_blackPlayer = blackPlayer;

			_game = new Game();
			_isGameRunning = true;
			newGameStarted?.Invoke(_game.BoardTimeline.Head);
		}

		private async void RequestNextMove() {
			_moveRequestPending = true;
			bool moveWasValid = false;

			try {
				(Square start, Square end) = await _currentPlayer.GetMoveAsync(_fenSerializer.Serialize(_game));
				moveWasValid = await TryExecuteMoveAsync(start, end);
				if (!moveWasValid) {
					// ER TODO handle illegal move from player if necessary (e.g. a cheating networked player)
				}
			} finally {
				_currentPlayer.ReportMoveValidity(moveWasValid);
				_moveRequestPending = false;
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
			if (_game.ResetGameToHalfMoveIndex(halfMoveIndex)) {
				gameResetToHalfMove?.Invoke(_game.BoardTimeline.Head, _game.HalfMoveTimeline);
			}
		}

		private async Task<bool> TryExecuteMoveAsync(Square startSquare, Square endSquare) {
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
		}

		private async Task<bool> ElectPieceAsync(Side requestingSide, PromotionMove moveNeedingPiece) {
			try {
				ElectedPiece choice = await _currentPlayer.ElectPieceAsync(requestingSide);
				moveNeedingPiece.SetPromotionPiece(PromotionUtil.GeneratePromotionPiece(choice, _sideToMove));
				return true;
			} catch (OperationCanceledException) {
				return false;
			}
		}
	}
}