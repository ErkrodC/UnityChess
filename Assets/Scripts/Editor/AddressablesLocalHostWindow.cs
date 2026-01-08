using System;
using System.IO;
using System.Net;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace UnityChess.Editor {
	public class AddressablesLocalHostWindow : EditorWindow {
		private const string _PORT_PREFERENCE_KEY = "AddressablesHostPort";
		private const string _PATH_PREFERENCE_KEY = "AddressablesHostPath";

		private HttpListener _listener;
		private Thread _listenerThread;
		private bool _isRunning;

		private int _port;
		private string _servedPath;

		[MenuItem("Tools/Addressables/ Local Host...")]
		private static void Open() {
			AddressablesLocalHostWindow window = GetWindow<AddressablesLocalHostWindow>("Addressables Local Host");
			window.minSize = new Vector2(420, 150);
			window.Show();
		}

		private void OnEnable() {
			_port = EditorPrefs.GetInt(_PORT_PREFERENCE_KEY, 3000);
			string platform = EditorUserBuildSettings.activeBuildTarget.ToString();
			string defaultFolder = Path.Combine("ServerData", platform);
			_servedPath = EditorPrefs.GetString(_PATH_PREFERENCE_KEY, defaultFolder);
		}

		private void OnDisable() {
			StopServer();
		}

		private void OnGUI() {
			EditorGUILayout.LabelField("Local Addressables Host", EditorStyles.boldLabel);
			EditorGUILayout.HelpBox("Serves Addressables bundles from a local HTTP endpoint for playmode testing.",
				MessageType.Info);

			using (new EditorGUI.DisabledScope(_isRunning)) {
				_servedPath = EditorGUILayout.TextField("Server Folder", _servedPath);
				_port = EditorGUILayout.IntField("Port", _port);
				if (GUILayout.Button("Start Server")) {
					StartServer();
				}
			}

			using (new EditorGUI.DisabledScope(!_isRunning)) {
				if (GUILayout.Button("Stop Server")) {
					StopServer();
				}
			}

			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Status:", _isRunning ? "Running" : "Stopped");
			if (_isRunning) {
				EditorGUILayout.LabelField("URL:", $"http://localhost:{_port}");
			}
		}

		private void StartServer() {
			if (_isRunning) { return; }

			if (!Directory.Exists(_servedPath)) {
				EditorUtility.DisplayDialog("Invalid Path", $"Folder not found:\n{_servedPath}", "OK");
				return;
			}

			try {
				_listener = new HttpListener();
				_listener.Prefixes.Add($"http://*:{_port}/");
				_listener.Start();
				_isRunning = true;

				_listenerThread = new Thread(() => ServeLoop(_servedPath));
				_listenerThread.Start();

				EditorPrefs.SetInt(_PORT_PREFERENCE_KEY, _port);
				EditorPrefs.SetString(_PATH_PREFERENCE_KEY, _servedPath);
			} catch (Exception e) {
				Debug.LogError($"Failed to start HTTP server: {e.Message}");
				StopServer();
			}
		}

		private void StopServer() {
			if (!_isRunning) { return; }

			_isRunning = false;
			try {
				_listener?.Stop();
				_listener?.Close();
			} catch { /*ignore*/ }

			if (_listenerThread?.IsAlive == true) {
				_listenerThread.Abort();
				_listenerThread = null;
			}
		}

		private void ServeLoop(string root) {
			while (_isRunning && _listener?.IsListening == true) {
				try {
					HttpListenerContext context = _listener.GetContext();
					ThreadPool.QueueUserWorkItem(_ => ProcessRequest(context, root));
				} catch (HttpListenerException) {
					// Listener close; exit loop
					break;
				} catch (Exception e) {
					Debug.LogError($"HTTP server error: {e}");
				}
			}
		}

		private static void ProcessRequest(HttpListenerContext context, string root) {
			string relativePath = context.Request.Url.AbsolutePath.TrimStart('/');
			string fullPath = Path.Combine(root, relativePath);

			if (Directory.Exists(fullPath)) {
				fullPath = Path.Combine(fullPath, "index.html");
			}

			if (!File.Exists(fullPath)) {
				context.Response.StatusCode = (int)HttpStatusCode.NotFound;
				context.Response.Close();
				return;
			}

			try {
				byte[] bytes = File.ReadAllBytes(fullPath);
				context.Response.StatusCode = (int)HttpStatusCode.OK;
				context.Response.ContentType = GetMimeType(Path.GetExtension(fullPath));
				context.Response.OutputStream.Write(bytes, 0, bytes.Length);
			} catch (Exception e) {
				Debug.LogError($"Failed to serve {relativePath}: {e}");
				context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
			} finally {
				context.Response.Close();
			}
		}

		private static string GetMimeType(string extension) {
			return extension switch {
				".html" => "application/json",
				".hash" => "text/plain",
				_ => "application/octet-stream"
			};
		}
	}
}