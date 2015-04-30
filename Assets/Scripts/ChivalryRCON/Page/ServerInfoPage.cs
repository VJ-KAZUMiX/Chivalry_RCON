using UnityEngine;
using System.Collections;

namespace ChivalryRCON.Page
{
	public class ServerInfoPage : PageRoot
	{
		public override void willAppear ()
		{
			Debug.Log ("ServerInfoPage willAppear");
		}

		public void testBackButtonHandler ()
		{
			PageManager.Instance.popPage ();
		}
	}
}