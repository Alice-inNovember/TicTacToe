using System;
using Game;
using Network;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Util.EventSystem;
using Util.SingletonSystem;
using EventType = Util.EventSystem.EventType;

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
		[Header("UI Set")]
		[SerializeField] private GameObject menuUISet;
	
		[Header("Animation UI")]
		[SerializeField] private AnimationUI loginToStartB;
		[SerializeField] private AnimationUI startB;
		[SerializeField] private AnimationUI logoUI;
		[SerializeField] private AnimationUI creditUI;
		[SerializeField] private AnimationUI loginUI;
		[SerializeField] private AnimationUI matchMakingUI;
	
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
		[SerializeField] private AnimationUI inGameUI;
		[SerializeField] private TMP_Text currentTurnText;
		[SerializeField] private TMP_Text gameTimeText;
		[SerializeField] private TMP_Text enemyTypeText;
		[SerializeField] private TMP_Text enemyNameText;
		[SerializeField] private TMP_Text playerTypeText;
		[SerializeField] private TMP_Text playerNameText;

		[Header("ResultUI")]
		[SerializeField] private AnimationUI resultUI;
		[SerializeField] private TMP_Text resultText;
		[SerializeField] private Button toLobbyButton;

		public void SetResultText(bool playerWin)
		{
			resultText.text = playerWin ? "You Win" : "You Lose";
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
		
		public const float AnimationTime = 0.5f;

		private void Start()
		{
			EventManager.Instance.AddListener(EventType.ProgramStart, this);
			EventManager.Instance.AddListener(EventType.ServerConnection, this);
			toStartButton.onClick.AddListener(()=>SetUI(EuiState.Start));
			startButton.onClick.AddListener(()=> SetUI(EuiState.Login));
			loginButton.onClick.AddListener(LoginButton);
			matchButton.onClick.AddListener(GameManager.Instance.StartMachMaking);
			toLobbyButton.onClick.AddListener(() =>
			{
				SetUI(EuiState.Login);
				GameManager.Instance.SetGameToLogin();
				NetworkManager.Instance.DisconnectServer();
				EventManager.Instance.PostNotification(EventType.Reset, this);
			});
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
		public void OnEvent(EventType eventType, Component sender, object param = null)
		{
			switch (eventType)
			{
				case EventType.ProgramStart:
					break;
				case EventType.ServerConnection:
					if (param != null)
						ServerConnectionAction((EConnectResult)param);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null);
			}
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
					SetUI(EuiState.Lobby);
					loginResultText.text = "Server Connect Success";
					break;
				case EConnectResult.TimeOut:
					SetUI(EuiState.Login);
					loginResultText.text = "Server TimeOut, Try Again";
					break;
				case EConnectResult.Disconnect:
					SetUI(EuiState.Login);
					loginResultText.text = "Server Disconnect, Try Again";
					break;
				case EConnectResult.Error:
					SetUI(EuiState.Login);
					loginResultText.text = "Server Error, Try Again";
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(result), result, null);
			}
		}
		public void SetUI(EuiState state, object param = null)
		{
			switch (state)
			{
				case EuiState.Start:
					menuUISet.SetActive(true);
					loginToStartB.Hide();
					startB.Show();
					logoUI.Show();
					creditUI.Show();
					loginUI.Hide();
					inGameUI.Hide();
					resultUI.Hide();
					matchMakingUI.Hide();
					break;
				case EuiState.Login:
					menuUISet.SetActive(true);
					loginToStartB.Show();
					startB.Hide();
					logoUI.Hide();
					creditUI.Hide();
					loginUI.Show();
					inGameUI.Hide();
					resultUI.Hide();
					matchMakingUI.Hide();
					break;
				case EuiState.Lobby:
					SetActiveUserText("--");
					menuUISet.SetActive(true);
					loginToStartB.Hide();
					startB.Hide();
					logoUI.Hide();
					creditUI.Hide();
					loginUI.Hide();
					inGameUI.Hide();
					resultUI.Hide();
					matchMakingUI.Show();
					break;
				case EuiState.InGame:
					menuUISet.SetActive(false);
					inGameUI.Show();
					resultUI.Hide();
					break;
				case EuiState.Result:
					SetResultText((bool)param);
					menuUISet.SetActive(false);
					inGameUI.Hide();
					resultUI.Show();
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(state), state, null);
			}
		}
	}
}