using System.IO;
using Newtonsoft.Json;
using UnityChess.Resource;

namespace UnityChess.ResourceAccess {
	public class JsonStorage : IStorage {
		private readonly string _persistentDataPath;
		private readonly ILogger _logger;

		public JsonStorage(IResourcePathProvider resourcePathProvider, ILogger logger) {
			_persistentDataPath = resourcePathProvider.persistentDataPath;
			_logger = logger;
		}

		public bool TryLoad<T>(out T value, string fileName = null) {
			string path = ResolvePath<T>(fileName);
			if (!File.Exists(path)) {
				value = default;
				return false;
			}

			string json = File.ReadAllText(path);
			value = JsonConvert.DeserializeObject<T>(json);
			return true;
		}

		public void Save<T>(T value, string fileName = null) {
			string json = JsonConvert.SerializeObject(value, Formatting.Indented);
			string path = ResolvePath<T>(fileName);
			File.WriteAllText(path, json);
			_logger.Info($"Saved storage file at {path}");
		}

		public void Delete<T>() {
			string path = ResolvePath<T>();
			if (!File.Exists(path)) { return; }

			File.Delete(path);
			_logger.Info($"Deleted storage file at {path}");
		}

		public void Delete(string fileName = null) {
			string path = ResolvePath(fileName);
			if (string.IsNullOrEmpty(fileName) || !File.Exists(path)) { return; }

			File.Delete(path);
			_logger.Info($"Deleted storage file at {path}");
		}

		private string ResolvePath<T>(string fileName = null) {
			return Path.Combine(_persistentDataPath, fileName ?? typeof(T).Name);
		}

		private string ResolvePath(string fileName) {
			return Path.Combine(_persistentDataPath, fileName);
		}
	}
}