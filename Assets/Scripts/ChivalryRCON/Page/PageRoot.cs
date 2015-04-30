using UnityEngine;
using System.Collections;

namespace ChivalryRCON.Page
{
	public class PageRoot : MonoBehaviour
	{
		public bool isReady = true;

		public virtual void willPop ()
		{
			Debug.Log ("PageRoot virtual willPop");
		}

		public virtual void willAppear ()
		{
			Debug.Log ("PageRoot virtual willAppear");
		}

		public virtual void didAppear ()
		{
			Debug.Log ("PageRoot virtual didAppear");
		}
		
		public virtual void willDisappear ()
		{
			Debug.Log ("PageRoot virtual willDisappear");
		}
		
		public virtual void didDisappear ()
		{
			Debug.Log ("PageRoot virtual didDisappear");
		}
		
	}
}