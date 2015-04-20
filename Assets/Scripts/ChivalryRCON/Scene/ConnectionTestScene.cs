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
			server = new Server ("192.168.1.192", 27015, "AdminPassword");
			server.connet ();
		}
	}
}