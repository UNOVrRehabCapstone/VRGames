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
    /**
     * The BalloonGameplayManager controls how the game is played, that is, it controls things like 
     * when to start and stop a game and watching conditions for when a game should be terminated.
     */
    public class BalloonGameplayManager : GameplayManager
    {
        //public static BalloonGameplayManager Instance {get; private set;}
        [SerializeField] private ParticleSystem confettiSystem;

        public delegate void AchievementUpdateEventHandler(string senderTag);
        public static event AchievementUpdateEventHandler OnAchievementUpdate;

        private CareerModeLevel currentLevel;
        private IEnumerator careerModeRoutine;
       
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
                Debug.Log("balloonStart = " + SocketClasses.BalloonGameSettingsValues.balloonStart);
                yield return new WaitUntil(() => SocketClasses.BalloonGameSettingsValues.balloonStart);
            }

            RefreshBalloonSettings();

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
                    AchievementManager.Instance.HideAchievementList();
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
            if (this.gameSettings.maxLives < 50) {
                this.StartCoroutine(this.WatchPlayerLives());
            }
            this.StartCoroutine(this.WatchScore());
            BalloonManager.Instance.StartAutomaticSpawner(3.0f);
        }

        private void CareerGameMode()
        {
            Debug.Log("CareerMode Reports: " + SocketClasses.BalloonGameSettingsValues.careerModeLevelToPlay);
            AchievementManager.Instance.HideAchievementList();
            this.StartCoroutine(this.WatchPlayerLives());
            this.setCurrentLevel(Int16.Parse(SocketClasses.BalloonGameSettingsValues.careerModeLevelToPlay));
            this.careerModeRoutine = PlayCareerLevel(this.currentLevel);
            //update scoreboard with countdown

            this.StartCoroutine(this.careerModeRoutine);
        }

        private IEnumerator PlayCareerLevel(CareerModeLevel level)
        {
            GameObject balloon;
            // spawnSchedule is an array of strings. If value is numerical, they get interpreted as a delay
            // if value is alphabetical, they're intepreted as the tag for the balloon to be spawned
            yield return ScoreboardCountdown(3, false);
            PointsManager.updateScoreboardMessage("Playing Level " + (Int16.Parse(SocketClasses.BalloonGameSettingsValues.careerModeLevelToPlay) +1));
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
                        PointsManager.updateScoreboardMessage("You beat level " + (Int16.Parse(SocketClasses.BalloonGameSettingsValues.careerModeLevelToPlay) + 1)
                            + " with a score of " + level.score + "!");
                        // Do achievement udpates
                        if(this.gameSettings.environment == GameSettingsSO.Environment.MEADOW)
                        {
                            SocketClasses.Achievements.LevelsPlayed[1] = true;
                        }
                        else
                        {
                            SocketClasses.Achievements.LevelsPlayed[0] = false;
                        }

                        switch (SocketClasses.BalloonGameSettingsValues.careerModeLevelToPlay)
                        {
                            case "0":
                                {
                                    SocketClasses.BalloonGameDataValues.levelOneScore = level.score.ToString();
                                    Debug.Log(level.score.ToString());
                                    break;
                                }
                            case "1":
                                {
                                    SocketClasses.BalloonGameDataValues.levelTwoScore = level.score.ToString();
                                    break;
                                }
                            case "2":
                                {
                                    SocketClasses.BalloonGameDataValues.levelThreeScore = level.score.ToString();
                                    break;
                                }
                            case "3":
                                {
                                    SocketClasses.BalloonGameDataValues.levelFourScore = level.score.ToString();
                                    break;
                                }
                            case "4":
                                {
                                    SocketClasses.BalloonGameDataValues.levelFiveScore = level.score.ToString();
                                    break;
                                }
                            default:
                                break;
                        }
                        this.CheckFullControl();
                        this.CheckOverachiever();
                        this.CheckWorldTraveler();
                        this.CheckFinishCareerMode();
                        Network.NetworkManager.Instance.UpdateBalloonProgression();
                        AchievementManager.Instance.ShowAchievementList();
                        yield return null;
                    }
                }
            }   
        }

        /**
         * The setCurrentLevel method sets the current level for carreer mode to level.
         *
         * @param level The level to set the career mode to.
         */
        public void setCurrentLevel(int level)
        {
            Debug.Log("setCurrentLevel reports: " + SocketClasses.BalloonGameSettingsValues.careerModeLevelToPlay);
            if (level >= 0 && level < 5)
            {
                this.currentLevel = SocketClasses.CareerModeLevels.levels[level];
            }
            else
            {
                Debug.Log("Level not found");
            }
        }

        private GameObject FindBalloonPrefabWithTag(string tag)
        {
            // Use LINQ to find the first GameObject in the list with the specified tag
            return this.gameSettings.balloonPrefabs.FirstOrDefault(prefab => prefab.CompareTag(tag));
        }

        /* RefreshBalloonSettings() is a method to apply any new settings the clinician has changed. Should be called on game start as well*/
        private void RefreshBalloonSettings()
        {
            Debug.Log("Refreshing, Balloonstart = " + SocketClasses.BalloonGameSettingsValues.balloonStart);
            this.gameSettings.gameMode = (GameSettingsSO.GameMode)Int16.Parse(SocketClasses.BalloonGameSettingsValues.balloonGameMode);

            this.gameSettings.spawnPattern = (GameSettingsSO.SpawnPattern)Int16.Parse(SocketClasses.BalloonGameSettingsValues.balloonGamePattern);
            this.gameSettings.handSetting = (GameSettingsSO.HandSetting)Int16.Parse(SocketClasses.BalloonGameSettingsValues.balloonGameHandSetting);
            this.gameSettings.goal = Int16.Parse(SocketClasses.BalloonGameSettingsValues.balloonGameGoal);
            this.gameSettings.specialBalloonSpawnChance = Int16.Parse(SocketClasses.BalloonGameSettingsValues.balloonGameSpecialBalloonFrequency);
            this.gameSettings.maxLives = Int16.Parse(SocketClasses.BalloonGameSettingsValues.balloonGameMaxLives);
            this.gameSettings.rightSpawnChance = float.Parse(SocketClasses.BalloonGameSettingsValues.balloonGameLeftRightRatio);
            this.gameSettings.spawnTime = float.Parse(SocketClasses.BalloonGameSettingsValues.spawnTime);
            this.gameSettings.maxNumBalloonsSpawnedAtOnce = Int16.Parse(SocketClasses.BalloonGameSettingsValues.numBalloonsSpawnedAtOnce);
            this.gameSettings.floatStrengthModifier = float.Parse(SocketClasses.BalloonGameSettingsValues.speedModifier);

            /* Verify game settings are being set. */
            Debug.Log(this.gameSettings.spawnPattern);
            Debug.Log(this.gameSettings.handSetting);
            Debug.Log(this.gameSettings.goal);
            Debug.Log(this.gameSettings.specialBalloonSpawnChance);
            Debug.Log(this.gameSettings.maxLives);
            Debug.Log(this.gameSettings.rightSpawnChance);
            Debug.Log(this.gameSettings.spawnTime);
            Debug.Log(this.gameSettings.maxNumBalloonsSpawnedAtOnce);
            Debug.Log(this.gameSettings.floatStrengthModifier);
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
            //DartManager.Instance.DestroyDarts();


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

        /**
         * The AchievementUpdateEvent method takes a message which is used to update Achievements.
         *
         * @param message The message used to update the achievements.
         */
        public void AchievementUpdateEvent(string message)
        {
            OnAchievementUpdate?.Invoke(message);
        }
    }
}