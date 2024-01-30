using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BalloonsGame 
{
	public class GameManager : MonoBehaviour
    {
        public static GameManager Instance {get; private set;}

        [SerializeField] protected GameSettingsSO gameSettings;

        private void Awake()
        {
            /* Singleton pattern make sure there is only one balloon manager. */
		    if (Instance != null) {
			    Debug.LogError("There should only be one balloon manager.");
                Destroy(this);
		    } else {
                Instance = this;
            }  

            Debug.Log("Balloon game manager active.");
        }

        private void Start()
        {
            /* Cast to the appropriate game manager. */
            if (gameSettings.gameMode == GameSettingsSO.GameMode.CAREER) {
                //Instance = (CareerGameManager) Instance;
                //Instance = Instantiate(CareerGameManager);
            } else if (gameSettings.gameMode == GameSettingsSO.GameMode.CUSTOM) {
                //Instance = (CustomGameManager) Instance;
                //Instance = Instantiate(CustomGameManager);
                this.PlayCustomGame();

            } else {
                Debug.LogError("Invalid game mode.");
            }
        }

        private void PlayCustomGame()
		{
			if (this.gameSettings.maxLives < 50) {
				PlayerManager.Instance.OnAllLivesLost += this.AllLivesLostHandler;
			}

			PointsManager.Instance.OnGoalReached += this.GoalReachedHandler;

			BalloonSpawnManager.Instance.StartAutomaticSpawner(3.0f);
		}

        protected void Restart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        protected void AllLivesLostHandler(object sender, EventArgs e)
		{
			this.Restart();
		}

        protected void GoalReachedHandler(object sender, EventArgs e)
        {
            this.Restart();
        }

        public GameSettingsSO GetGameSettings()
        {
            return this.gameSettings;
        }
    }
}

