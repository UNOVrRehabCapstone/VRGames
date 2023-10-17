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

        private void Awake()
	    {
            //Singleton pattern make sure there is only one balloon spawn manager.
		    if (Instance != null) {
			    Debug.LogError("There should only be one balloon spawn manager.");
		    }
		    Instance = this;
	    }

        new void Start()
        {
            base.Start();
            PointsManager.updateScoreboardMessage("Press The Buttons Behind You To Spawn A Dart!");
            PointsManager.addPointTrigger("==", winConditionPoints, "onWinConditionPointsReached");
        }

        void FixedUpdate()
        {
            Debug.Log("Game mode set to " + this.gameSettings.gameModeStr);
            BalloonManager.Instance.SpawnBalloons();
        }

        public GameSettingsSO GetGameSettings()
        {
            return this.gameSettings;
        }

        private IEnumerator Restart()
        {
            yield return new WaitForSeconds(5);
            PointsManager.updateScoreboardMessage("Restarting in: 3");
            yield return new WaitForSeconds(1);
            PointsManager.updateScoreboardMessage("Restarting in: 2");
            yield return new WaitForSeconds(1);
            PointsManager.updateScoreboardMessage("Restarting in: 1");
            yield return new WaitForSeconds(1);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public override void onWinConditionPointsReached()
        {
            print("You beat the game!");
            PointsManager.updateScoreboardMessage("You Win!");
            
        }
        
        //Kills all balloons in scene
        public override void reset()
        {
            BalloonManager.Instance.KillAllBalloons();
        }
    }
}