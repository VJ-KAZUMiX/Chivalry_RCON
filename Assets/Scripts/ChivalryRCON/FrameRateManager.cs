using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

namespace ChivalryRCON.Scene
{
	public class FrameRateManager : MonoBehaviour
	{
		IEnumerator Start ()
		{
			while (true) {
				if (Input.GetMouseButton (0)) {
					enableHighPerformance (true);
				} else {
					enableHighPerformance (false);
				}
				yield return new WaitForSeconds (0.1f);
			}
		}

		private bool isHightPerformance = true;
		
		private void enableHighPerformance (bool isEnable) {
			if (isHightPerformance == isEnable) {
				return;
			}
			if (isEnable) {
				QualitySettings.vSyncCount = 1;
				Application.targetFrameRate = 60;
			} else {
				QualitySettings.vSyncCount = 0;
				Application.targetFrameRate = 1;
			}

			isHightPerformance = isEnable;
		}


	}
}