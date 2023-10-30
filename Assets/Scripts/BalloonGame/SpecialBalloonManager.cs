using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Classes.Managers {
	public class SpecialBalloonManager : MonoBehaviour
	{
		/* Singleton pattern. Holds a reference to the balloon manager object. */
		public static SpecialBalloonManager Instance {get; private set;}

		private void Awake()
		{
			/* Singleton pattern make sure there is only one balloon manager. */
			if (Instance != null) {
				Debug.LogError("There should only be one special balloon manager.");
			}
			Instance = this; 

			Debug.Log("Special balloon manager active.");
		}
	}
}

