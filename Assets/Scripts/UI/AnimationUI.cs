using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using Util.EventSystem;

namespace UI
{
	public class AnimationUI : MonoBehaviour, IEventListener
	{
		[SerializeField] private AnimationUIActionData actionData;
		private RectTransform _rect;
		[SerializeField] private Vector2 showPosition;
		[SerializeField] private Vector2 showSize;
		[SerializeField] private Vector3 showScale;
		[SerializeField] private Vector2 hidePosition;
		[SerializeField] private Vector2 hideSize;
		[SerializeField] private Vector3 hideScale;
		[SerializeField] private bool doChangeActive;
		[SerializeField] private bool doChangeTransform;

		public void Start()
		{
			EventManager.Instance.AddListener(EEventType.UIStateChange, this);
			_rect = GetComponent<RectTransform>();
		}

		public void OnEvent(EEventType eventType, Component sender, object param = null)
		{
			if (eventType != EEventType.UIStateChange || param == null)
				return;
			UnityAction action = actionData.GetUIVisualState((EuiState)param) == UIVisualState.Show ? Show : Hide;
			action();
		}
		public void Show()
		{
			if (actionData.doChangeTransform == false)
			{
				_rect.transform.gameObject.SetActive(true);
				return;
			}
			
			_rect.DOPause();
			_rect.transform.gameObject.SetActive(true);
			_rect.DOAnchorPos(actionData.showPosition, UIManager.AnimationTime);
			_rect.DOSizeDelta(actionData.showSize, UIManager.AnimationTime);
			_rect.DOScale(actionData.showScale, UIManager.AnimationTime);
		}

		public void Hide()
		{
			if (actionData.doChangeTransform == false)
			{
				if (actionData.doChangeActive)
					_rect.transform.gameObject.SetActive(false);
				return;
			}
			
			_rect.DOPause();
			_rect.DOAnchorPos(actionData.hidePosition, UIManager.AnimationTime).OnComplete(() => 
			{
				if (actionData.doChangeActive)
					_rect.transform.gameObject.SetActive(false);
			});
			_rect.DOSizeDelta(actionData.hideSize, UIManager.AnimationTime);
			_rect.DOScale(actionData.hideScale, UIManager.AnimationTime);
		}
	}
}