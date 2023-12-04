using System;
using System.Collections;
using System.Collections.Generic;
using Oculus.Platform.Samples.VrHoops;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using System.Linq;

namespace Classes.Managers
{
    public class BalloonGameplayManager : GameplayManager
    {
        public static BalloonGameplayManager Instance {get; private set;}
        [SerializeField] private ParticleSystem confettiSystem;

        public delegate void AchievementUpdateEventHandler(string senderTag);
        public static event AchievementUpdateEventHandler OnAchievementUpdate;

        private CareerModeLevel currentLevel;
        private IEnumerator careerModeRoutine;


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

            //RefreshBalloonSettings();
	    }

        new void Start()
        {
            base.Start();
            PointsManager.updateScoreboardMessage("Welcome to the Balloon Game!");
            this.StartCoroutine(this.WaitForClinThenStart());
           
        }

        private IEnumerator WaitForClinThenStart()
        {
            if (SocketClasses.BalloonGameSettingsValues.clinicianIsControlling)
            {
                Debug.Log("balloonStart = " + SocketClasses.BalloonGameSettingsValues.careerModeLevelToPlay);
                yield return new WaitUntil(() => SocketClasses.BalloonGameSettingsValues.balloonStart);
            }


            

            Debug.Log("Game mode set to " + this.gameSettings.gameMode.ToString());
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
            AchievementManager.Instance.HideAchievementList();
            this.StartCoroutine(this.WatchPlayerLives());
            this.StartCoroutine(this.WatchScore());
            BalloonManager.Instance.StartAutomaticSpawner(3.0f);
        }

        private void CareerGameMode()
        {
            /* TODO */
            AchievementManager.Instance.HideAchievementList();
            this.StartCoroutine(this.WatchPlayerLives());
            this.setCurrentLevel(SocketClasses.BalloonGameSettingsValues.careerModeLevelToPlay);
            this.careerModeRoutine = PlayCareerLevel(this.currentLevel);
            //update scoreboard with countdown

            this.StartCoroutine(this.careerModeRoutine);
        }

        IEnumerator PlayCareerLevel(CareerModeLevel level)
        {
            GameObject balloon;
            // spawnSchedule is an array of strings. If value is numerical, they get interpreted as a delay
            // if value is alphabetical, they're intepreted as the tag for the balloon to be spawned
            yield return ScoreboardCountdown(3, false);
            PointsManager.updateScoreboardMessage("Playing Level " + (SocketClasses.BalloonGameSettingsValues.careerModeLevelToPlay +1));
            for (int i = 0; i < level.schedule.Length; i++)
            {
                string value = level.schedule[i];
                if (float.TryParse(value, out float intValue))
                {
                    yield return new WaitForSeconds(intValue);
                }
                else
                {
                    if (!value.Equals("END"))
                    {
                        balloon = FindBalloonPrefabWithTag(value);
                        if (balloon != null)
                        {
                            BalloonManager.Instance.ManualSpawn(this.gameSettings.spawnPattern, balloon);
                            yield return null;
                        }
                    }
                    else
                    {
                        Debug.Log("End of Career Mode Level, or Invalid Balloon");
                        yield return new WaitUntil(BalloonManager.Instance.AllBalloonsAreGone);
                        //Right now it's set so that however many lives you have left is your score. 
                        //This isn't super ideal because it means we're locked into exactly three lives for career mode
                        if (this.playerLives > this.gameSettings.maxLives)
                        {
                            level.score = 3;
                        }
                        else
                        {
                            level.score = this.playerLives;
                        }
                        PointsManager.updateScoreboardMessage("You beat level " + (SocketClasses.BalloonGameSettingsValues.careerModeLevelToPlay + 1)
                            + " with a score of " + level.score + "!");
                        // Do achievement udpates
                        this.CheckFullControl();
                        this.CheckOverachiever();
                        this.CheckWorldTraveler();
                        this.CheckFinishCareerMode();
                        AchievementManager.Instance.ShowAchievementList();
                        yield return null;
                    }
                }

            }

            

            
        }

        public void setCurrentLevel(int level)
        {
            if (level >= 0 && level < 5)
            {
                this.currentLevel = SocketClasses.CareerModeLevels.levels[level];
            }
            else
            {
                Debug.Log("Level not found");
            }
        }


        GameObject FindBalloonPrefabWithTag(string tag)
        {
            // Use LINQ to find the first GameObject in the list with the specified tag
            return this.gameSettings.balloonPrefabs.FirstOrDefault(prefab => prefab.CompareTag(tag));
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

            AchievementManager.Instance.ShowAchievementList();
            if(this.gameSettings.gameMode == GameSettingsSO.GameMode.CUSTOM)
            {
                BalloonManager.Instance.StopAutomaticSpawner();
            }
            if(this.gameSettings.gameMode == GameSettingsSO.GameMode.CAREER)
            {
                Debug.Log("Stopping PlayCareerLevel");
                StopCoroutine(this.careerModeRoutine);
            }

            if (SocketClasses.BalloonGameSettingsValues.clinicianIsControlling)
            {
                SocketClasses.BalloonGameSettingsValues.balloonStart = false;
            }

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
                Debug.Log("Restarting");
                yield return new WaitForSeconds(5);
            }
            yield return ScoreboardCountdown(5, true);
            PointsManager.resetPoints();
            SocketClasses.BalloonGameSettingsValues.balloonStart = false;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }


        //ScoreboardCountdown is now supplied with arguments
        private IEnumerator ScoreboardCountdown(int countDownTime, bool restarting)
        {
            string prefix = "Starting in: ";
            if (restarting)
            {
                prefix = "Restarting in: ";

            }
            for (int i = countDownTime; i > 0; i--)
            {
                PointsManager.updateScoreboardMessage(prefix + i);
                yield return new WaitForSeconds(1);
            }

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
            this.CheckFullControl();
            this.CheckOverachiever();
            this.CheckWorldTraveler();
            this.CheckFinishCareerMode();
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

        private void CheckOverachiever()
        {
            //    Overachiever 
            if (this.playerLives > gameSettings.maxLives)
            {
                this.AchievementUpdateEvent("Ended with more lives");
            }
        }
        private void CheckWorldTraveler()
        {
            bool hasPlayedAll = true;
            foreach(bool levelPlayed in SocketClasses.Achievements.LevelsPlayed)
            {
                if (!levelPlayed)
                {
                    hasPlayedAll = false;
                    break;
                }
            }

            if (hasPlayedAll)
            {
                this.AchievementUpdateEvent("Played Both Environments");
            }
        }
        private void CheckFullControl()
        {
            //    Full control
            if (this.gameSettings.gameMode == GameSettingsSO.GameMode.CUSTOM)
            {
                this.AchievementUpdateEvent("Custom game ended");
            }
        }
        private void CheckFinishCareerMode()
        {
            bool hasPlayedAllLevels = true;
            foreach(string levelScore in SocketClasses.BalloonGameDataValues.levelScores)
            {
                if(Int16.Parse(levelScore) <= 0)
                {
                    hasPlayedAllLevels = false;
                    break;
                }
            }
            if (hasPlayedAllLevels)
            {
                AchievementUpdateEvent("Played All Career Levels");
            }
        }

        public void AchievementUpdateEvent(string message)
        {
            OnAchievementUpdate?.Invoke(message);
        }
    }
}