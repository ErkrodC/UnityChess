using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;
using UnityChess.Core;
using UnityChess.Core.Resource;
using UnityChess.Core.Util;

namespace UnityChess.ResourceAcces {
	public class PigeonUCIEngine : IUCIEngine {
		private const string EXE_PATH = "/UCIEngines/pigeon-1.5.1/pigeon-1.5.1.exe";
		private readonly ILogger _logger;
		private readonly IResourcePathProvider _resourcePathProvider;
		private readonly FENSerializer _fenSerializer = new();
		private Process _engineProcess;
		private bool _isReady;
		private Timer _timer;
		private float _timeMS;
		private bool _isSearchingForBestMove;
		private Game _game;

		public PigeonUCIEngine(ILogger logger, IResourcePathProvider resourcePathProvider) {
			_logger = logger;
			_resourcePathProvider = resourcePathProvider;
		}

		public async void StartAsync() {
			_timer = new Timer(100);
			_timer.Elapsed += (_, _) => _timeMS += 100;

			_engineProcess = new Process();
			_engineProcess.StartInfo = new ProcessStartInfo(_resourcePathProvider.streamingAssetsPath + EXE_PATH) {
				UseShellExecute = false,
				RedirectStandardInput = true,
				RedirectStandardOutput = true,
				CreateNoWindow = true
			};
			_engineProcess.Start();

			await foreach (string engineOutputLine in Receive()) {
				_logger.Info(engineOutputLine);
			}

			await Send("uci");
			await foreach (string engineOutputLine in Receive("uciok")) {
				_logger.Info(engineOutputLine);
			}

			await Send("isready");
			await foreach (string engineOutputLine in Receive("readyok")) {
				_logger.Info(engineOutputLine);
			}
			_isReady = true;
		}

		public void ShutDown() {
			_engineProcess.Close();
		}

		public async Task SetupNewGame(Game game) {
			_game = game;

			while (!_isReady) {
				await Task.Yield();
			}

			await Send("ucinewgame");
		}

		public async Task<Movement> GetBestMove(int timeoutMS = -1) {
			Side sideToMove = _game.ConditionsTimeline.Head.SideToMove;
			await Send($"position fen {_fenSerializer.Serialize(_game)}");

			if (!_isSearchingForBestMove) {
				_isSearchingForBestMove = true;
				await Send($"go movetime {timeoutMS}");
			}

			await foreach (string line in Receive("bestmove")) {
				_logger.Info(line);
				if (line.StartsWith("bestmove")) {
					_isSearchingForBestMove = false;
					return ParseUCIMove(line.Split(" ")[1], sideToMove);
				}
			}

			await Send("stop");

			Movement result = null;
			await foreach (string line in Receive("bestmove")) {
				_logger.Info(line);
				if (line.StartsWith("bestmove")) {
					_isSearchingForBestMove = false;
					result = ParseUCIMove(line.Split(" ")[1], sideToMove);
				}
			}

			return result;
		}

		private static Movement ParseUCIMove(string uciMove, Side sideToMove) {
			Movement result;
			if (uciMove.Length > 4) {
				result = new PromotionMove(
					new Square(uciMove[..2]),
					new Square(uciMove[2..4])
				);

				ElectedPiece electedPiece = uciMove[4..5].ToLower() switch {
					"b" => ElectedPiece.Bishop,
					"n" => ElectedPiece.Knight,
					"q" => ElectedPiece.Queen,
					"r" => ElectedPiece.Rook,
					_ => ElectedPiece.None
				};

				((PromotionMove)result).SetPromotionPiece(
					PromotionUtil.GeneratePromotionPiece(electedPiece, sideToMove)
				);
			} else {
				result = new Movement(
					new Square(uciMove[..2]),
					new Square(uciMove[2..4])
				);
			}

			return result;
		}

		private async Task Send(string data) {
			await _engineProcess.StandardInput.WriteLineAsync($"{data}\n");
		}

		private async IAsyncEnumerable<string> Receive(string responseBreak = null, int timeoutMS = -1) {
			string line = null;
			float startTime = _timeMS;

			while (!ResponseFinished() && (timeoutMS < 0 || _timeMS - startTime < timeoutMS)) {
				line = await _engineProcess.StandardOutput.ReadLineAsync();
				yield return line;
			}

			bool ResponseFinished() => responseBreak switch {
				null => _engineProcess.StandardOutput.Peek() == -1,
				_ => line?.StartsWith(responseBreak) ?? false
			};
		}
	}
}