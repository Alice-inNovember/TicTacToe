using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Util.EventSystem;
using EventType = Util.EventSystem.EventType;

namespace Game
{
	public class Tile : MonoBehaviour, IEventListener
	{
		public TileType Type { get; private set; }
		private Vector2Int _id;
		private Button _button;
		private TMP_Text _text;
		
		public void OnEvent(EventType eventType, Component sender, object param = null)
		{
			switch (eventType)
			{
				case EventType.ProgramStart:
					break;
				case EventType.ServerConnection:
					break;
				case EventType.GameStart:
					SetTile( TileType.Null, true);
					break;
				case EventType.Reset:
					SetTile(TileType.Null, true);
					break;
				case EventType.PlayerTileClicked:
					break;
				case EventType.EnemyTileClicked:
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null);
			}
		}

		private void Start()
		{
			EventManager.Instance.AddListener(EventType.GameStart, this);
			EventManager.Instance.AddListener(EventType.Reset, this);
		}

		public void Init(Vector2Int id)
		{
			_id = id;
			_text = GetComponentInChildren<TMP_Text>();
			_button = GetComponent<Button>();
			_button.onClick.AddListener(OnButtonClick);
		}

		private void OnButtonClick()
		{
			Debug.Log("OnButtonClick" + _id.x + _id.y);
			EventManager.Instance.PostNotification(EventType.PlayerTileClicked, this, _id);
		}

		public void SetTile(TileType type, bool interactable)
		{
			_button.interactable = interactable;
			Type = type;
			_text.text = Type switch
			{
				TileType.Null => " ",
				TileType.O => "O",
				TileType.X => "X",
				_ => throw new ArgumentOutOfRangeException()
			};
		}
	}
}