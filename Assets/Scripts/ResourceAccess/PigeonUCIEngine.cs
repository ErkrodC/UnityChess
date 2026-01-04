using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityChess.Core;
using UnityChess.Resource;

namespace UnityChess.ResourceAccess {
	public class PigeonUCIEngine : IUCIEngine, IDisposable {
		public ElectedPiece promotionElection { get; private set; }

		private const int _DEFAULT_INIT_TIMEOUT_MS = 10_000;
		private const int _DEFAULT_MOVE_TIMEOUT_MS = 15_000;
		private const int _GET_BEST_MOVE_GRACE_PERIOD = 1000;
		private const string _EXE_PATH = "UCIEngines/pigeon-1.5.1/pigeon-1.5.1.exe";
		private readonly ILogger _logger;
		private readonly IResourcePathProvider _resourcePathProvider;
		private readonly SemaphoreSlim _commandLock = new(1, 1);
		private readonly SemaphoreSlim _engineInitLock = new(1, 1);
		private CancellationTokenSource _engineCancellationSource;
		private Process _engineProcess;

		public PigeonUCIEngine(ILogger logger, IResourcePathProvider resourcePathProvider) {
			_logger = logger;
			_resourcePathProvider = resourcePathProvider;
		}

		public void Dispose() {
			CleanupProcess();
			_engineInitLock.Dispose();
			_commandLock.Dispose();
		}

		public async Task StartNewGameAsync() {
			await EnsureEngineReadyAsync();
			await SendAsync("ucinewgame");
		}

		public async Task<(Square start, Square end)> GetBestMove(string fen, int timeoutMS = -1) {
			await EnsureEngineReadyAsync();
			await _commandLock.WaitAsync();
			bool hasTimeout = timeoutMS > 0;
			int effectiveTimeout = hasTimeout ? timeoutMS : _DEFAULT_MOVE_TIMEOUT_MS;
			int waitForTimeoutMS = hasTimeout ? effectiveTimeout + _GET_BEST_MOVE_GRACE_PERIOD : -1;
			string goCommand = hasTimeout ? $"go movetime {timeoutMS}" : "go";

			try {
				await SendAsync($"position fen {fen}");
				await SendAsync(goCommand);

				string bestMoveLine = await WaitForResponseAsync("bestmove", waitForTimeoutMS);
				string[] tokens = bestMoveLine.Split(" ", StringSplitOptions.RemoveEmptyEntries);
				if (tokens.Length < 2) {
					throw new InvalidOperationException($"Engine returned an unexpected bestmove payload: '{bestMoveLine}'");
				}

				return ParseUCIMove(tokens[1]);
			} catch (TimeoutException) {
				_logger.Warn($"Timed out waiting for bestmove after {effectiveTimeout} ms");
				await SendSafeAsync("stop");
				throw;
			} finally {
				_commandLock.Release();
			}
		}

		private async Task EnsureEngineReadyAsync() {
			if (_engineProcess is { HasExited: false }) { return; }

			await _engineInitLock.WaitAsync();
			try {
				if (_engineProcess is { HasExited: false }) { return; }

				CleanupProcess();

				string exeFullPath = Path.Combine(_resourcePathProvider.streamingAssetsPath, _EXE_PATH);
				if (!File.Exists(exeFullPath)) {
					throw new FileNotFoundException($"Could not find engine executable at '{exeFullPath}'.");
				}

				_engineCancellationSource = new CancellationTokenSource();
				_engineProcess = new Process {
					StartInfo = new ProcessStartInfo {
						FileName = exeFullPath,
						UseShellExecute = false,
						RedirectStandardInput = true,
						RedirectStandardOutput = true,
						RedirectStandardError = true,
						CreateNoWindow = true
					},
					EnableRaisingEvents = true
				};

				_engineProcess.ErrorDataReceived += (_, args) => {
					if (!string.IsNullOrWhiteSpace(args.Data)) {
						_logger.Error($"[{nameof(PigeonUCIEngine)} stderr]: {args.Data}");
					}
				};
				_engineProcess.Exited += (_, _) => _logger.Warn("UCI engine process exited unexpectedly.");

				_engineProcess.Start();
				_engineProcess.BeginErrorReadLine();

				await SendAsync("uci");
				await WaitForResponseAsync("uciok", _DEFAULT_INIT_TIMEOUT_MS);

				await SendAsync("isready");
				await WaitForResponseAsync("readyok", _DEFAULT_INIT_TIMEOUT_MS);
			} finally {
				_engineInitLock.Release();
			}
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

		private async Task SendAsync(string data) {
			if (_engineProcess?.HasExited ?? true) {
				throw new InvalidOperationException("Engine process has exited.");
			}

			await _engineProcess.StandardInput.WriteLineAsync(data);
			await _engineProcess.StandardInput.FlushAsync();
		}

		private Task SendSafeAsync(string data) {
			try { return SendAsync(data); }
			catch { return Task.CompletedTask; }
		}

		private async Task<string> WaitForResponseAsync(string expectedPrefix, int timeoutMs) {
			Stopwatch stopwatch = Stopwatch.StartNew();
			while (true) {
				int remaining = timeoutMs > 0
					? Math.Max(1, timeoutMs - (int)stopwatch.ElapsedMilliseconds)
					: Timeout.Infinite;
				string line = await ReadLineAsync(remaining);
				if (line == null) {
					_logger.Error($"UCI engine output stream closed while waiting for '{expectedPrefix}'.");
					throw new InvalidOperationException("UCI engine closed its output stream unexpectedly.");
				}

				_logger.Info(line);
				if (line.StartsWith(expectedPrefix, StringComparison.Ordinal)) {
					return line;
				}
			}
		}

		private async Task<string> ReadLineAsync(int timeoutMs) {
			if (_engineProcess == null) { throw new InvalidOperationException("UCI engine process is not available."); }

			Task<string> readTask = _engineProcess.StandardOutput.ReadLineAsync();
			if (timeoutMs == Timeout.Infinite) {
				return await readTask;
			}

			Task completed = await Task.WhenAny(
				readTask,
				Task.Delay(timeoutMs, _engineCancellationSource?.Token ?? CancellationToken.None)
			);
			if (completed == readTask) { return await readTask; }

			ResetEngineCancellationSource();
			throw new TimeoutException($"UCI engine did not reply within {timeoutMs} ms");
		}

		private void CleanupProcess() {
			try {
				if (_engineProcess == null) { return; }

				_engineCancellationSource?.Cancel();
				if (!_engineProcess.HasExited) {
					_engineProcess.Kill();
					_engineProcess.WaitForExit();
				}
				_engineProcess.Dispose();
			} finally {
				_engineProcess = null;
				_engineCancellationSource?.Dispose();
				_engineCancellationSource = null;
			}
		}

		private void ResetEngineCancellationSource() {
			if (_engineCancellationSource == null) { return; }

			try {
				if (!_engineCancellationSource.IsCancellationRequested) {
					_engineCancellationSource.Cancel();
				}
			} catch (ObjectDisposedException) {
				// already disposed elsewhere
			} finally {
				_engineCancellationSource.Dispose();
				_engineCancellationSource = new CancellationTokenSource();
			}
		}
	}
}