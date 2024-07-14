using DG.Tweening;
using UnityEngine;

namespace UI
{
	public class AnimationUI : MonoBehaviour
	{
		[SerializeField] private Vector2 showPosition;
		[SerializeField] private Vector2 showSize;
		[SerializeField] private Vector3 showScale;
		[SerializeField] private Vector2 hidePosition;
		[SerializeField] private Vector2 hideSize;
		[SerializeField] private Vector3 hideScale;
		[SerializeField] private bool doChangeActive;
	
		public void Show()
		{
			var rect = GetComponent<RectTransform>();
			rect.transform.gameObject.SetActive(true);
			rect.DOPause();
			rect.DOAnchorPos(showPosition, UIManager.AnimationTime);
			rect.DOSizeDelta(showSize, UIManager.AnimationTime);
			rect.DOScale(showScale, UIManager.AnimationTime);
		}

		public void Hide()
		{
			var rect = GetComponent<RectTransform>();
			rect.DOPause();
			rect.DOAnchorPos(hidePosition, UIManager.AnimationTime).OnComplete(() => 
			{
				if (doChangeActive)
				{
					rect.transform.gameObject.SetActive(false);
				}
			});
			rect.DOSizeDelta(hideSize, UIManager.AnimationTime);
			rect.DOScale(hideScale, UIManager.AnimationTime);
		}
	}
}