using System;
using Audio;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Util.EventSystem;

namespace Game
{
	public class Tile : MonoBehaviour, IEventListener, IPointerEnterHandler, IPointerExitHandler
	{
		private Button _button;
		private Vector2Int _id;
		private TMP_Text _text;
		public TileType Type { get; private set; }

		private void Start()
		{
			EventManager.Instance.AddListener(EEventType.GameStart, this);
			EventManager.Instance.AddListener(EEventType.Reset, this);
		}

		public void OnEvent(EEventType eventType, Component sender, object param = null)
		{
			switch (eventType)
			{
				case EEventType.ProgramStart:
					break;
				case EEventType.ServerConnection:
					break;
				case EEventType.GameStart:
					SetTile(TileType.Null, true);
					break;
				case EEventType.Reset:
					SetTile(TileType.Null, true);
					break;
				case EEventType.PlayerTileClicked:
					break;
				case EEventType.EnemyTileClicked:
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null);
			}
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			EventManager.Instance.PostNotification(EEventType.TilePointerEnter, this, _id);
			SoundSystem.Instance.PlaySFX(ESoundClip.TileEnter);
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			EventManager.Instance.PostNotification(EEventType.TilePointerExit, this);
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
			EventManager.Instance.PostNotification(EEventType.PlayerTileClicked, this, _id);
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

		public void SetHighlight(bool value)
		{
			GetComponentInChildren<TMP_Text>().color = value ? new Color(0, 1, 0.677f) : Color.white;
		}
	}
}