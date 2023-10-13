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
        //TODO: Note that this is currently being set in the editor for testing purposes, but 
        //      this needs to be changed so that the settings are determined based on whatever
        //      game mode the user selects.
        //The chosenGameSettings based on user selection.
        [SerializeField] private GameSettingsSO chosenGameSettings;
        
// ===============================================================================================
// THE REST OF THESE FIELDS ARE NOT USED FOR THE CURRENT GAME LOGIC. Will be deleted later.
//
// TODO: DELETE THIS STUFF

        public GameObject leftBalloonPrefab;
        public GameObject rightBalloonPrefab;
        public GameObject dartPrefab;
        public GameObject leftBalloonSpawn;
        public GameObject rightBalloonSpawn;
        public float secondsTilDespawn = 200;
        public List<GameObject> balloons = new List<GameObject>();    //list of current interactables (planes/boats/balloons)

        //adding in the ability to change the frequency when the balloons spawn
        public float nextSpawnTime = 0f;
        public float maxSpawnTime = 3.0f;
        public float minSpawnTime = 0f;

        public int balloonsSpawnedAtOnce = 0;
        private bool alternatingBalloonsController = false;
        private int pointTotal;
        private bool restarting = false;
        public int goalOne = 5;
        public int goalTwo = 10;


        new void Start()
        {
            base.Start();
            PointsManager.updateScoreboardMessage("Press The Buttons Behind You To Spawn A Dart!");
            PointsManager.addPointTrigger("==", winConditionPoints, "onWinConditionPointsReached");
            this.SetBalloonTimer();
        }

        void FixedUpdate()
        {
            Debug.Log("Game mode set to " + this.chosenGameSettings.gameModeStr);
            BalloonSpawnManager.Instance.SpawnBalloons(chosenGameSettings);

            // Temporarily disabled
            /* 
            this.nextSpawnTime -= Time.deltaTime;
            pointTotal = PointsManager.getLeftPoints() + PointsManager.getRightPoints();
            if (pointTotal == this.winConditionPoints && !restarting)
            {
                restarting = true;
                onWinConditionPointsReached();
                StartCoroutine(Restart());
            }
            if (pointTotal == goalOne || pointTotal == goalTwo)
            {
                this.IncreaseDifficulty();
            }
            if (balloons.Count <= balloonsSpawnedAtOnce && this.nextSpawnTime <= 0)
            {
                spawnBalloons();
                if(difficulty == 3)
                {
                    SetBalloonTimer();
                }
                else
                {
                    this.nextSpawnTime = 5.0f;
                }
            }
            */
        }

// ======================================================================================================== //
// EVERYTHING BELOW THIS LINE IS NOT USED IN THE CURRENT GAME LOGIC. I haven't deleted this yet because I do 
// not know if any of these methods are used elsewhere in the code.
//
// TODO: DELETE THIS STUFF

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

        void SetBalloonTimer()
        {
            this.nextSpawnTime = Random.Range(this.minSpawnTime, this.maxSpawnTime);
        }

        public override void onWinConditionPointsReached()
        {
            print("You beat the game!");
            PointsManager.updateScoreboardMessage("You Win!");
            
        }

        public void OnDartGrabbed(GameObject dart)
        {
            
        }

        public void OnDartReleased(GameObject dart)
        {
     
        }
        

        //Removes a balloon from the list and destroys it.
        public void killBalloon(GameObject obj)
        {
            balloons.Remove(obj);
            Destroy(obj);
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
            foreach (GameObject balloon in balloons)
            {
                killBalloon(balloon);
            }
        }

        //Creates a new balloon GameObject and adds it to the list
        public void spawnBalloons()
        {
            
            //This is the easiest difficulty. Balloons will come up at the same time as each other
            if (this.difficulty == 1)
            {
                SpawnLeftBalloon();
                SpawnRightBalloon();
            }

            //This is the second difficulty level. Here the balloons alternate but always stay on a strict alternation pattern. 
            else if(this.difficulty == 2)
            {
               if (alternatingBalloonsController)
               {
                   SpawnLeftBalloon();
                   alternatingBalloonsController = false;
               }
               else
               {
                   SpawnRightBalloon();
                   alternatingBalloonsController = true;
               }
            }

            else if(this.difficulty == 3)
            {
                int balloonSpawnIndicator = Random.Range(0,3);
                Debug.Log ("balloon choice: " + balloonSpawnIndicator + " Number of Ballons in scene: " + balloons.Count);

                switch (balloonSpawnIndicator)
                {
                    case 0 : SpawnLeftBalloon();
                    break;

                    case 1 : SpawnRightBalloon();
                    break;

                    case 2 :
                    {
                        SpawnRightBalloon();
                        SpawnLeftBalloon();
                    }
                        break;
                }
            }
        }

        private void SpawnLeftBalloon()
        {
            GameObject tempLeft = Instantiate(leftBalloonPrefab);
            tempLeft.transform.position = leftBalloonSpawn.transform.position; 
            balloons.Add(tempLeft);
            StartCoroutine(despawnCountdown(tempLeft));
        }

        private void SpawnRightBalloon()
        {
            GameObject tempRight = Instantiate(rightBalloonPrefab);
            tempRight.transform.position = rightBalloonSpawn.transform.position;
            balloons.Add(tempRight);
            StartCoroutine(despawnCountdown(tempRight));
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

        //Coroutine for counting down and despawning balloon after certain amount of time
        private IEnumerator despawnCountdown(GameObject balloon)
        {
            /*var endTime = Time.realtimeSinceStartup + secondsTilDespawn;
            while (Time.realtimeSinceStartup < endTime)
            {
                yield return new WaitForSeconds(.5f);
            }*/
                
            yield return new WaitForSeconds(secondsTilDespawn);

            killBalloon(balloon);
        }
    }
}