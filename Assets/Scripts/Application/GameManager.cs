using System;
using System.Collections.Generic;
using System.Threading;
using UnityChess.Core;

namespace UnityChess.Application {
	public class GameManager {
		public event Action<Board> newGameStarted;
		public event Action<Board> gameEnded;
		public event Action<Timeline<HalfMove>> gameResetToHalfMove;
		public event Action<Board, HalfMove> moveExecuted;

		// ER TODO probably move to presentation, SideToMove is not a game concept
		public Side SideToMove => game.ConditionsTimeline.Head.SideToMove;

		// ER TODO probably move to presentation, FullMoveNumber is not a game concept
		public int FullMoveNumber {
			get {
				Side startingSide = game.ConditionsTimeline[0].SideToMove;
				int latestHalfMoveIndex = game.HalfMoveTimeline.HeadIndex;

				return startingSide switch {
					Side.White => latestHalfMoveIndex / 2 + 1,
					Side.Black => (latestHalfMoveIndex + 1) / 2 + 1,
					_ => -1
				};
			}
		}

		private Game game;

		private FENSerializer fenSerializer;
		private PGNSerializer pgnSerializer;
		private GameSerializationType selectedSerializationType = GameSerializationType.FEN;
		private Dictionary<GameSerializationType, IGameSerializer> serializersByType;

		private CancellationTokenSource promotionUITaskCancellationTokenSource;

		private IUCIEngine uciEngine;
		private bool isWhiteAI;
		private bool isBlackAI;

		public void Start() {
			serializersByType = new Dictionary<GameSerializationType, IGameSerializer> {
				[GameSerializationType.FEN] = new FENSerializer(),
				[GameSerializationType.PGN] = new PGNSerializer()
			};

			StartNewGame();
		}

		private void OnDestroy() {
			uciEngine?.ShutDown();
		}

#if AI_TEST
	public async void StartNewGame(bool isWhiteAI = true, bool isBlackAI = true) {
#else
		public async void StartNewGame(bool isWhiteAI = false, bool isBlackAI = false) {
#endif
			game = new Game();

			this.isWhiteAI = isWhiteAI;
			this.isBlackAI = isBlackAI;

			if (isWhiteAI || isBlackAI) {
				if (uciEngine == null) {
					uciEngine = new MockUCIEngine();
					uciEngine.Start();
				}

				await uciEngine.SetupNewGame(game);
				newGameStarted?.Invoke(game.BoardTimeline.Head);

				if (isWhiteAI) {
					Movement bestMove = await uciEngine.GetBestMove(10_000);
					DoAIMove(bestMove);
				}
			} else {
				newGameStarted?.Invoke(game.BoardTimeline.Head);
			}
		}

		public string SerializeGame() {
			return serializersByType.TryGetValue(selectedSerializationType, out IGameSerializer serializer)
				? serializer?.Serialize(game)
				: null;
		}

		public void LoadGame(string serializedGame) {
			game = serializersByType[selectedSerializationType].Deserialize(serializedGame);
			newGameStarted?.Invoke(game.BoardTimeline.Head);
		}

		public void ResetGameToHalfMoveIndex(int halfMoveIndex) {
			if (!game.ResetGameToHalfMoveIndex(halfMoveIndex)) { return; }

			promotionUITaskCancellationTokenSource?.Cancel();
			gameResetToHalfMove?.Invoke(game.HalfMoveTimeline);
		}

		public bool TryExecuteMove(Square movedPieceInitialSquare, Square endSquare) {
			if (!game.TryGetLegalMove(movedPieceInitialSquare, endSquare, out Movement move)) {
				return false;
			}

			if (move is PromotionMove promotionMove) {
				// ER TODO raise event to get promotion piece choice, and await it
				promotionUITaskCancellationTokenSource?.Cancel();
				promotionUITaskCancellationTokenSource = new CancellationTokenSource();

				// ER TODO
				/*ElectedPiece choice = await Task.Run(GetUserPromotionPieceChoice, promotionUITaskCancellationTokenSource.Token);*/
				if (promotionUITaskCancellationTokenSource == null
				    || promotionUITaskCancellationTokenSource.Token.IsCancellationRequested
				   ) {
					return false;
				}

				// ER TODO
				/*promotionMove.SetPromotionPiece(
						PromotionUtil.GeneratePromotionPiece(choice, SideToMove)
					);*/
				promotionUITaskCancellationTokenSource = null;
			}

			if (!game.TryExecuteMove(movedPieceInitialSquare, endSquare, out HalfMove latestHalfMove)) {
				return false;
			}

			moveExecuted?.Invoke(game.BoardTimeline.Head, latestHalfMove);

			if (latestHalfMove.CausedCheckmate || latestHalfMove.CausedStalemate) {
				gameEnded?.Invoke(game.BoardTimeline.Head);
			}


			return true;

			// ER TODO do AI
			/*bool gameIsOver = game.HalfMoveTimeline.TryGetCurrent(out HalfMove lastHalfMove)
				&& lastHalfMove.CausedStalemate || lastHalfMove.CausedCheckmate;
			if (!gameIsOver
			    && (SideToMove == Side.White && isWhiteAI
			        || SideToMove == Side.Black && isBlackAI)
			   ) {
				Movement bestMove = await uciEngine.GetBestMove(10_000);
				DoAIMove(bestMove);
			}*/
		}

		private void DoAIMove(Movement move) {
			// ER TODO remove, app layer should not know about presentation
			/*GameObject movedPiece = BoardManager.Instance.GetPieceGOAtPosition(move.Start);
			GameObject endSquareGO = BoardManager.Instance.GetSquareGOByPosition(move.End);
			OnPieceMoved(
				move.Start,
				movedPiece.transform,
				endSquareGO.transform,
				(move as PromotionMove)?.PromotionPiece
			);*/
		}
	}
}