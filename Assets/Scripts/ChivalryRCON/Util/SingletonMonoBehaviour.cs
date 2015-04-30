using UnityEngine;
using System.Collections;

 namespace ChivalryRCON.Util {

	public class SingletonMonoBehaviour<T> : MonoBehaviour where T : MonoBehaviour
	{
		// private.
		private static T _instance = null;

		public virtual void Awake ()
		{
			if (null != _instance) {
				if (_instance.gameObject != this.gameObject) {
					Destroy (this.gameObject);
				}
			}
		}

		public static T Instance {
			get {
				if (null == _instance) {
					_instance = (T)FindObjectOfType(typeof(T));

					if (null == _instance) {
						Debug.LogWarning (typeof(T) + " is nothing");
					}
				} else {

				}
				return _instance;
			}
		}
	}
}