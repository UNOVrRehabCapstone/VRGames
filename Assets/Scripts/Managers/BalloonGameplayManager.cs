using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Platform.Samples.VrHoops;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Classes.Managers
{
    public class BalloonGameplayManager : GameplayManager
    {
        public static BalloonGameplayManager Instance {get; private set;}

        /*TODO: Note that this is currently being set in the editor for testing purposes, but 
                this needs to be changed so that the settings are determined based on whatever
                game mode the user selects. */
        [SerializeField] private GameSettingsSO gameSettings;
        private bool                            isRestarting = false;
        public  int                             playerLives; /* TODO: Temporary; needs to be placed in another file */

        private void Awake()
	    {
            //Singleton pattern make sure there is only one balloon gameplay manager.
		    if (Instance != null) {
			    Debug.LogError("There should only be one balloon gameplay manager.");
		    }
		    Instance = this;
	    }

        new void Start()
        {
            base.Start();

            Debug.Log("Game mode set to " +     this.gameSettings.gameMode.ToString());
            Debug.Log("Spawn pattern set to " + this.gameSettings.spawnPattern.ToString());

            this.playerLives = this.gameSettings.numLives;
            
            switch(this.gameSettings.gameMode) {
                /* RELAXED: Just watch the score.*/
                case GameSettingsSO.GameMode.RELAXED:
                    this.StartCoroutine(this.WatchScore());
                    break;

                /* NORMAL: Watch lives and score.*/
                case GameSettingsSO.GameMode.NORMAL:
                    this.StartCoroutine(this.WatchScore());
                    this.StartCoroutine(this.WatchPlayerLives());
                    break;

                /* ENDLESS: Just watch lives. */
                case GameSettingsSO.GameMode.ENDLESS:
                    this.StartCoroutine(this.WatchPlayerLives());
                    break;
                default:
                    Debug.LogError("This should never happen.");
                    break;

            }
            
            //PointsManager.addPointTrigger("==", this.gameSettings.goal, "onWinConditionPointsReached");
            PointsManager.updateScoreboardMessage("Press The Buttons Behind You To Spawn A Dart!");
        }

        void FixedUpdate()
        {
            if (!isRestarting) {
                /* If you wanted to log the game mode and spawn pattern on every frame */
                //Debug.Log("Game mode set to " +     this.gameSettings.gameMode.ToString());
                //Debug.Log("Spawn pattern set to " + this.gameSettings.spawnPattern.ToString());
                BalloonManager.Instance.SpawnBalloons();
            }
        }

        public GameSettingsSO GetGameSettings()
        {
            return this.gameSettings;
        }
        
        public IEnumerator Restart()
        {
            this.isRestarting = true;
            BalloonManager.Instance.KillAllBalloons();

            yield return new WaitForSeconds(5);
            PointsManager.updateScoreboardMessage("Restarting in: 3");
            yield return new WaitForSeconds(1);
            PointsManager.updateScoreboardMessage("Restarting in: 2");
            yield return new WaitForSeconds(1);
            PointsManager.updateScoreboardMessage("Restarting in: 1");
            yield return new WaitForSeconds(1);
            PointsManager.resetPoints();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private IEnumerator WatchPlayerLives()
        {
            yield return new WaitUntil(() => (this.playerLives < 1));

            Debug.Log("Out of lives");
            PointsManager.updateScoreboardMessage("Out of lives"); 
            this.StartCoroutine(this.Restart());
        }

        private IEnumerator WatchScore()
        {
            yield return new WaitUntil(() => (PointsManager.getPoints() == this.gameSettings.goal));

            Debug.Log("Goal has been reached!");
            PointsManager.updateScoreboardMessage("You Win!"); 
            this.StartCoroutine(this.Restart());
        }

        /* Included this because it has to be overrided due to the GameplayManager class. 
           Currently not used. */
        public override void reset()
        {
            
        }

        /* Included this because it has to be overrided due to the GameplayManager class. 
           Currently not used. */
        public override void onWinConditionPointsReached()
        {
            
        }
    }
}