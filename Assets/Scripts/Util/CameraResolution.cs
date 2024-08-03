using UnityEngine;

namespace Util
{
	public class CameraResolution : MonoBehaviour
	{
		[SerializeField] private int widthRatio = 16;
		[SerializeField] private int hightRatio = 9;
		private void Start()
		{
			var thisCamera = GetComponent<Camera>();
			var rect = thisCamera.rect;
			var scaleHeight = (float)Screen.width / Screen.height / ((float)widthRatio / hightRatio);// (가로 / 세로)
			var scaleWidth = 1f / scaleHeight;
			if (scaleHeight < 1)
			{
				rect.height = scaleHeight;
				rect.y = (1f - scaleHeight) / 2f;
			}
			else
			{
				rect.width = scaleWidth;
				rect.x = (1f - scaleWidth) / 2f;
			}

			thisCamera.rect = rect;
		}

		private void OnPreCull()
		{
			GL.Clear(true, true, Color.black);
		}
	}
}