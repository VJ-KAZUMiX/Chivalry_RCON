using UnityEngine;
using System.Collections;

namespace ChivalryRCON.Page
{
	public class PageRoot : MonoBehaviour
	{

		public virtual void willAppear ()
		{
			Debug.Log ("virtual willAppear");
		}

		public virtual void didAppear ()
		{
			Debug.Log ("virtual didAppear");
		}
		
		public virtual void willDisappear ()
		{
			Debug.Log ("virtual willDisappear");
		}
		
		public virtual void didDisappear ()
		{
			Debug.Log ("virtual didDisappear");
		}
		
	}
}