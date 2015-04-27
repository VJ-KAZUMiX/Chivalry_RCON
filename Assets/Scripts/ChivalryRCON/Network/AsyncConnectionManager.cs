using UnityEngine;
using System;
using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ChivalryRCON.Network
{
	public class AsyncConnectionManager
	{
		private readonly object sendSyncRoot = new object ();
		private readonly object receiveSyncRoot = new object ();
		private Socket socket;
		private IPEndPoint iep;

		public bool isReceiveCompleted {
			get { return socket.Available == 0;}
		}

		private AsyncCallback recieveData;
		private AsyncCallback callbackProc;

		public delegate void DataReceivedHandler (object sender,byte[] Data,int dataLength);

		public event DataReceivedHandler DataReceived;

		public delegate void ConnectionLostHandler (object sender);

		public event ConnectionLostHandler ConnectionLost;

		private byte[] mRecvBuf = new byte[8192];

		public AsyncConnectionManager ()
		{

		}

		public void connect (string host, int port)
		{
			socket = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			iep = new IPEndPoint (IPAddress.Parse (host), port);
			socket.Blocking = false;
			callbackProc = new AsyncCallback (ConnectCallback);
			IAsyncResult result = socket.BeginConnect (this.iep, callbackProc, socket);
			result.AsyncWaitHandle.WaitOne (1000);
			
			if (!socket.Connected) {
				// NOTE, MUST CLOSE THE SOCKET
				socket.Close ();
				throw new ApplicationException ("Failed to connect server.");
			}
		}

		private void ConnectCallback (IAsyncResult pAsyncRes)
		{
			try {
				Socket asyncState = (Socket)pAsyncRes.AsyncState;
				// Complete the connection.
				asyncState.EndConnect(pAsyncRes);

				if (asyncState.Connected) {
					this.recieveData = new AsyncCallback (this.OnRecieveData);
					asyncState.BeginReceive (this.mRecvBuf, 0, (int)this.mRecvBuf.Length, SocketFlags.None, this.recieveData, asyncState);
				}
			} catch (Exception exception) {
				Debug.LogError (exception);
			}
		}

		private void OnRecieveData (IAsyncResult pAsyncRes)
		{
			Debug.Log ("OnRecieveData");
			lock (receiveSyncRoot) {
				Socket asyncState = (Socket)pAsyncRes.AsyncState;
				int num = 0;
				try {
					num = asyncState.EndReceive (pAsyncRes);
					if (num > 0) {
						if (this.DataReceived != null) {
							this.DataReceived (this, this.mRecvBuf, num);
						}
						asyncState.BeginReceive (this.mRecvBuf, 0, (int)this.mRecvBuf.Length, SocketFlags.None, this.recieveData, asyncState);
					} else if (this.ConnectionLost != null) {
						this.ConnectionLost (this);
					}
					if (!asyncState.Connected && this.ConnectionLost != null) {
						this.ConnectionLost (this);
					}
				} catch (SocketException socketException) {
					if (this.ConnectionLost != null) {
						this.ConnectionLost (this);
					}
				} catch (Exception exception) {
					if (this.ConnectionLost != null) {
						this.ConnectionLost (this);
					}
				}
			}
		}

		public void Send (byte[] pData)
		{
			Debug.Log ("Send");
			lock (sendSyncRoot) {
				try {
					IAsyncResult asyncResult = socket.BeginSend (pData, 0, (int)pData.Length, SocketFlags.None, new AsyncCallback (SendCallback), socket);
				} catch (ObjectDisposedException objectDisposedException) {
					Debug.LogError ("Send: ObjectDisposedException");
					socket.Shutdown (SocketShutdown.Both);
					socket.Close ();
					Environment.Exit (0);
				} catch (Exception exception) {
					Debug.LogError ("Send: Exception");
					Environment.Exit (0);
				}
			}
		}

		private static void SendCallback (IAsyncResult ar)
		{
			try {
				// Retrieve the socket from the state object.
				Socket client = (Socket)ar.AsyncState;

				// Complete sending the data to the remote device.
				int bytesSent = client.EndSend (ar);
				Debug.LogFormat ("Sent {0} bytes to server.", bytesSent);
				
			} catch (Exception e) {
				Debug.LogFormat (e.ToString ());
			}
		}
	}
}