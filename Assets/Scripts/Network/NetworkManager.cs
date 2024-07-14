using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Game;
using UI;
using UnityEngine;
using Util;
using Util.EventSystem;
using Util.SingletonSystem;

namespace Network
{
	public class NetworkManager : MonoBehaviourSingleton<NetworkManager>
	{
		public const int MsgTypeSize = 4;
		public const int MsgArgSize = 128;
		public const int MsgTotalSize = 132;
		[SerializeField] private int serverPort;
		[SerializeField] private string serverIP;

		private TcpClient _client;
		private NetworkStream _stream;

		public async void TryConnectServer(string userName)
		{
			if (_client != null)
			{
				Debug.Log("이미 연결되어 있음");
				return;
			}

			using CancellationTokenSource cancelTokenSource = new(TimeSpan.FromSeconds(2));
			try
			{
				_client = new TcpClient();
				await _client.ConnectAsync(serverIP, serverPort).WithCancellation(cancelTokenSource.Token);
				_stream = _client.GetStream();
				Receive();
				Debug.Log("서버에 성공적으로 연결되었습니다.");
				EventManager.Instance.PostNotification(EEventType.ServerConnection, this, EConnectResult.Success);
				await Send(new Message(EMessageType.MT_SET_NAME, userName));
			}
			catch (OperationCanceledException)
			{
				Debug.Log("연결 실패: 타임아웃");
				DisconnectServer();
				EventManager.Instance.PostNotification(EEventType.ServerConnection, this, EConnectResult.TimeOut);
			}
			catch (Exception ex)
			{
				Debug.Log($"연결 실패: {ex.Message}");
				DisconnectServer();
				EventManager.Instance.PostNotification(EEventType.ServerConnection, this, EConnectResult.Error);
			}
		}
		public void DisconnectServer()
		{
			EventManager.Instance.PostNotification(EEventType.ServerConnection, this, EConnectResult.Disconnect);
			Debug.Log("Server disconnected");
			_stream?.Dispose();
			_client?.Dispose();
			_stream = null;
			_client = null;
		}
		public async Task Send(Message msg)
		{
			Debug.Log($"Message Send : Type={msg.Type().ToString()}, Arg={msg.Arg()}");
			if (_client is not { Connected: true })
			{
				Debug.Log("서버에 연결되어 있지 않습니다.");
				return;
			}
			try
			{
				await _stream.WriteAsync(msg.Byte(), 0, MsgTotalSize);
			}
			catch (Exception ex)
			{
				Debug.Log($"Send Error: {ex.Message}");
				DisconnectServer();
			}
		}
		private async void Receive()
		{
			while (_client != null)
			{
				try
				{
					var msgTypeBuff = new byte[MsgTypeSize];
					var msgArgBuff = new byte[MsgArgSize];

					var bytesRead = await _stream.ReadAsync(msgTypeBuff, 0, msgTypeBuff.Length);
					if (bytesRead < msgTypeBuff.Length)
					{
						DisconnectServer();
						break;
					}
					bytesRead = await _stream.ReadAsync(msgArgBuff, 0, msgArgBuff.Length);
					if (bytesRead < msgArgBuff.Length)
					{
						DisconnectServer();
						break;
					}
					OnMessageReceived(new Message(msgTypeBuff, msgArgBuff));
				}
				catch (Exception ex)
				{
					Debug.Log($"Receive Error: {ex.Message}");
					DisconnectServer();
					break;
				}
			}
		}
		private void OnMessageReceived(Message msg)
		{
			Debug.Log($"Message Received : Type={msg.Type().ToString()}, Arg={msg.Arg()}");
			switch (msg.Type())
			{
				case EMessageType.MT_NOTING:
					break;
				case EMessageType.MT_MATCHQ_JOIN:
					break;
				case EMessageType.MT_SET_NAME:
					break;
				case EMessageType.MT_MESSEGE:
					//메시지
					break;
				case EMessageType.MT_ACTIVE_USER:
					UIManager.Instance.SetActiveUserText(msg.Arg());
					break;
				case EMessageType.MT_ROOM_CREATED:
					GameManager.Instance.MatchFound(msg.Arg());
					break;
				case EMessageType.MT_GAME_RESULT:
					if (GameManager.Instance.state == EGameState.InGame && msg.Arg() == "Enemy escaped")
						GameManager.Instance.GameOver(GameManager.Instance.playerTileType);
					break;
				case EMessageType.MT_USER_ACTION:
					var temp = msg.Arg().Split(",");
					var id = new Vector2Int(int.Parse(temp[0]),int.Parse(temp[1]));
					EventManager.Instance.PostNotification(EEventType.EnemyTileClicked, this, id);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		~NetworkManager()
		{
			_stream?.Dispose();
			_client?.Dispose();
		}
	}
}