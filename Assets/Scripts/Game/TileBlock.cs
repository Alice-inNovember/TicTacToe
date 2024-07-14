using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using Util.EventSystem;

namespace Game
{
	public class TileBlock : MonoBehaviour, IEventListener
	{
		public List<Tile> Tiles => tiles;
		public TileType Type { get; private set; }
		[SerializeField] private List<Tile> tiles;
		[SerializeField] private GameObject panel;
		[SerializeField] public int id;
		[SerializeField] private TMP_Text text;
		[SerializeField] private GameObject focus;

		private bool _isComplete;
		public void OnEvent(EEventType eventType, Component sender, object param = null)
		{
			switch(eventType)
			{
				case EEventType.ProgramStart:
					break;
				case EEventType.ServerConnection:
					break;
				case EEventType.GameStart:
					_isComplete = false;
					SetBlock(TileType.Null, GameManager.Instance.turn == GameManager.Instance.playerTileType);
					break;
				case EEventType.Reset:
					SetBlock(TileType.Null, GameManager.Instance.turn == GameManager.Instance.playerTileType);
					break;
				case EEventType.PlayerTileClicked:
					break;
				case EEventType.EnemyTileClicked:
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(eventType), eventType, null);
			}
		}

		public void Start()
		{
			EventManager.Instance.AddListener(EEventType.GameStart, this);
			EventManager.Instance.AddListener(EEventType.Reset, this);

			for (var i = 0; i < 9 ; i++)
			{
				tiles[i].Init(new Vector2Int(id, i));
			}
		}

		public void CheckComplete()
		{
			var tileType = TileType.Null;
			for (var i = 0; i < 3; i++)
			{
				var h = 3 * i;
				var v = i;
				Debug.Log($"H {h} V {v}");
				
				//가로 세로 체크
				if (Tiles[h].Type == Tiles[h + 1].Type && Tiles[h].Type == Tiles[h + 2].Type)
					tileType = Tiles[h].Type;
				else if (Tiles[v].Type == Tiles[v + 3].Type && Tiles[v].Type == Tiles[v + 6].Type)
					tileType = Tiles[v].Type;

				if (tileType != TileType.Null)
					break;
			}
			if (tileType == TileType.Null)
			{
				//대각선
				if (Tiles[0].Type == Tiles[4].Type && Tiles[0].Type == Tiles[8].Type)
					tileType = Tiles[0].Type;
				else if (Tiles[2].Type == Tiles[4].Type && Tiles[2].Type == Tiles[6].Type)
					tileType = Tiles[2].Type;
			}
			//무승부
			if(tileType == TileType.Null && tiles.Count(tile => tile.Type == TileType.Null) == 0)
			{
				SetBlock(TileType.Null, false);
				_isComplete = true;
			} 
			//결과
			else if (tileType != TileType.Null)
			{
				SetBlock(tileType, false);
				_isComplete = true;
			}
			Debug.Log("Result _isComplete : " + _isComplete);
		}

		public bool IsComplete()
		{
			return _isComplete;
		}

		public void SetBlock(TileType type, bool interactable)
		{
			panel.SetActive(!interactable);
			Type = type;
			text.text = Type switch
			{
				TileType.Null => " ",
				TileType.O => "O",
				TileType.X => "X",
				_ => throw new ArgumentOutOfRangeException()
			};
		}

		public void SetFocusActive(bool value)
		{
			focus.SetActive(value);
		}

		public void SetTile(int id, TileType type, bool interactable)
		{
			Tiles[id].SetTile(type, interactable);
			Tiles[id].SetHighlight(true);
		}

		public void ResetHilight()
		{
			foreach (var tile in Tiles)
			{
				tile.SetHighlight(false);
			}
		}
	}
}