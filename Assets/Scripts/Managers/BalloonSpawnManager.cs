using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonSpawnManager : MonoBehaviour
{	
	public static BalloonSpawnManager Instance {get; private set;}

    private float secondsTilDespawn = 5;
    [SerializeField] private GameObject leftSpawn;
    [SerializeField] private GameObject rightSpawn;

	private List<GameObject> balloons = new List<GameObject>();

	private void Awake()
	{
		if (Instance != null) {
			Debug.LogError("There should be only one balloon spawn manager.");
		}
		Instance = this;
	}

	public void SpawnBalloons(GameSettingsSO gameSettings)
	{
        Debug.Log("Entered");
        if (this.balloons.Count < gameSettings.maxNumBalloonsSpawnedAtOnce) {
            switch (gameSettings.spawnPattern) {
                case 0: //Concurrent
                    Debug.Log("Concurrent spawn pattern chosen");
                    GameObject leftBalloon  = GetBalloonBasedOnProb(gameSettings);
                    GameObject rightBalloon = GetBalloonBasedOnProb(gameSettings);
                    SpawnBalloon(leftBalloon, leftSpawn);
                    SpawnBalloon(rightBalloon, rightSpawn);

                    break;
                default:
                    Debug.LogError("This should never happen.");
                    break;
            }
        }
	}

	/**
     * Selects a balloon prefab based on the probability of it spawning.
     *
     * Author: Dante Lawrence
     * Note: Code was adapted from the probability code provided by Unity which can be found here 
     *       https://docs.unity3d.com/2019.3/Documentation/Manual/RandomNumbers.html
     */
    private GameObject GetBalloonBasedOnProb(GameSettingsSO gameSettings)
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
     * A more generic version of the spawn left balloon and spawn right balloon methods. Just provide
     * the spawn point and balloon to spawn.
     *
     * Author: Dante Lawrence
     */
    private void SpawnBalloon(GameObject balloon, GameObject spawnPoint)
    {
        GameObject tmp = Instantiate(balloon);
        tmp.transform.position = spawnPoint.transform.position;
        balloons.Add(tmp);
        StartCoroutine(despawnCountdown(tmp));
    }

    //Removes a balloon from the list and destroys it.
    private void killBalloon(GameObject balloon)
    {
        balloons.Remove(balloon);
        Destroy(balloon);
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
