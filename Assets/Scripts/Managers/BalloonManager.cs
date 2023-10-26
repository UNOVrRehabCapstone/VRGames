/**
 * The BalloonSpawnManager class handles the logic for spawning balloons.
 *
 * Authors: Dante Lawrence
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Classes.Managers 
{
	public class BalloonManager : MonoBehaviour
    {	
        /* Singleton pattern. Holds a reference to the balloon manager object. */
	    public static BalloonManager Instance {get; private set;}

        private GameSettingsSO   gameSettings;

        private GameObject leftSpawn;
        private GameObject rightSpawn;
        
	    private List<GameObject> balloons = new List<GameObject>(); /* Holds the current balloons in
                                                                       the scene */
        private bool             alternate = false;
        private float            nextSpawnTime;
        private Coroutine        automaticSpawner;

	    private void Awake()
	    {
            /* Singleton pattern make sure there is only one balloon manager. */
		    if (Instance != null) {
			    Debug.LogError("There should only be one balloon manager.");
		    }
		    Instance = this; 

            Debug.Log("Balloon manager active.");
	    }

        private void Start()
        {
            this.gameSettings  = BalloonGameplayManager.Instance.GetGameSettings();

            this.leftSpawn     = GameObject.Find("BalloonSpawn_Left");
            this.rightSpawn    = GameObject.Find("BalloonSpawn_Right");
            /* TODO Get rid of */
            this.nextSpawnTime = this.gameSettings.maxSpawnTime;
            this.StartAutomaticSpawner(this.gameSettings.maxSpawnTime);
        }

        private IEnumerator AutomaticSpawner(float initDelay)
        {
            yield return new WaitForSeconds(initDelay);

            while (true)
            {
                switch (this.gameSettings.spawnPattern) {
                    case GameSettingsSO.SpawnPattern.CONCURRENT: 
                        //Debug.Log("Concurrent spawn pattern chosen");

                        /* Have to be really careful here since two balloons are spawned with this 
                           spawn pattern */
                        yield return new WaitUntil(() => ((balloons.Count + 2) <= this.gameSettings.maxNumBalloonsSpawnedAtOnce));
                        this.ConcurrentSpawn();

                        break;
                    case GameSettingsSO.SpawnPattern.ALTERNATING: 
                        //Debug.Log("Alternate spawn pattern chosen.");
                        
                        yield return new WaitUntil(() => (balloons.Count < this.gameSettings.maxNumBalloonsSpawnedAtOnce));
                        this.AlternateSpawn();

                        break;

                    default:
                        Debug.LogError("This should never happen.");
                        break;
                }

                yield return new WaitForSeconds(Random.Range(this.gameSettings.minSpawnTime, this.gameSettings.maxSpawnTime));
            }
        }

        public void StartAutomaticSpawner(float initDelay)
        {
            this.automaticSpawner = this.StartCoroutine(this.AutomaticSpawner(initDelay));
        }

        public void StopAutomaticSpawner()
        {
            this.StopCoroutine(this.automaticSpawner);
        }

        private void ConcurrentSpawn()
        {
            GameObject leftBalloon  = GetBalloonBasedOnProb();
            GameObject rightBalloon = GetBalloonBasedOnProb();
            SpawnBalloon(leftBalloon,  this.leftSpawn);
            SpawnBalloon(rightBalloon, this.rightSpawn);
        }

        private void AlternateSpawn()
        {
            GameObject balloon = GetBalloonBasedOnProb();
            GameObject spawnPoint = alternate ? this.leftSpawn :
                                                this.rightSpawn;
            this.alternate = !alternate;

            this.SpawnBalloon(balloon, spawnPoint);
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
         */
        public void SpawnBalloon(GameObject balloon, GameObject spawnPoint)
        {
            GameObject tmp = Instantiate(balloon);
            tmp.transform.position = spawnPoint.transform.position;
            balloons.Add(tmp);
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
            for(int i = 0; i < this.balloons.Count; ++i)
            { 
                Destroy(this.balloons[i]);
            }
            this.balloons.Clear();
        }
    }
}

