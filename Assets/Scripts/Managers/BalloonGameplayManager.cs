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
        private enum GameMode 
        {
            Relaxed,    
            Normal,
            Endless
        }

        [SerializeField] private GameMode         gameMode;
        [SerializeField] private List<GameObject> positiveSpecialBalloonPrefabs;
        [SerializeField] private List<GameObject> negativeSpecialBalloonPrefabs;
        [SerializeField] private GameObject       genericBalloonPrefab;

        public GameObject leftBalloonPrefab;
        public GameObject rightBalloonPrefab;
        public GameObject increaseLifeBalloonPrefab;
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
            //spawnBalloons();
            this.SetBalloonTimer();
        }

        void FixedUpdate()
        {
            switch(this.gameMode) {
                case GameMode.Relaxed:
                    Debug.Log("Game mode is set to relaxed.");
                    RelaxedGameMode();
                    break;
                
                case GameMode.Normal:
                    Debug.Log("Game mode is set to normal.");
                    NormalGameMode();
                    break;


                case GameMode.Endless:
                    Debug.Log("Game mode is set to endless.");
                    EndlessGameMode();
                    break;

                default:
                    Debug.Log("Should never happen.");
                    break;
            }


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

        private void RelaxedGameMode()
        {
            //Do Stuff
            Debug.Log("Entered RelaxedGameMode()");
        }

        private void NormalGameMode()
        {
            //Do stuff
            Debug.Log("Entered NormalGameMode()");
            
            if (balloons.Count == 0) {
                 // Select random prefabs to spawn
                GameObject leftBalloonPrefab  = positiveSpecialBalloonPrefabs[Random.Range(0, positiveSpecialBalloonPrefabs.Count)];
                GameObject rightBalloonPrefab = positiveSpecialBalloonPrefabs[Random.Range(0, positiveSpecialBalloonPrefabs.Count)];

                SpawnBalloon(leftBalloonPrefab,  leftBalloonSpawn);
                SpawnBalloon(rightBalloonPrefab, rightBalloonSpawn);
            }
           
        }

        
        private void EndlessGameMode()
        {
            //Do Stuff 
            Debug.Log("Entered EndlessGameMode()");
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

        private void SpawnBalloon(GameObject balloon, GameObject spawnPoint)
        {
            GameObject tmp = Instantiate(balloon);
            tmp.transform.position = spawnPoint.transform.position;
            balloons.Add(tmp);
            StartCoroutine(despawnCountdown(tmp));
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
 

        private void spawnRestoreLifeBalloon()
        {
            GameObject tempLeft = Instantiate(increaseLifeBalloonPrefab);
            tempLeft.transform.position = leftBalloonSpawn.transform.position; 
            balloons.Add(tempLeft);
            StartCoroutine(despawnCountdown(tempLeft));
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