using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ChivalryRCON.Util;

namespace ChivalryRCON.Page
{
	public class PageManager : SingletonMonoBehaviour<PageManager>
	{
		private const float TRANSITION_DURATION = 0.25f;
		private const float TRANSITION_SCALE = 2.0f;
		private const float INCH_PER_MILLIMETER = 25.4f;

		[SerializeField]
		private PageRoot initialPage;

		private List<PageRoot> stackPageList = new List<PageRoot> ();
		private PageRoot currentPage;

		void Awake ()
		{
			DOTween.Init ();

			// hide all children
			foreach (Transform child in transform) {
				child.gameObject.SetActive (false);
			}
		}

		IEnumerator Start ()
		{
			yield return null;
			pushPage (initialPage);
		}

		public void pushPage (PageRoot page)
		{
			StartCoroutine (pushPageCoroutine (page));
		}

		private IEnumerator pushPageCoroutine (PageRoot page)
		{
			// TODO: Add a UI Mask

			// disable new page at first;
			page.gameObject.SetActive (false);

			// outgoing
			if (currentPage != null) {
				StartCoroutine (pageOutgoingCoroutine (currentPage, calcPushOutPos (), 1.0f / TRANSITION_SCALE));
				currentPage = null;
			}

			// wait till the page is ready
			while (!page.isReady) {
				yield return null;
			}

			// incoming
			currentPage = page;
			stackPageList.Add (page);
			StartCoroutine (pageIncomingCoroutine (page, calcPushInPos (), TRANSITION_SCALE));

			while (isOutgoing || isIncoming) {
				yield return null;
			}

			// TODO: Remove a UI Mask
		}

		public void popPage ()
		{
			if (stackPageList.Count <= 1) {
				Debug.LogError ("No pages to pop");
				return;
			}
			
			StartCoroutine (popPageCoroutine ());
		}

		private IEnumerator popPageCoroutine ()
		{
			// TODO: Add a UI Mask

			// outgoing
			StartCoroutine (pageOutgoingCoroutine (currentPage, calcPopOutPos (), TRANSITION_SCALE));
			stackPageList.RemoveAt (stackPageList.Count - 1);

			PageRoot incomingPage = stackPageList [stackPageList.Count - 1];
			incomingPage.willPop ();
			// wait till the page is ready
			while (!incomingPage.isReady) {
				yield return null;
			}

			// incoming
			StartCoroutine (pageIncomingCoroutine (incomingPage, calcPopInPos (), 1.0f / TRANSITION_SCALE));

			while (isOutgoing || isIncoming) {
				yield return null;
			}

			currentPage = incomingPage;

			// TODO: Remove a UI Mask
		}
		

		private bool isOutgoing = false;

		private IEnumerator pageOutgoingCoroutine (PageRoot outgoingPage, Vector3 lastPos, float lastScale)
		{
			outgoingPage.willDisappear ();
			isOutgoing = true;
			outgoingPage.transform.DOLocalMove (lastPos, TRANSITION_DURATION)
				.SetEase (Ease.OutCubic)
					.OnComplete (delegate () {
						isOutgoing = false;
					});
			outgoingPage.transform.DOScale (lastScale, TRANSITION_DURATION)
				.SetEase (Ease.OutCubic);
			while (isOutgoing) {
				yield return null;
			}
			outgoingPage.gameObject.SetActive (false);
			outgoingPage.didDisappear ();
		}

		private bool isIncoming = false;

		private IEnumerator pageIncomingCoroutine (PageRoot incomingPage, Vector3 startPos, float startScale)
		{
			incomingPage.willAppear ();
			incomingPage.transform.localPosition = startPos;
			incomingPage.transform.localScale = new Vector3 (startScale, startScale, 1);
			incomingPage.gameObject.SetActive (true);
			isIncoming = true;
			incomingPage.transform.DOLocalMove (Vector3.zero, TRANSITION_DURATION)
				.SetEase (Ease.OutCubic)
					.OnComplete (delegate () {
						isIncoming = false;
					});
			incomingPage.transform.DOScale (Vector3.one, TRANSITION_DURATION)
				.SetEase (Ease.OutCubic);
			while (isIncoming) {
				yield return null;
			}
			incomingPage.didAppear ();
		}

		private Vector3 calcPushOutPos ()
		{
			float physicalWidth  = calcScreenPhysicalWidth ();
			return new Vector3 (-physicalWidth + (physicalWidth - physicalWidth / TRANSITION_SCALE) / 2.0f, 0, 0);
		}

		private Vector3 calcPushInPos ()
		{
			float physicalWidth  = calcScreenPhysicalWidth ();
			return new Vector3 (physicalWidth + (physicalWidth * TRANSITION_SCALE - physicalWidth) / 2.0f, 0, 0);
		}

		private Vector3 calcPopOutPos ()
		{
			float physicalWidth  = calcScreenPhysicalWidth ();
			return new Vector3 (physicalWidth + (physicalWidth * TRANSITION_SCALE - physicalWidth) / 2.0f, 0, 0);
		}

		private Vector3 calcPopInPos ()
		{
			float physicalWidth  = calcScreenPhysicalWidth ();
			return new Vector3 (-physicalWidth + (physicalWidth - physicalWidth / TRANSITION_SCALE) / 2.0f, 0, 0);
		}

		private float calcScreenPhysicalWidth ()
		{
			return Screen.width * INCH_PER_MILLIMETER / Screen.dpi;
		}

		private float calcScreenUnitWidth ()
		{
			float pixelsPerUnit = Screen.height / 2;
			float screenUnitWidth = Screen.width / pixelsPerUnit;
			return screenUnitWidth;
		}
	}
}