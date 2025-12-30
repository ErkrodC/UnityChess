using System;
using System.Threading.Tasks;
using UnityChess.Application.Service;
using UnityChess.Core;
using UnityChess.Core.Util;
using UnityChess.DependencyInjection;

namespace UnityChess.Application {
	public class GameManager : IManager, IUpdateable {
		public event Action<Board> newGameStarted;
		public event Action<Board> gameEnded;
		public event Action<Board, Timeline<HalfMove>> gameResetToHalfMove;
		public event Action<Board, Timeline<HalfMove>> moveExecuted;

		private IPlayerService _whitePlayer;
		private IPlayerService _blackPlayer;
		private Game _game;
		private bool _isGameRunning;
		private bool _moveRequestPending;
		private readonly FENSerializer _fenSerializer = new();

		public int GetCurrentHalfMoveIndex() => _game.HalfMoveTimeline.HeadIndex;
		public int GetHalfMoveTimelineCount() => _game.HalfMoveTimeline.Count;
		private Side GetCurrentSideToMove() => _game.ConditionsTimeline.Head.SideToMove;
		private IPlayerService GetCurrentPlayer() => GetCurrentSideToMove() == Side.White ? _whitePlayer : _blackPlayer;

		public void Update(float deltaTime) {
			if (!_isGameRunning || _moveRequestPending) { return; }

			AwaitTurnAsync();
		}

		public void StartNewGame(IPlayerService whitePlayer, IPlayerService blackPlayer, Game game = null) {
			_whitePlayer = whitePlayer;
			_blackPlayer = blackPlayer;

			_game = game ?? new Game();
			_isGameRunning = true;
			newGameStarted?.Invoke(_game.BoardTimeline.Head);
		}

		private async void AwaitTurnAsync() {
			_moveRequestPending = true;
			try {
				await HandleMoveInteraction();
			} finally {
				_moveRequestPending = false;
			}

			return;

			async Task HandleMoveInteraction() {
				bool moveWasValid = false;
				IPlayerService currentPlayer = GetCurrentPlayer();
				try {
					(Square start, Square end) = await currentPlayer.GetMoveAsync(_fenSerializer.Serialize(_game));
					moveWasValid = await TryExecuteMoveAsync(start, end);
					if (!moveWasValid) {
						// ER TODO handle illegal move from player if necessary (e.g. a cheating networked player)
					}
				} catch (OperationCanceledException) {
				} finally {
					currentPlayer.ReportMoveValidity(moveWasValid);
				}
			}
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
				bool promotionReady = await ElectPieceAsync(GetCurrentSideToMove(), promotionMove);
				if (!promotionReady) { return false; }
			}

			if (!_game.TryExecuteMove(startSquare, endSquare, out HalfMove latestHalfMove)) {
				return false;
			}

			moveExecuted?.Invoke(_game.BoardTimeline.Head, _game.HalfMoveTimeline);

			if (latestHalfMove.CausedCheckmate || latestHalfMove.CausedStalemate) {
				gameEnded?.Invoke(_game.BoardTimeline.Head);
				_isGameRunning = false;
			}

			return true;
		}

		private async Task<bool> ElectPieceAsync(Side requestingSide, PromotionMove moveNeedingPiece) {
			try {
				ElectedPiece choice = await GetCurrentPlayer().ElectPieceAsync(requestingSide);
				moveNeedingPiece.SetPromotionPiece(PromotionUtil.GeneratePromotionPiece(choice, GetCurrentSideToMove()));
				return true;
			} catch (OperationCanceledException) {
				return false;
			}
		}
	}
}