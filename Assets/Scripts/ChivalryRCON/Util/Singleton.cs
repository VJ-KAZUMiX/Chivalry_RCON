using UnityEngine;
using System.Collections;

namespace ChivalryRCON.Util {

	public class Singleton<T> where T : class, new ()
	{
		public static readonly T Instance = new T ();
	}
}