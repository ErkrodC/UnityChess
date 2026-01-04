using System;

namespace UnityChess.Resource {
	public interface ILogger {
		void Info(string message);
		void Warn(string message);
		void Error(string message, Exception exception = null);
	}
}