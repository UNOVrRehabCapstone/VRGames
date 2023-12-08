using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Classes.Managers 
{
    /**
     * The BalloonManager class handles the logic for spawning and despawning balloons.
     */
	public class BalloonManager : MonoBehaviour
    {	
        /* Singleton pattern. Holds a reference to the balloon manager object. */
	    public static BalloonManager Instance {get; private set;}

        private GameSettingsSO   gameSettings;

        [SerializeField] private GameObject leftSpawn;
        [SerializeField] private GameObject rightSpawn;
        
	    private List<GameObject> balloons = new List<GameObject>(); /* Holds the current balloons in
                                                                       the scene */

        private bool             alternate = false;
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
            this.gameSettings  = BalloonGameplayManager.Instance.gameSettings;
        }


        //This is for careerMode, to not end the level until the final balloon is either destroyed or popped
        public bool AllBalloonsAreGone()
        {
            if (this.balloons.Count > 0)
            {
                return false;
            }
            else return true;
        }

        /**
         * This method automatically spawns balloons with an initial delay of initDelay seconds.
         */
        private IEnumerator AutomaticSpawner(float initDelay)
        {
            yield return new WaitForSeconds(initDelay);

            while (true)
            {
                GameObject balloon = GetBalloonBasedOnProb();

                switch (this.gameSettings.spawnPattern) {
                    case GameSettingsSO.SpawnPattern.CONCURRENT:
                        //Debug.Log("Concurrent spawn pattern chosen");

                        /* Have to be really careful here since two balloons are spawned with this 
                           spawn pattern */
                        yield return new WaitUntil(() => ((balloons.Count + 2) <= this.gameSettings.maxNumBalloonsSpawnedAtOnce));
                        GameObject rightBalloon = GetBalloonBasedOnProb();
                        this.ConcurrentSpawn(balloon, rightBalloon);

                        break;
                    case GameSettingsSO.SpawnPattern.ALTERNATING: 
                        //Debug.Log("Alternate spawn pattern chosen.");
                        
                        yield return new WaitUntil(() => (balloons.Count < this.gameSettings.maxNumBalloonsSpawnedAtOnce));
                        this.AlternateSpawn(balloon);

                        break;
                    case GameSettingsSO.SpawnPattern.RANDOM:
                        //Debug.Log("Random spawn pattern chosen.");

                        yield return new WaitUntil(() => (balloons.Count < this.gameSettings.maxNumBalloonsSpawnedAtOnce));
                        this.RandomSpawn(balloon);

                        break;

                    default:
                        Debug.LogError("This should never happen.");
                        break;
                }

                yield return new WaitForSeconds(this.gameSettings.spawnTime);
            }
        }

        /**
         * The StartAutomaticSpawner method starts the automatic spawner with an initial delay of 
         * initDelay seconds. 
         *
         * @param initDelay The delay of when to start the automatic spawner.
         */
        public void StartAutomaticSpawner(float initDelay)
        {
            this.automaticSpawner = this.StartCoroutine(this.AutomaticSpawner(initDelay));
        }

        /**
         * The StopAutomaticSpawner method stops the automatic spawner. This method is useful if 
         * there is a need to finely control the spawning of the balloons.
         */
        public void StopAutomaticSpawner()
        {
            this.StopCoroutine(this.automaticSpawner);
        }

        /**
         * The ManualSpawn method spawns one balloon using the passed in spawn pattern.
         *
         * @param balloon The balloon to be spawned. 
         */
        public void ManualSpawn(GameSettingsSO.SpawnPattern pattern, GameObject balloon)
        {
            switch (pattern)
            {
                case GameSettingsSO.SpawnPattern.CONCURRENT:
                    //Debug.Log("Concurrent spawn pattern chosen");

                    /* Have to be really careful here since two balloons are spawned with this 
                       spawn pattern */
                    this.ConcurrentSpawn(balloon, balloon);

                    break;
                case GameSettingsSO.SpawnPattern.ALTERNATING:
                    //Debug.Log("Alternate spawn pattern chosen.");
                    this.AlternateSpawn(balloon);

                    break;
                case GameSettingsSO.SpawnPattern.RANDOM:
                    //Debug.Log("Random spawn pattern chosen.");
                    this.RandomSpawn(balloon);
                    break;

                default:
                    Debug.LogError("This should never happen.");
                    break;
            }

        }

        /**
         * The ConcurrentSpawn is a helper method for spawning the balloons for the concurrent spawn 
         * pattern.
         */
        private void ConcurrentSpawn(GameObject leftBalloon, GameObject rightBalloon)
        {
            SpawnBalloon(leftBalloon,  this.leftSpawn);
            SpawnBalloon(rightBalloon, this.rightSpawn);
        }

        /**
         * The AlternateSpawn is a helper method for spawning the balloons for the alternating 
         * spawn pattern.
         * 11/2/2023: Updated, now does an rng check based on the clinician controlled special balloon spawn chance to determine if it should spawn
         *            a normal balloon or a random powerup.
         */ 
        private void AlternateSpawn(GameObject balloon)
        {
            GameObject spawnPoint = alternate ? this.leftSpawn :
                                                this.rightSpawn;
            this.alternate = !alternate;

            this.SpawnBalloon(balloon, spawnPoint);
        }

        /**
         * The RandomSpawn is a helper method for spawning the balloons in a (weighted) random pattern.
         * rightSpawnChance represents the chance for a balloon to be spawned on the right.
         * The chance for a balloon to be spawned on the left is 1 - rightSpawnChance.
         * For example, a rightSpawnChance of 0.25 gives a 1 - 0.25 = 0.75 spawn chance for the left.
         */
        private void RandomSpawn(GameObject balloon)
        {
            GameObject spawnPoint;

            if (Random.Range(0.0f, 1.0f) <= this.gameSettings.rightSpawnChance)
            {
                spawnPoint = this.rightSpawn;
            }
            else
            {
                spawnPoint = this.leftSpawn;
            }

            this.SpawnBalloon(balloon, spawnPoint);
        }
        
	    /**
         * Returns a balloon prefab based on the probability of it spawning. Probability of a balloon spawning
         * is set in the game settings.
         *
         * Author: Dante Lawrence
         * Note: Code was adapted from the probability code provided by Unity which can be found here 
         *       https://docs.unity3d.com/2019.3/Documentation/Manual/RandomNumbers.html
         *       
         * 11/2/2023: Updated, Now it picks a random balloon from the balloonPrefabs, excluding the default balloon. - Edward
         */
        private GameObject GetBalloonBasedOnProb()
        {
            GameObject balloon;

            if (Random.Range(1, 100) <= this.gameSettings.specialBalloonSpawnChance) {
                int rand;

                /* IMPORTANT: The Life balloon must be the first special balloon following the basic balloon 
                 * in the list in order to properly block it from spawning in relaxed where lives don't matter. */
                if (this.gameSettings.maxLives > 50) {
                    rand = Random.Range(2, this.gameSettings.balloonPrefabs.Count);
                } else {
                    rand = Random.Range(1, this.gameSettings.balloonPrefabs.Count);
                }
                balloon = this.gameSettings.balloonPrefabs[rand];
            } else {
                balloon = this.gameSettings.balloonPrefabs[0]; // regular balloon
            }

            return balloon;
        }

        /**
         * Given a balloon and a spawn point this method spawns a balloon at the spawn point.
         *
         * @param balloon The balloon to spawn.
         * @param spawnPoint Where to spawn the balloon.
         */
        public void SpawnBalloon(GameObject balloon, GameObject spawnPoint)
        {
            GameObject tmp = Instantiate(balloon);

            _BaseBalloon script  = tmp.GetComponent<_BaseBalloon>();
            script.SetSpawnLoc(spawnPoint);
            
            tmp.transform.position = spawnPoint.transform.position;
            balloons.Add(tmp);
        }

        /** 
         * Remove a balloon from the scene and removes it from the current list of balloons.
         *
         * @param balloon The balloon to remove.
         */
        public void KillBalloon(GameObject balloon)
        {
            balloons.Remove(balloon);
            Destroy(balloon);
        }

        /**
         * Removes a balloon from the scene and the current list of balloons after a number of 
         * seconds indicated by time.
         *
         * @param balloon The balloon to remove.
         * @param time The delay for when to remove the balloon.
         */
        public void KillBalloonDelay(GameObject balloon, int time)
        {
            StartCoroutine(KillBalloonCountdown(balloon, time));
        }

        /**
         * Coroutine that waits for a period of time before removing a balloon from the scene and the list of balloons
         */
        private IEnumerator KillBalloonCountdown(GameObject balloon, int time)
        {
            yield return new WaitForSecondsRealtime(time);
            balloons.Remove(balloon);
            Destroy(balloon);
        }

        /**
         * Removes all balloons from the scene.
         */
        public void KillAllBalloons()
        {
            Debug.Log("Calling KillALlBalloons");
            for(int i = 0; i < this.balloons.Count; ++i)
            {
                Debug.Log("Destroying a balloon!");
                Destroy(this.balloons[i]);
            }
            this.balloons.Clear();
        }

        /**
         * The AdjustLeftSpawn adjusts the left spawn of the balloons using the given offsets.
         *
         * @param x The offset in the x direction.
         * @param y The offset in the y direction.
         * @param z The offset in the z direction.
         */
        public void AdjustLeftSpawn(float x, float y, float z)
        {
            Utils.AdjustPosition(this.leftSpawn, x, y, z);
        }

        /**
         * The AdjustRightSpawn adjusts the right spawn of the darts using the given offsets.
         *
         * @param x The offset in the x direction.
         * @param y The offset in the y direction.
         * @param z The offset in the z direction.
         */
        public void AdjustRightSpawn(float x, float y, float z)
        {
            Utils.AdjustPosition(this.rightSpawn, x, y, z);
        }

        /**
         * The AdjustLeftSpawn adjusts the left and right spawns of the darts using the given offsets.
         *
         * @param x The offset in the x direction.
         * @param y The offset in the y direction.
         * @param z The offset in the z direction.
         */
        public void AdjustBothSpawns(float x, float y, float z)
        {
            AdjustLeftSpawn(x, y, z);
            AdjustRightSpawn(x, y, z);
        }
    }
}


