using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using UnityChess;
using UnityChess.Networking;
using UnityEngine;

public class MultiplayerSystem : MonoBehaviourSingleton<MultiplayerSystem> {
	private Socket serverSocket;
	
	private void Start() {
		serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
		serverSocket.Bind(new IPEndPoint(IPAddress.Any, 23001));
		serverSocket.ConnectAsync(new IPEndPoint(IPAddress.Loopback, 23000)).ContinueWith(task => {
			if (!task.IsFaulted) {
				Debug.LogError("Connected");
				GameManager.Instance.MoveExecuted += SendMoveToServer;
			}
		});
	}

	private void SendMoveToServer() {
		Movement latestMove = GameManager.Instance.HalfMoveTimeline.Current.Move;

		UnityChessDataPacket dataPacket;
		dataPacket.UserCommand = UserCommand.ExecuteMove;
		dataPacket.byte0 = (byte) latestMove.Start.File;
		dataPacket.byte1 = (byte) latestMove.Start.Rank;
		dataPacket.byte2 = (byte) latestMove.End.File;
		dataPacket.byte3 = (byte) latestMove.End.Rank;
		
		int dataPacketSize = Marshal.SizeOf<UnityChessDataPacket>();
		
		byte[] buffer = new byte[dataPacketSize];
		IntPtr dataPacketPtr = Marshal.AllocHGlobal(dataPacketSize);
		int offset = 0;
		
		Marshal.StructureToPtr(dataPacket, dataPacketPtr, false);
		Marshal.Copy(dataPacketPtr, buffer, offset, dataPacketSize);
		Marshal.FreeHGlobal(dataPacketPtr);
		// offset += dataPacketSize;

		serverSocket.Send(buffer, 1024, SocketFlags.None);
	}
}