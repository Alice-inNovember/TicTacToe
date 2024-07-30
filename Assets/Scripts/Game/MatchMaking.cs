using System;
using System.Threading;
using System.Threading.Tasks;
using Network;
using UI;
using UnityEngine;
using Util.EventSystem;
using Util.SingletonSystem;

namespace Game
{
	public class MatchMaking : MonoBehaviourSingleton<MatchMaking>
	{
		private CancellationTokenSource _matchTimeCountCancelToken;
		public async void StartMachMaking()
		{
			if (GameManager.Instance.state != EGameState.Lobby)
				return;
			GameManager.Instance.state = EGameState.MatchMaking;
			MatchMakingTimeCount();
			await NetworkManager.Instance.Send(new Message(EMessageType.MT_MATCHQ_JOIN, ""));
		}
		
		public void MatchFound(string matchInfo)
		{
			var arg = matchInfo.Split('|');
			var playerType = arg[0].Trim(' ');
			var enemyName = arg[1].Trim(' ');
			_matchTimeCountCancelToken?.Cancel();
			GameManager.Instance.playerTileType = playerType switch
			{
				"O" => TileType.O,
				"X" => TileType.X,
				_ => GameManager.Instance.playerTileType
			};
			GameManager.Instance.enemyTileType = GameManager.Instance.playerTileType == TileType.O ? TileType.X : TileType.O;
			UIManager.Instance.SetEnemyName(enemyName);
			UIManager.Instance.SetEnemyTypeText();
			UIManager.Instance.SetPlayerName(GameManager.Instance.playerName);
			UIManager.Instance.SetPlayerTypeText();
			EventManager.Instance.PostNotification(EEventType.UIStateChange, this, EuiState.InGame);
			Debug.Log("My type : " + playerType + " | EnemyName : " + enemyName);
			GameManager.Instance.state = EGameState.PreGame;
			GameManager.Instance.GameStart();
		}
		public void MatchStop()
		{
			_matchTimeCountCancelToken?.Cancel();
			GameManager.Instance.state = EGameState.Lobby;
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
	}
}