using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BalloonsGame 
{
	public class BalloonGameManager : MonoBehaviour
    {
        public static BalloonGameManager Instance {get; private set;}

        [SerializeField] protected GameSettingsSO gameSettings;

        private void Awake()
        {
            /* Singleton pattern make sure there is only one balloon manager. */
		    if (Instance != null) {
			    Debug.LogError("There should only be one balloon manager.");
                Destroy(this);
		    } else {
                Instance = this;

                if (Instance.gameSettings.gameMode == GameSettingsSO.GameMode.CAREER) {
                    Instance = (CareerGameManager) Instance;
                } else if (Instance.gameSettings.gameMode == GameSettingsSO.GameMode.CUSTOM) {
                    Instance = (CustomGameManager) Instance;
                } else {
                    Debug.LogError("Invalid game mode.");
                }
            }  

            Debug.Log("Balloon game manager active.");
        }
    }
}

