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
        

        /* ====================================== START ========================================= */

        /* TODO: The fields between the lines are NOT used in the new refactored code however they 
         *       are needed for the methods below which I have also marked off. The reason I have 
         *       not deleted the methods is because they are used in other parts of the code that I 
         *       have not touched yet, so attempting to delete the methods breaks the game. 
         *
         *       Wanted to put this note here so no one is confused on why there are a bunch of 
         *       methods/fields not being used.
         */

        public GameObject leftBalloonPrefab;
        public GameObject rightBalloonPrefab;
        public GameObject dartPrefab;
        public GameObject leftBalloonSpawn;
        public GameObject rightBalloonSpawn;
        public float secondsTilDespawn = 200;
        public List<GameObject> balloons = new List<GameObject>();   

        public float nextSpawnTime = 0f;
        public float maxSpawnTime = 3.0f;
        public float minSpawnTime = 0f;

        public int balloonsSpawnedAtOnce = 0;
        private bool alternatingBalloonsController = false;
        private int pointTotal;
        private bool restarting = false;
        public int goalOne = 5;
        public int goalTwo = 10;
        /* ======================================= END ========================================== */

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
            BalloonSpawnManager.Instance.SpawnBalloons();
        }

        public GameSettingsSO getGameSettings()
        {
            return this.gameSettings;
        }
        
        /* ===================================================================================== */
        /* TODO: Everything below this line needed to be deleted/refactored into other modules   */

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

        //Destroys the dart and sets the spawner back to false.
        public void killDart(GameObject dart)
        {
            DartRespawn.disableDart();
            Destroy(dart);
        }
        
        //Kills all balloons in scene
        public override void reset()
        {
            BalloonSpawnManager.Instance.KillAllBalloons();
        }

        public void SpawnDart(GameObject dartSpawn)
        {
            GameObject temp = Instantiate(dartPrefab);
            MeshRenderer wing = temp.gameObject.GetComponentInChildren<MeshRenderer>();
            foreach (Transform child in temp.transform)
            { 
                if (child.CompareTag("DartColorMatch"))
                {
                    wing = child.GetComponent<MeshRenderer>();
                    if (dartSpawn.gameObject.CompareTag("YellowDartSpawn"))
                    {
                        wing.material.color = Color.yellow;
                    }
                    else if (dartSpawn.gameObject.CompareTag("BlueDartSpawn"))
                    {
                        wing.material.color = Color.blue;
                    }
                }
            }
            temp.transform.position = dartSpawn.transform.position + new Vector3(0, 0, -.06f);
        }
    }
}