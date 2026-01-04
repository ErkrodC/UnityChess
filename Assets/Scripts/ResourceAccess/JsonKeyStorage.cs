using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityChess.Resource;

namespace UnityChess.ResourceAccess {
	public class JsonKeyStorage : IKeyStorage {
		private readonly string _filePath;
		private readonly IResourcePathProvider _resourcePathProvider;
		private readonly ILogger _logger;
		private readonly Dictionary<string, string> _cache;

		public JsonKeyStorage(IResourcePathProvider resourcePathProvider, ILogger logger) {
			_resourcePathProvider = resourcePathProvider;
			_filePath = _resourcePathProvider.persistentDataPath + "/keys.json";
			_logger = logger;

			if (File.Exists(_filePath)) {
				string json = File.ReadAllText(_filePath);
				_cache = JsonConvert.DeserializeObject<Dictionary<string, string>>(json)
					?? new Dictionary<string, string>();
			} else {
				Directory.CreateDirectory(Path.GetDirectoryName(_filePath));
				_cache = new Dictionary<string, string>();
			}
		}

		public bool TryGet<T>(string key, out T value) {
			if (_cache.TryGetValue(key, out string json)) {
				value = JsonConvert.DeserializeObject<T>(json);
				return true;
			}

			value = default;
			return false;
		}

		public void Set<T>(string key, T value) {
			_cache[key] = JsonConvert.SerializeObject(value);
		}

		public void Delete(string key) {
			if (_cache.Remove(key)) { Save(); }
		}

		public void Save() {
			string json = JsonConvert.SerializeObject(_cache, Formatting.Indented);
			File.WriteAllText(_filePath, json);
			_logger.Info($"Saved storage file at {_filePath}");
		}
	}
}