using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using Util.EventSystem;

namespace UI
{
	public class ToggleUI : MonoBehaviour
	{
		[SerializeField] private ActionData show;
		[SerializeField] private ActionData hide;
		private RectTransform _rect;

		public void Start()
		{
			_rect = GetComponent<RectTransform>();
			SetPosition(EVisualState.Hide);
		}

		public void Show()
		{
			SetPosition(EVisualState.Show);
		}

		public void Hide()
		{
			SetPosition(EVisualState.Hide);
		}

		private void SetPosition(EVisualState visualState)
		{
			var actionData = visualState == EVisualState.Show ? show : hide;

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

		public enum EVisualState
		{
			Show,
			Hide
		}
	}
}