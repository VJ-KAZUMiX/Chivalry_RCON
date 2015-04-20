using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChivalryRCON.Network
{
	internal class Packet
	{
		public MessageType msgType;
		private List<byte> data = new List<byte> ();
		private byte[] fullBuffer;
		private uint dataLength;
		private ulong _playerId;
		private string message;
		
		public Packet ()
		{
		}

		public Packet (MessageType msgType)
		{
			this.msgType = msgType;
		}

		public void addGUID (ulong playerId)
		{
			this.data.AddRange (BitConverter.GetBytes (playerId).Reverse<byte> ());
		}
		
		public void addInt (int toAdd)
		{
			this.data.AddRange (BitConverter.GetBytes (toAdd).Reverse<byte> ());
		}
		
		public void addString (string str)
		{
			byte[] bytes = Encoding.UTF8.GetBytes (str);
			this.data.AddRange (BitConverter.GetBytes ((int)bytes.Length).Reverse<byte> ());
			this.data.AddRange (Encoding.UTF8.GetBytes (str));
		}
		
		public void decode (byte[] rawPacket, int offset = 0)
		{
			this.msgType = (Packet.MessageType)BitConverter.ToUInt16 (rawPacket.Skip<byte> (offset).Take<byte> (2).Reverse<byte> ().ToArray<byte> (), 0);
			this.dataLength = BitConverter.ToUInt32 (rawPacket.Skip<byte> (2 + offset).Take<byte> (4).Reverse<byte> ().ToArray<byte> (), 0);
			this.data = rawPacket.Skip<byte> (6 + offset).Take<byte> ((int)this.dataLength).ToList<byte> ();
		}
		
		public byte[] encode ()
		{
			List<byte> nums = new List<byte> ();
			nums.AddRange (BitConverter.GetBytes ((ushort)this.msgType).Reverse<byte> ());
			nums.AddRange (BitConverter.GetBytes (this.data.Count).Reverse<byte> ());
			nums.AddRange (this.data);
			return nums.ToArray ();
		}
		
		public ulong getGUID ()
		{
			ulong num = BitConverter.ToUInt64 (this.data.Take<byte> (8).Reverse<byte> ().ToArray<byte> (), 0);
			this.data.RemoveRange (0, 8);
			return num;
		}
		
		public static int getHeaderSize ()
		{
			return 6;
		}
		
		public uint getInt ()
		{
			uint num = BitConverter.ToUInt32 (this.data.Take<byte> (4).Reverse<byte> ().ToArray<byte> (), 0);
			this.data.RemoveRange (0, 4);
			return num;
		}
		
		public static int getPacketSize (Packet toMeasure)
		{
			return (int)(toMeasure.dataLength + Packet.getHeaderSize ());
		}
		
		public static int getPacketSize (byte[] rawPacket, int offset)
		{
			return (int)(BitConverter.ToUInt32 (rawPacket.Skip<byte> (2 + offset).Take<byte> (4).Reverse<byte> ().ToArray<byte> (), 0) + Packet.getHeaderSize ());
		}
		
		public string getString ()
		{
			uint num = this.getInt ();
			string str = Encoding.UTF8.GetString (this.data.Take<byte> ((int)num).ToArray<byte> ());
			this.data.RemoveRange (0, (int)num);
			return str;
		}
		
		public enum MessageType
		{
			SERVER_CONNECT,
			SERVER_CONNECT_SUCCESS,
			PASSWORD,
			PLAYER_CHAT,
			PLAYER_CONNECT,
			PLAYER_DISCONNECT,
			SAY_ALL,
			SAY_ALL_BIG,
			SAY,
			MAP_CHANGED,
			MAP_LIST,
			CHANGE_MAP,
			ROTATE_MAP,
			TEAM_CHANGED,
			NAME_CHANGED,
			KILL,
			SUICIDE,
			KICK_PLAYER,
			TEMP_BAN_PLAYER,
			BAN_PLAYER,
			UNBAN_PLAYER,
			ROUND_END,
			PING
		}
	}
}