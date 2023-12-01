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
        [SerializeField] private GameObject     meadow;
        [SerializeField] private GameObject     balloonEnclosure;


        public delegate void AchievementUpdateEventHandler(string senderTag);
        public static event AchievementUpdateEventHandler OnAchievementUpdate;
        public string message = "default message";


        /*TODO: Note that this is currently being set in the editor for testing purposes, but 
                this needs to be changed so that the settings are determined based on whatever
                game mode the user selects. */
        public GameSettingsSO                   gameSettings;
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
            this.playerLives = this.gameSettings.maxLives;
            PointsManager.updateScoreboard();

            switch (this.gameSettings.gameMode) {
                /* CAREER: Wait for clinician to start a level.*/
                case GameSettingsSO.GameMode.CAREER:
                    this.CareerGameMode();
                    break;

                case GameSettingsSO.GameMode.CUSTOM:
                    this.CustomGameMode();
                    break;
                
                case GameSettingsSO.GameMode.MANUAL:
                    /* Do nothing; gameplay is dictated by the clinician. */
                    break;

                default:
                    Debug.LogError("This should never happen.");
                    break;

            }
            
            PointsManager.updateScoreboardMessage("Game mode set to " + this.gameSettings.gameMode.ToString());
        }

        private void CustomGameMode()
        {
            this.StartCoroutine(this.WatchPlayerLives());
            this.StartCoroutine(this.WatchScore());
            BalloonManager.Instance.StartAutomaticSpawner(3.0f);
        }

        private void CareerGameMode()
        {
            /* TODO */
        }

        /* RefreshBalloonSettings() is a method to apply any new settings the clinician has changed. Should be called on game start as well*/
        void RefreshBalloonSettings()
        {
            this.gameSettings.gameMode = (GameSettingsSO.GameMode)Int16.Parse(SocketClasses.BalloonGameSettingsValues.balloonGameMode);
            this.gameSettings.goal = Int16.Parse(SocketClasses.BalloonGameSettingsValues.balloonGameGoal);
            this.gameSettings.specialBalloonSpawnChance = Int16.Parse(SocketClasses.BalloonGameSettingsValues.balloonGameSpecialBalloonFrequency);
            this.gameSettings.handSetting = (GameSettingsSO.HandSetting)Int16.Parse(SocketClasses.BalloonGameSettingsValues.balloonGameHandSetting);
            this.gameSettings.maxLives = Int16.Parse(SocketClasses.BalloonGameSettingsValues.balloonGameMaxLives);
            this.gameSettings.rightSpawnChance = float.Parse(SocketClasses.BalloonGameSettingsValues.balloonGameLeftRightRatio);
            this.gameSettings.spawnPattern = (GameSettingsSO.SpawnPattern)Int16.Parse(SocketClasses.BalloonGameSettingsValues.balloonGamePattern);
            Debug.Log(this.gameSettings.rightSpawnChance);
        }

        public GameSettingsSO GetGameSettings()
        {
            return this.gameSettings;
        }
        
        /**
         * Restart() reloads the scene. Useful for automatically reloading the scene when some 
         * condition has been met.
         */
        public IEnumerator Restart()
        {
            BalloonManager.Instance.StopAutomaticSpawner();
            /* Required because if a balloon despawns while the game is restarting, it will still 
               cause a loss of life. */
            BalloonManager.Instance.KillAllBalloons();
            /* Make sure to destroy the darts on restart. This is necessary becauase if the player 
               is holding a dart between scene resets, the held darts will not be destroyed 
               resulting in duplicated darts. TODO: There has to be a better way of doing this. */
            DartManager.Instance.DestroyDarts();


            //Check if the clinician is controlling the game. If they are, wait until they manually start the game again
            //  Otherwise, wait for five seconds then automatically restart. clinicianIsControlling is defaulted to false
            //  and is set to true if the game receives a clinician view settings update in the network manager
            if (SocketClasses.BalloonGameSettingsValues.clinicianIsControlling)
            {
                yield return new WaitUntil(() => SocketClasses.BalloonGameSettingsValues.balloonStart);
            }
            else
            {
                yield return new WaitForSeconds(5);
            }
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
            yield return new WaitUntil(() => PointsManager.isGoalReached(this.gameSettings.handSetting, this.gameSettings.goal));
            
            
            // Do achievement udpates
            //    Overachiever 
            if(this.playerLives > gameSettings.maxLives)
            {
                this.message = "Ended with more lives";
                    this.AchievementUpdateEvent();
            }

            //    Full control
           if(this.gameSettings.gameMode == GameSettingsSO.GameMode.CUSTOM)
            {
                this.message = "Custom game ended";
                this.AchievementUpdateEvent();
            }
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

        public void AchievementUpdateEvent()
        {
            OnAchievementUpdate?.Invoke(this.message);
        }
    }
}