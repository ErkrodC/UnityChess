using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class MultiplayerSystem : MonoBehaviourSingleton<MultiplayerSystem> {
	private Socket serverSocket;
	
	private void Start() {
		serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		serverSocket.Bind(new IPEndPoint(IPAddress.Any, 23001));
		serverSocket.ConnectAsync(new IPEndPoint(IPAddress.Loopback, 23000)).ContinueWith(task => {
			if (!task.IsFaulted) {
				Debug.LogError("Connected");
			}
		});
	}
}