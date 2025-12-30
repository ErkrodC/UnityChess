using System;
using UnityChess.Core.Resource;

namespace Server.ResourceAccess {
	public class ServerLogger : ILogger {
		public void Info(string message) {
			Console.WriteLine($"INFO: {message}");
		}

		public void Warn(string message) {
			Console.WriteLine($"WARN: {message}");
		}

		public void Error(string message, Exception exception = null) {
			Console.WriteLine($"ERROR: {message}\n{exception}");
		}
	}
}