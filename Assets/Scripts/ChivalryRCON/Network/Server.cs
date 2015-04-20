using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ChivalryRCON.Network
{
	public class Server
	{
		private enum LogState
		{
			NotLogged = 0,
			Loggin,
			Logged
		}
		private LogState logState = LogState.NotLogged;
		private string host;
		private int port;
		private string pass;
		private AsyncConnectionManager asyncConnectionManager;
		private volatile List<byte> buffer = new List<byte> (8192);
		private bool isDecoding;

		public Server (string host, int port, string pass)
		{
			this.host = host;
			this.port = port;
			this.pass = pass;
		}

		public void connet ()
		{
			asyncConnectionManager = new AsyncConnectionManager ();
			asyncConnectionManager.DataReceived += asyncClient_DataReceived;
			asyncConnectionManager.connect (host, port);
		}

		private void asyncClient_DataReceived (object sender, byte[] Data, int dataLength)
		{
			switch (logState) {

			case LogState.NotLogged:
				{
					Debug.Log ("LogState.NotLogged");
					Packet packet = new Packet ();
					packet.decode (Data, 0);
					Packet packet1 = new Packet (Packet.MessageType.PASSWORD);
					packet1.addString (SHA1Util.SHA1HashStringForUTF8String (string.Concat (pass, packet.getString ())));
					logState = LogState.Loggin;
					asyncConnectionManager.Send (packet1.encode ());
					return;
					break;
				}
			case Server.LogState.Loggin:
				{
					Debug.Log ("Server.LogState.Loggin");
					Packet packet2 = new Packet ();
					packet2.decode (Data, 0);
					Packet.MessageType messageType = packet2.msgType;
					logState = LogState.Logged;
					break;
				}
			case Server.LogState.Logged:
				{
					Debug.Log ("Server.LogState.Logged");
					lock (this.buffer) {
						this.buffer.AddRange (Data.Take<byte> (dataLength));
						decodeBuffer ();
					}
					break;
				}

			}
		}

		private void decodeBuffer ()
		{
			int num = 0;
			this.isDecoding = true;
			int num1 = 0;
			try {
				while (this.buffer.Count >= 6 && this.isDecoding) {
					num1++;
					if (num1 <= 100) {
						int packetSize = Packet.getPacketSize (this.buffer.ToArray (), 0);
						if (this.buffer.Count < packetSize) {
							continue;
						}
						Packet packet = new Packet ();
						packet.decode (this.buffer.ToArray (), num);
						if (packet.msgType < Packet.MessageType.PLAYER_CHAT || packet.msgType > Packet.MessageType.PING || num1 > 200) {
							//this.asyncClient_ConnectionLost(null);
							break;
						} else {
							this.buffer.RemoveRange (0, Packet.getPacketSize (this.buffer.ToArray (), 0));
							switch (packet.msgType) {
							case Packet.MessageType.PLAYER_CHAT:
								{
									Debug.Log ("Packet.MessageType.PLAYER_CHAT");
									continue;
								}
							case Packet.MessageType.PLAYER_CONNECT:
								{
									Debug.Log ("Packet.MessageType.PLAYER_CONNECT");
									continue;
								}
							case Packet.MessageType.PLAYER_DISCONNECT:
								{
									Debug.Log ("Packet.MessageType.PLAYER_DISCONNECT");
									continue;
								}
							case Packet.MessageType.MAP_CHANGED:
								{
									Debug.Log ("Packet.MessageType.MAP_CHANGED");
									uint mapIndex = packet.getInt ();
									string mapName = packet.getString ();
									Debug.LogFormat ("MAP_CHANGED : {0}, {1}", mapName, mapIndex);
									continue;
								}
							case Packet.MessageType.MAP_LIST:
								{
									Debug.Log ("Packet.MessageType.MAP_LIST");
									string mapName = packet.getString ();
									Debug.LogFormat ("MAP_LIST : {0}", mapName);
									continue;
								}
							case Packet.MessageType.TEAM_CHANGED:
								{
									Debug.Log ("Packet.MessageType.PLAYER_CHAT");
									continue;
								}
							case Packet.MessageType.NAME_CHANGED:
								{
									Debug.Log ("Packet.MessageType.NAME_CHANGED");
									continue;
								}
							case Packet.MessageType.KILL:
								{
									Debug.Log ("Packet.MessageType.KILL");
									continue;
								}
							case Packet.MessageType.SUICIDE:
								{
									Debug.Log ("Packet.MessageType.SUICIDE");
									continue;
								}
							case Packet.MessageType.ROUND_END:
								{
									Debug.Log ("Packet.MessageType.ROUND_END");
									continue;
								}
							case Packet.MessageType.PING:
								{
									Debug.Log ("Packet.MessageType.PING");
									continue;
								}
							default:
								{
									Debug.Log ("unknown Packet.MessageType: " + packet.msgType);
									continue;
								}
							}
						}
					} else {
						//this.asyncClient_ConnectionLost(null);
						this.isDecoding = false;
						break;
					}
				}
			} catch (Exception exception) {
				//this.asyncClient_ConnectionLost(null);
				this.isDecoding = false;
			}
			if (this.buffer.Count != 0) {
				//this.asyncClient_ConnectionLost(null);
				this.isDecoding = false;
			}
			this.isDecoding = false;
		}

	
	}
}