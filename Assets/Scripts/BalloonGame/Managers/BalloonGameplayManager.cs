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
        [SerializeField] private ParticleSystem confettiSystem;

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

            Debug.Log("Balloon gameplay manager active.");
	    }

        new void Start()
        {
            base.Start();
            RefreshBalloonSettings();

            Debug.Log("Game mode set to " +     this.gameSettings.gameMode.ToString());
            Debug.Log("Spawn pattern set to " + this.gameSettings.spawnPattern.ToString());
            this.playerLives = this.gameSettings.numLives;
            PointsManager.updateScoreboard();

            switch (this.gameSettings.gameMode) {
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
            PointsManager.updateScoreboardMessage("Press The Buttons Behind You To Spawn A Dart!\n" +
                                                  "Game mode set to " + this.gameSettings.gameMode.ToString());
        }

        void FixedUpdate()
        {
            if (!isRestarting) {
                /* If you wanted to log the game mode and spawn pattern on every frame */
                //Debug.Log("Game mode set to " +     this.gameSettings.gameMode.ToString());
                //Debug.Log("Spawn pattern set to " + this.gameSettings.spawnPattern.ToString());
                //BalloonManager.Instance.SpawnBalloons();
            }
        }

        public GameSettingsSO GetGameSettings()
        {
            return this.gameSettings;
        }

        /* RefreshBalloonSettings() is a method to apply any new settings the clinician has changed. Should be called on game start as well*/
        void RefreshBalloonSettings()
        {
            this.gameSettings.gameMode = (GameSettingsSO.GameMode)Int16.Parse(SocketClasses.BalloonGameSettingsValues.balloonGameMode);
            this.gameSettings.goal = Int16.Parse(SocketClasses.BalloonGameSettingsValues.balloonGameGoal);
            this.gameSettings.specialBalloonSpawnChance = Int16.Parse(SocketClasses.BalloonGameSettingsValues.balloonGameSpecialBalloonFrequency);
        }
        
        /**
         * Restart() reloads the scene. Useful for automatically reloading the scene when some 
         * condition has been met.
         */
        public IEnumerator Restart()
        {
            this.isRestarting = true;
            /* Required because if a balloon despawns while the game is restarting, it will still 
               cause a loss of life. */
            BalloonManager.Instance.KillAllBalloons();
            BalloonManager.Instance.StopAutomaticSpawner();
            /* Make sure to destroy the darts on restart. This is necessary becauase if the player 
               is holding a dart between scene resets, the held darts will not be destroyed 
               resulting in duplicated darts. TODO: There has to be a better way of doing this. */
            DartManager.Instance.DestroyDarts();

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

        /**
         * WatchPlayerLives() is a coroutine that executes when all lives are lost. Currently only 
         * activated for the Normal and Endless game modes. See the Start() method.
         */
        private IEnumerator WatchPlayerLives()
        {
            yield return new WaitUntil(() => (this.playerLives < 1));

            Debug.Log("Out of lives");
            PointsManager.updateScoreboardMessage("Out of lives"); 
            this.StartCoroutine(this.Restart());
        }

        /**
         * WatchScore() is a coroutine that executes when the score reache the goal defined by the 
         * game settings. Currently only activated for the Relaxed and Normal game modes. See the 
         * Start() method.
         */
        private IEnumerator WatchScore()
        {
            yield return new WaitUntil(() => (PointsManager.getPoints() >= this.gameSettings.goal));

            Debug.Log("Goal has been reached!");
            PointsManager.updateScoreboardMessage("You Win!"); 
            confettiSystem.Play();
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