using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Classes.Managers {
	public class EnvironmentManager : MonoBehaviour
    {
        public static EnvironmentManager Instance  {get; private set;}

        [SerializeField] private GameObject     meadow;
        [SerializeField] private GameObject     balloonEnclosure; 
                         private GameSettingsSO gameSettings; 

        private void Awake()
        {
            /* Singleton pattern make sure there is only one environment manager. */
		    if (Instance != null && Instance != this) {
                Destroy(this);
		    } else {
                Instance = this;
            } 
        }

        // Start is called before the first frame update
        private void Start()
        {
            this.gameSettings = BalloonGameplayManager.Instance.gameSettings;

            if (this.gameSettings.environment == GameSettingsSO.Environment.ORIGINAL) {
                Instantiate(this.balloonEnclosure);
            } else {
                Instantiate(this.meadow);
            }
        }
    }
}

