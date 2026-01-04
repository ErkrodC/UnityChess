using System;
using UnityEngine;
using ILogger = UnityChess.Resource.ILogger;

namespace UnityChess.Presentation.ResourceAccess {
	public class UnityLogger : ILogger {
		public void Info(string message) {
			Debug.Log(message);
		}

		public void Warn(string message) {
			Debug.LogWarning(message);
		}

		public void Error(string message, Exception exception = null) {
			Debug.LogError($"{message}\n{exception}");
		}
	}
}