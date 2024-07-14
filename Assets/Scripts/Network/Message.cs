using System;
using System.Net;
using System.Text;
using UnityEngine;

namespace Network
{
	public class Message
	{
		private readonly byte[] _data;
		public Message(byte[] type, byte[] arg)
		{
			_data = new byte[NetworkManager.MsgTotalSize];
			Array.Copy(type, 0, _data, 0, NetworkManager.MsgTypeSize);
			Array.Copy(arg, 0, _data, NetworkManager.MsgTypeSize, NetworkManager.MsgArgSize); // Corrected line
		}
		public Message(EMessageType type, string arg)
		{
			_data = new byte[NetworkManager.MsgTotalSize];
			var msgTypeBuff = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)type));
			var msgArgBuff = Encoding.UTF8.GetBytes(arg);

			if (msgArgBuff.Length > NetworkManager.MsgArgSize)
			{
				Debug.Log("메시지 인자가 너무 큽니다.");
				return;
			}
			Array.Resize(ref msgArgBuff, NetworkManager.MsgArgSize);
			Array.Copy(msgTypeBuff, 0, _data, 0, NetworkManager.MsgTypeSize);
			Array.Copy(msgArgBuff, 0, _data, NetworkManager.MsgTypeSize, NetworkManager.MsgArgSize); // Corrected line
		}
		public byte[] Byte() => _data;
		public EMessageType Type() => (EMessageType)IPAddress.NetworkToHostOrder(BitConverter.ToInt32(_data[0..4], 0));
		public string Arg() => Encoding.UTF8.GetString(_data, NetworkManager.MsgTypeSize, NetworkManager.MsgArgSize)
			.TrimEnd('\0');
	}
}