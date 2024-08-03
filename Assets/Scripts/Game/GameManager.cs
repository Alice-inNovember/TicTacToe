using System;
using Audio;
using Network;
using UI;
using UnityEngine;
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
		PostGame
	}

	public class GameManager : MonoBehaviourSingleton<GameManager>, IEventListener
	{
		public EGameState state;
		public TileType playerTileType;
		public TileType enemyTileType;
		public TileType turn;
		public string playerName;
		public float gameTimerTime;

		private void Start()
		{
			EventManager.Instance.AddListener(EEventType.ServerConnection, this);
			Application.runInBackground = true;
			state = EGameState.PreLogin;
			playerTileType = TileType.Null;
		}

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

		public void SetGameState(EGameState gameState)
		{
			state = gameState;
		}

		public void GameStart()
		{
			turn = TileType.O;
			UIManager.Instance.SetCurrentTurnText();
			Debug.Log("Start!");
			EventManager.Instance.PostNotification(EEventType.GameStart, this);
			state = EGameState.InGame;
		}

		public void GameOver(TileType winnerTileType)
		{
			state = EGameState.PostGame;
			if (winnerTileType == TileType.Null)
				SoundSystem.Instance.PlaySFX(ESoundClip.GameDraw);
			if (winnerTileType == playerTileType)
				SoundSystem.Instance.PlaySFX(ESoundClip.GameWin);
			else
				SoundSystem.Instance.PlaySFX(ESoundClip.GameLose);
			UIManager.Instance.SetResultInfo(winnerTileType);
			EventManager.Instance.PostNotification(EEventType.UIStateChange, this, EuiState.Result);
			EventManager.Instance.PostNotification(EEventType.GameOver, this, EuiState.Result);
			NetworkManager.Instance.DisconnectServer();
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
					MatchMaking.Instance.MatchStop();
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