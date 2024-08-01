using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using Util.EventSystem;

namespace UI
{
	public class AnimationUI : MonoBehaviour, IEventListener
	{
		[Header("ActionInfo")]
		[SerializeField] private ActionData defaultStateAction;
		[SerializeField] private List<ActionData> uiStateActions;

		private RectTransform _rect;

		public void Start()
		{
			EventManager.Instance.AddListener(EEventType.UIStateChange, this);
			_rect = GetComponent<RectTransform>();
			UIStateAction(UIManager.Instance.CurruntState);
		}

		public void OnEvent(EEventType eventType, Component sender, object param = null)
		{
			if (eventType != EEventType.UIStateChange || param == null)
				return;
			UIStateAction((EuiState)param);
		}
		
		private ActionData FindStateAction(EuiState uiState)
		{
			foreach (var action in uiStateActions.Where(action => action.targetState == uiState))
				return action;
			return defaultStateAction;
		}
		
		private void UIStateAction(EuiState uiState)
		{
			var actionData = FindStateAction(uiState);

			if (actionData.setActive)
				gameObject.SetActive(true);

			_rect.DOPause();
			_rect.DOAnchorPos(actionData.position, UIManager.AnimationTime);
			_rect.DOSizeDelta(actionData.size, UIManager.AnimationTime);
			_rect.DOScale(actionData.scale, UIManager.AnimationTime).OnComplete(() => 
			{
				if (actionData.setActive == false)
					gameObject.SetActive(false);
			});
		}
	}

	[Serializable]
	public class ActionData
	{
		[Header("State")]
		public EuiState targetState;

		[Header("Transform")]
		public Vector2 position;
		public Vector2 size;
		public Vector3 scale;

		[Header("Active")]
		public bool setActive;
	}
}