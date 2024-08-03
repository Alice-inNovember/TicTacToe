using System.Collections;
using UI;
using UnityEngine;
using Util.EventSystem;

namespace Game
{
	public class TurnTimer : MonoBehaviour, IEventListener
	{
		public float XTime { get; private set; }

		public float OTime { get; private set; }
		
		private Coroutine timer;

		private void Start()
		{
			EventManager.Instance.AddListener(EEventType.GameStart, this);
			EventManager.Instance.AddListener(EEventType.Reset, this);
			EventManager.Instance.AddListener(EEventType.TurnSwap, this);
		}

		public void OnEvent(EEventType eventType, Component sender, object param = null)
		{
			switch (eventType)
			{
				case EEventType.GameStart:
					XTime = GameManager.Instance.gameTimerTime;
					OTime = GameManager.Instance.gameTimerTime;
					timer = StartCoroutine(Timer(TileType.O));
					break;
				case EEventType.GameOver:
					StopCoroutine(timer);
					break;
				case EEventType.TurnSwap:
					SwapTimer();
					break;
			}
		}

		private void SwapTimer()
		{
			StopCoroutine(timer);
			StartCoroutine(Timer(GameManager.Instance.turn));
		}

		private IEnumerator Timer(TileType TargetType)
		{
			yield return new WaitForSeconds(0.5f);
			while (GameManager.Instance.state == EGameState.InGame)
			{
				if (GameManager.Instance.turn != TargetType)
					yield break;
				yield return new WaitForSeconds(0.25f);
				switch (TargetType)
				{
					case TileType.O:
						OTime -= 0.25f;
						break;
					case TileType.X:
						XTime -= 0.25f;
						break;
				}
				UIManager.Instance.SetTimerText((int)OTime, (int)XTime);

				if (OTime <= 0)
					GameManager.Instance.GameOver(TileType.X);
				if (XTime <= 0)
					GameManager.Instance.GameOver(TileType.O);
			}
		}
	}
}