using System;

namespace UnityChess.Core.Resource {
	public interface ILogger {
		void Info(string message);
		void Warn(string message);
		void Error(string message, Exception exception = null);
	}
}