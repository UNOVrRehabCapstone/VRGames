/**
 * The BalloonSpawnManager class handles the logic for spawning balloons.
 *
 * Authors: Dante Lawrence
 **/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* TODO: Extract some of this code. This is some code I believe Jared wrote? There's some good code 
         here, so I do not want to delete until I have re-implemented into the new code. Too lazy to 
         do it right now though :) */

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

namespace Classes.Managers 
{
	public class BalloonManager : MonoBehaviour
    {	
        /* Singleton pattern. Holds a reference to the balloon spawn manager object. */
	    public static BalloonManager Instance {get; private set;}

        private GameSettingsSO gameSettings;

        /* Holds the current balloons in the scene (the balloons that have been spawned but not despawned) */
	    private List<GameObject> balloons = new List<GameObject>();

        private bool alternate = false;

	    private void Awake()
	    {
            //Singleton pattern make sure there is only one balloon spawn manager.
		    if (Instance != null) {
			    Debug.LogError("There should only be one balloon spawn manager.");
		    }
		    Instance = this; 
	    }

        private void Start()
        {
            Instance.gameSettings = BalloonGameplayManager.Instance.GetGameSettings();
        }

        /**
         * Spawns the balloons based on the spawn settings. See the game settings for more information.
         *
         * Author: Dante Lawrence
         */
	    public void SpawnBalloons()
	    {
            this.gameSettings.nextSpawnTime -= Time.deltaTime;

            /* Both conditions must be met to spawn another balloon */
            if (   (this.balloons.Count < this.gameSettings.maxNumBalloonsSpawnedAtOnce)
                && (this.gameSettings.nextSpawnTime <= 0) ) { 

                switch (this.gameSettings.spawnPattern) {
                    case GameSettingsSO.SpawnPattern.CONCURRENT: 
                        Debug.Log("Concurrent spawn pattern chosen");

                        /* Have to be really careful here since two balloons are spawned with this 
                           spawn pattern */
                        if ( !(this.balloons.Count + 2 <= this.gameSettings.maxNumBalloonsSpawnedAtOnce) )
                            break;

                        GameObject leftBalloon  = GetBalloonBasedOnProb();
                        GameObject rightBalloon = GetBalloonBasedOnProb();
                        SpawnBalloon(leftBalloon,  this.gameSettings.leftSpawn);
                        SpawnBalloon(rightBalloon, this.gameSettings.rightSpawn);

                        break;
                    case GameSettingsSO.SpawnPattern.ALTERNATING: 
                        Debug.Log("Alternate spawn pattern chosen.");
                        
                        GameObject balloon = GetBalloonBasedOnProb();
                        Vector3 spawnPoint = alternate ? this.gameSettings.leftSpawn :
                                                         this.gameSettings.rightSpawn;
                        this.alternate = !alternate;

                        this.SpawnBalloon(balloon, spawnPoint);
                        break;

                    default:
                        Debug.LogError("This should never happen.");
                        break;
                }

                this.gameSettings.nextSpawnTime = Random.Range(this.gameSettings.minSpawnTime, this.gameSettings.maxSpawnTime);
            }
	    }

	    /**
         * Returns a balloon prefab based on the probability of it spawning. Probability of a balloon spawning
         * is set in the game settings.
         *
         * Author: Dante Lawrence
         * Note: Code was adapted from the probability code provided by Unity which can be found here 
         *       https://docs.unity3d.com/2019.3/Documentation/Manual/RandomNumbers.html
         */
        private GameObject GetBalloonBasedOnProb()
        {
            float       total = 0;
            List<float> probs = gameSettings.probabilities;

            foreach (float elem in probs) {
                total += elem;
            }

            float randomPoint = Random.value * total;

            for (int i= 0; i < probs.Count; i++) {
                if (randomPoint < probs[i]) {
                    return gameSettings.balloonPrefabs[i];
                }
                else {
                    randomPoint -= probs[i];
                }
            }
            return gameSettings.balloonPrefabs[probs.Count - 1];
        }

        /**
         * Given a balloon and a spawn point this method spawns a balloon at the spawn point.
         *
         * Author: Dante Lawrence
         */
        private void SpawnBalloon(GameObject balloon, Vector3 spawnPoint)
        {
            GameObject tmp = Instantiate(balloon);
            tmp.transform.position = spawnPoint;
            balloons.Add(tmp);
            StartCoroutine(DespawnCountdown(tmp));
        }

        /** 
         * Remove a balloon from the scene and removes it from the current list of balloons.
         */
        public void KillBalloon(GameObject balloon)
        {
            balloons.Remove(balloon);
            Destroy(balloon);
        }

        /**
         * Removes all balloons from the scene.
         */
        public void KillAllBalloons()
        {
            foreach (GameObject balloon in balloons)
            {
                this.KillBalloon(balloon);
            }
        }

        /**
         * Coroutine for counting down and despawning balloon after certain amount of time.
         */
        private IEnumerator DespawnCountdown(GameObject balloon)
        {
            /*var endTime = Time.realtimeSinceStartup + secondsTilDespawn;
            while (Time.realtimeSinceStartup < endTime)
            {
                yield return new WaitForSeconds(.5f);
            }*/
                
            yield return new WaitForSeconds(this.gameSettings.secondsTilDespawn);

            KillBalloon(balloon);
        }
    }
}

