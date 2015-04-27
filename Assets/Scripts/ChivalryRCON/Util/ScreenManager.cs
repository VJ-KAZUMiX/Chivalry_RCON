using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Threading;

namespace ChivalryRCON.Util
{
	/// <summary>
	/// Screen manager for Power Saving
	/// </summary>
	public class ScreenManager : MonoBehaviour
	{
		private enum PerformanceMode {
			Normal,
			PowerSaving
		}

		private PerformanceMode currentPerformanceMode = PerformanceMode.Normal;

		[SerializeField]
		private GameObject captureButton;

		[SerializeField]
		private RawImage capturedRawImage;

		[SerializeField]
		private CanvasGroup mainContentsContainer;

		void Awake ()
		{
			capturedRawImage.enabled = false;
		}
		
		public void captureHandler ()
		{
			changePerformanceMode (PerformanceMode.PowerSaving);
		}

		IEnumerator captureScreen ()
		{
			yield return new WaitForEndOfFrame ();
			int width = Screen.width;
			int height = Screen.height;
			Texture2D tex = new Texture2D (width, height, TextureFormat.RGB24, false);
			tex.ReadPixels (new Rect (0, 0, width, height), 0, 0);
			tex.Apply ();
			capturedRawImage.texture = tex;
		}

		private IEnumerator startPowerSaving ()
		{
			yield return StartCoroutine (captureScreen ());
			mainContentsContainer.alpha = 0;
			capturedRawImage.enabled = true;
		}

		void Update ()
		{
			if (Input.GetMouseButtonDown (0)) {
				if (EventSystem.current.currentSelectedGameObject == captureButton) {
					return;
				}
				changePerformanceMode (PerformanceMode.Normal);
			}
		}

		private void changePerformanceMode (PerformanceMode performanceMode)
		{
			if (currentPerformanceMode == performanceMode) {
				return;
			}
			Debug.Log (performanceMode);

			switch (performanceMode) {
			case PerformanceMode.Normal:
				mainContentsContainer.alpha = 1;
				capturedRawImage.enabled = false;
				QualitySettings.vSyncCount = 1;
				Application.targetFrameRate = 60;
				break;

			case PerformanceMode.PowerSaving:
				StartCoroutine (startPowerSaving ());
				QualitySettings.vSyncCount = 0;
				Application.targetFrameRate = 10;
				break;
			}

			currentPerformanceMode = performanceMode;
		}
	}
}