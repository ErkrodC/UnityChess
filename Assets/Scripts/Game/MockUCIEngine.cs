using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace UnityChess.Engine {
	public class MockUCIEngine : IUCIEngine {
		private Process engineProcess;
		private string exePath = Application.streamingAssetsPath + "/UCIEngines/pigeon-1.5.1/pigeon-1.5.1.exe";
		private bool isReady;
		private Timer timer;
		private float timeMS;
		
		private FENSerializer fenSerializer = new FENSerializer();
		private bool isSearchingForBestMove;
		private Game game;
		
		public async void Start() {
			timer = new Timer(100);
			timer.Elapsed += (_, _) => timeMS += 100;
			
			engineProcess = new Process();
			engineProcess.StartInfo = new ProcessStartInfo(
				exePath
			) {
				UseShellExecute = false,
				RedirectStandardInput = true,
				RedirectStandardOutput = true,
				CreateNoWindow = true
			};
			engineProcess.Start();
			
			await foreach (string engineOutputLine in Receive()) {
				Debug.Log(engineOutputLine);
			}

			await Send("uci");
			await foreach (string engineOutputLine in Receive("uciok")) {
				Debug.Log(engineOutputLine);
			}
			
			await Send("isready");
			await foreach (string engineOutputLine in Receive("readyok")) {
				Debug.Log(engineOutputLine);
			}
			isReady = true;
		}

		public void ShutDown() {
			engineProcess.Close();
		}
		
		public async Task SetupNewGame(Game game) {
			this.game = game;

			while (!isReady) {
				await Task.Yield();
			}
			
			await Send("ucinewgame");
		}

		public async Task<Movement> GetBestMove(int timeoutMS = -1) {
			game.ConditionsTimeline.TryGetCurrent(out GameConditions currentConditions);
			Side sideToMove = currentConditions.SideToMove;
			await Send($"position fen {fenSerializer.Serialize(game)}");

			if (!isSearchingForBestMove) {
				isSearchingForBestMove = true;
				await Send($"go movetime {timeoutMS}");
			}
			
			await foreach (string line in Receive("bestmove")) {
				Debug.Log(line);
				if (line.StartsWith("bestmove")) {
					isSearchingForBestMove = false;
					return ParseUCIMove(line.Split(" ")[1], sideToMove);
				}
			}

			await Send("stop");

			Movement result = null;
			await foreach (string line in Receive("bestmove")) {
				Debug.Log(line);
				if (line.StartsWith("bestmove")) {
					isSearchingForBestMove = false;
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
			await engineProcess.StandardInput.WriteLineAsync($"{data}\n");
		}

		private async IAsyncEnumerable<string> Receive(string responseBreak = null, int timeoutMS = -1) {
			string line = null;
			float startTime = timeMS;

			while (!ResponseFinished() && (timeoutMS < 0 || timeMS - startTime < timeoutMS)) {
				line = await engineProcess.StandardOutput.ReadLineAsync();
				yield return line;
			}

			bool ResponseFinished() => responseBreak switch {
				null => engineProcess.StandardOutput.Peek() == -1,
				_ => line?.StartsWith(responseBreak) ?? false
			};
		}
	}
}