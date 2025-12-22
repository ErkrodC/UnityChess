using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;
using UnityChess.Core;
using UnityChess.Core.Resource;

namespace UnityChess.ResourceAccess {
	public class PigeonUCIEngine : IUCIEngine, IDisposable {
		public ElectedPiece promotionElection { get; private set; }
		private const string EXE_PATH = "/UCIEngines/pigeon-1.5.1/pigeon-1.5.1.exe";
		private readonly ILogger _logger;
		private readonly IResourcePathProvider _resourcePathProvider;
		private Process _engineProcess;
		private bool _isReady;
		private Timer _timer;
		private float _timeMS;
		private bool _isSearchingForBestMove;

		public PigeonUCIEngine(ILogger logger, IResourcePathProvider resourcePathProvider) {
			_logger = logger;
			_resourcePathProvider = resourcePathProvider;
		}

		public void Dispose() {
			_engineProcess.Close();
		}

		public async Task StartNewGameAsync() {
			if (!_isReady) {
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

			await Send("ucinewgame");
		}

		public async Task<(Square start, Square end)> GetBestMove(string fen, int timeoutMS = -1) {
			await Send($"position fen {fen}");

			if (!_isSearchingForBestMove) {
				_isSearchingForBestMove = true;
				await Send($"go movetime {timeoutMS}");
			}

			await foreach (string line in Receive("bestmove")) {
				_logger.Info(line);
				if (line.StartsWith("bestmove")) {
					_isSearchingForBestMove = false;
					return ParseUCIMove(line.Split(" ")[1]);
				}
			}

			await Send("stop");

			return default;
		}

		private (Square start, Square end) ParseUCIMove(string uciMove) {
			promotionElection = uciMove.Length > 4
				? uciMove[4..5].ToLower() switch {
					"b" => ElectedPiece.Bishop,
					"n" => ElectedPiece.Knight,
					"q" => ElectedPiece.Queen,
					"r" => ElectedPiece.Rook,
					_ => ElectedPiece.None
				}
				: ElectedPiece.None;

			return (
				start: new Square(uciMove[..2]),
				end: new Square(uciMove[2..4])
			);
		}

		private async Task Send(string data) {
			await _engineProcess.StandardInput.WriteLineAsync($"{data}\n");
		}

		private async IAsyncEnumerable<string> Receive(string responseBreak = null, int timeoutMS = -1) {
			string line = null;
			float startTime = _timeMS;

			while (!IsResponseFinished() && (timeoutMS < 0 || _timeMS - startTime < timeoutMS)) {
				line = await _engineProcess.StandardOutput.ReadLineAsync();
				yield return line;
			}

			yield break;

			bool IsResponseFinished() => responseBreak switch {
				null => _engineProcess.StandardOutput.Peek() == -1,
				_ => line?.StartsWith(responseBreak) ?? false
			};
		}
	}
}