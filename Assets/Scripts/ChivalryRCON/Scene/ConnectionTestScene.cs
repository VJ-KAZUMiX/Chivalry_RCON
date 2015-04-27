using UnityEngine;
using System.Collections;

using ChivalryRCON.Network;

namespace ChivalryRCON.Scene
{
	public class ConnectionTestScene : MonoBehaviour
	{
		private Server server;


		void Start ()
		{
			server = new Server ("192.168.100.189", 28017, "AdminPassword");
			server.connet ();
		}
	}
}