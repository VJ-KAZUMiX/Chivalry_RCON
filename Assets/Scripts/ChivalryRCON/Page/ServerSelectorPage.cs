using UnityEngine;
using System.Collections;

namespace ChivalryRCON.Page
{
	public class ServerSelectorPage : PageRoot
	{
		[SerializeField]
		private ServerInfoPage serverInfoPage;

		public void testButtonClickHander ()
		{
			PageManager.Instance.pushPage (serverInfoPage);
		}
	}
}