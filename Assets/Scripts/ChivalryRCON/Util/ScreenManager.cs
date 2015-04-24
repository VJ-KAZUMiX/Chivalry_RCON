using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace ChivalryRCON.Util
{
	public class ScreenManager : MonoBehaviour
	{
		[SerializeField]
		private RawImage targetRawImage;

		public void captureHandler ()
		{
			StartCoroutine (captureScreen ());
		}

		IEnumerator captureScreen ()
		{
			yield return new WaitForEndOfFrame ();
			int width = Screen.width;
			int height = Screen.height;
			Texture2D tex = new Texture2D (width, height, TextureFormat.RGB24, false);
			tex.ReadPixels (new Rect (0, 0, width, height), 0, 0);
			tex.Apply ();
			targetRawImage.texture = tex;
		}

	}
}