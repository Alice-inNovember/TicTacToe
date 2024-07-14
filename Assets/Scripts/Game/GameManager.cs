using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using Network;
using UI;
using UnityEngine;
using UnityEngine.Serialization;
using Util.EventSystem;
using Util.SingletonSystem;

namespace Game
{
	public enum EGameState
	{
		PreLogin,
		Lobby,
		MatchMaking,
		PreGame,
		InGame,
		PostGame,
	}

	public class GameManager : MonoBehaviourSingleton<GameManager>, IEventListener
	{
		public EGameState state;
		public TileType playerTileType;
		public TileType enemyTileType;
		public TileType turn;
		private CancellationTokenSource _matchTimeCountCancelToken;
		public string playerName;
	
		public void OnEvent(EEventType eventType, Component sender, object param = null)
		{
			switch (eventType)
			{
				case EEventType.ProgramStart:
					break;
				case EEventType.ServerConnection:
					if (param != null)
						ServerConnectionAction((EConnectResult)param);
					break;
				case EEventType.GameStart:
					break;
				case EEventType.PlayerTileClicked:
					break;
				case EEventType.EnemyTileClicked:
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null);
			}
		}

		private void Start()
		{
			EventManager.Instance.AddListener(EEventType.ServerConnection, this);
			Application.runInBackground = true;
			state = EGameState.PreLogin;
			playerTileType = TileType.Null;
		}

		public void SetGameToLogin()
		{
			state = EGameState.PreLogin;
		}
		public async void StartMachMaking()
		{
			if (state != EGameState.Lobby)
				return;
			state = EGameState.MatchMaking;
			MatchMakingTimeCount();
			await NetworkManager.Instance.Send(new Message(EMessageType.MT_MATCHQ_JOIN, ""));
		}
		public void MatchFound(string matchInfo)
		{
			var arg = matchInfo.Split('|');
			var playerType = arg[0].Trim(' ');
			var enemyName = arg[1].Trim(' ');
			_matchTimeCountCancelToken?.Cancel();
			playerTileType = playerType switch
			{
				"O" => TileType.O,
				"X" => TileType.X,
				_ => playerTileType
			};
			enemyTileType = playerTileType == TileType.O ? TileType.X : TileType.O;
			UIManager.Instance.SetEnemyName(enemyName);
			UIManager.Instance.SetEnemyTypeText();
			UIManager.Instance.SetPlayerName(playerName);
			UIManager.Instance.SetPlayerTypeText();
			EventManager.Instance.PostNotification(EEventType.UIStateChange, this, EuiState.InGame);
			Debug.Log("My type : " + playerType + " | EnemyName : " + enemyName);
			state = EGameState.PreGame;
			StartCoroutine(GameStart());
		}
		IEnumerator GameStart()
		{
			turn = TileType.O;
			UIManager.Instance.SetCurrentTurnText();
			Debug.Log("3");
			yield return new WaitForSeconds(1);
			Debug.Log("2");
			yield return new WaitForSeconds(1);
			Debug.Log("1");
			yield return new WaitForSeconds(1);
			Debug.Log("Start!");
			EventManager.Instance.PostNotification(EEventType.GameStart, this);
			state = EGameState.InGame;
		}
		private void MatchStop()
		{
			_matchTimeCountCancelToken?.Cancel();
			state = EGameState.Lobby;
		}
		public void GameOver(TileType winnerTileType)
		{
			state = EGameState.PostGame;
			UIManager.Instance.SetResultInfo(winnerTileType);
			EventManager.Instance.PostNotification(EEventType.UIStateChange, this, EuiState.Result); 
		}
		private async void MatchMakingTimeCount()
		{
			Debug.Log("MatchMakingTimeCount");
			_matchTimeCountCancelToken?.Cancel();
			_matchTimeCountCancelToken = new CancellationTokenSource();
			var startTime = Time.time;
			try
			{
				while (true)
				{
					_matchTimeCountCancelToken.Token.ThrowIfCancellationRequested();
					await NetworkManager.Instance.Send(new Message(EMessageType.MT_ACTIVE_USER, ""));
					UIManager.Instance.SetMatchMakingText((int)(Time.time - startTime));
					await Task.Delay(1000);
				}
			}
			catch (OperationCanceledException)
			{
				UIManager.Instance.SetMatchMakingText(00);
			}
			finally
			{
				_matchTimeCountCancelToken.Dispose();
				_matchTimeCountCancelToken = null;
			}
		}
	
		private void ServerConnectionAction(EConnectResult connectResult)
		{
			switch (connectResult)
			{
				case EConnectResult.Success:
					state = EGameState.Lobby;
					break;
				case EConnectResult.TimeOut:
					state = EGameState.PreLogin;
					break;
				case EConnectResult.Disconnect:
					state = EGameState.PreLogin;
					MatchStop();
					break;
				case EConnectResult.Error:
					state = EGameState.PreLogin;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(connectResult), connectResult, null);
			}
		}
	}
}