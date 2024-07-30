using System;
using Game;
using Network;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Util.EventSystem;
using Util.SingletonSystem;

namespace UI
{
	public enum EuiState
	{
		Start,
		Login,
		Lobby,
		InGame,
		Result
	}
	public class UIManager : MonoBehaviourSingleton<UIManager>, IEventListener
	{
		[Header("Info")]
		[SerializeField] private EuiState curruntState;
		
		[Header("Buttons")]
		[SerializeField] private Button toStartButton;
		[SerializeField] private Button startButton;
		[SerializeField] private Button loginButton;
		[SerializeField] private Button matchButton;
	
		[Header("Texts")]
		[SerializeField] private TMP_Text loginResultText;
		[SerializeField] private TMP_Text matchTimeText;
		[SerializeField] private TMP_Text activeUserText;
	
		[Header("InputFields")]
		[SerializeField] private TMP_InputField loginInput;

		[Header("InGameUI")]
		[SerializeField] private TMP_Text currentTurnText;
		[SerializeField] private TMP_Text gameTimeText;
		[SerializeField] private TMP_Text enemyTypeText;
		[SerializeField] private TMP_Text enemyNameText;
		[SerializeField] private TMP_Text playerTypeText;
		[SerializeField] private TMP_Text playerNameText;

		[Header("ResultUI")]
		[SerializeField] private TMP_Text resultText;
		[SerializeField] private Button toLobbyButton;

		public const float AnimationTime = 0.5f;

		private void Start()
		{
			EventManager.Instance.AddListener(EEventType.ProgramStart, this);
			EventManager.Instance.AddListener(EEventType.ServerConnection, this);
			EventManager.Instance.AddListener(EEventType.UIStateChange, this);
			toStartButton.onClick.AddListener(()=>EventManager.Instance.PostNotification(EEventType.UIStateChange, this, EuiState.Start));
			startButton.onClick.AddListener(()=> EventManager.Instance.PostNotification(EEventType.UIStateChange, this, EuiState.Login));
			loginButton.onClick.AddListener(LoginButton);
			matchButton.onClick.AddListener(MatchMaking.Instance.StartMachMaking);
			toLobbyButton.onClick.AddListener(() =>
			{
				EventManager.Instance.PostNotification(EEventType.UIStateChange, this, EuiState.Login);
				GameManager.Instance.SetGameState(EGameState.PreLogin);
				NetworkManager.Instance.DisconnectServer();
				EventManager.Instance.PostNotification(EEventType.Reset, this);
			});
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
				case EEventType.UIStateChange:
					if (param != null)
						curruntState = (EuiState)param;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null);
			}
		}
		
		public void SetCurrentTurnText()
		{
			var type = GameManager.Instance.turn == TileType.O ? "O" : "X";
			currentTurnText.text = new string($"{type}'s\nTurn");
		}
		public void SetGameTimeText(int second)
		{
			var min = (second / 60).ToString("00");
			var sec = (second % 60).ToString("00");
			var text = new string($"Time\n{min}:{sec}");
			gameTimeText.text = text;
		}
		public void SetEnemyTypeText()
		{
			var type = GameManager.Instance.enemyTileType == TileType.O ? "O" : "X";
			enemyTypeText.text = type;
		}
		public void SetEnemyName(string enemyName)
		{
			enemyNameText.text = enemyName;
		}
		public void SetPlayerTypeText()
		{
			var type = GameManager.Instance.playerTileType == TileType.O ? "O" : "X";
			playerTypeText.text = type;
		}
		public void SetPlayerName(string playerName)
		{
			playerNameText.text = playerName;
		}
		private void LoginButton()
		{
			if (loginInput.text.Length == 0)
			{
				loginResultText.text = "Enter Name And Try Again";
				return;
			}
			loginResultText.text = "Connecting...";
			GameManager.Instance.playerName = loginInput.text;
			NetworkManager.Instance.TryConnectServer(loginInput.text);
		}
		
		public void SetActiveUserText(string number)
		{
			activeUserText.text = "ActiveUser : " + number;
		}
		public void SetMatchMakingText(int second)
		{
			matchTimeText.text = "MatchQ  " + (second / 60).ToString("00") + ":" + (second % 60).ToString("00");
		}
		private void ServerConnectionAction(EConnectResult result)
		{
			switch (result)
			{
				case EConnectResult.Success:
					loginResultText.text = "Server Connect Success";
					EventManager.Instance.PostNotification(EEventType.UIStateChange, this, EuiState.Lobby);
					break;
				case EConnectResult.TimeOut:
					loginResultText.text = "Server TimeOut, Try Again";
					EventManager.Instance.PostNotification(EEventType.UIStateChange, this, EuiState.Login);
					break;
				case EConnectResult.Disconnect:
					loginResultText.text = "Server Disconnect, Try Again";
					EventManager.Instance.PostNotification(EEventType.UIStateChange, this, EuiState.Login);
					break;
				case EConnectResult.Error:
					loginResultText.text = "Server Error, Try Again";
					EventManager.Instance.PostNotification(EEventType.UIStateChange, this, EuiState.Login);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(result), result, null);
			}
		}
		public void SetResultInfo(TileType winner)
		{
			if (winner == TileType.Null)
				resultText.text = "Draw";
			else
				resultText.text = (winner == GameManager.Instance.playerTileType ? "You Win" : "You Lose");
		}
	}
}