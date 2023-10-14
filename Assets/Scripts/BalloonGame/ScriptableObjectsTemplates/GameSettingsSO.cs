using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(menuName = "ScriptableObjects/GameSettings", fileName = "GameSettingsSO")]
public class GameSettingsSO : ScriptableObject
{
	public enum GameMode {
		RELAXED,
		NORMAL,
		ENDLESS
	}

	public enum SpawnPattern {
		CONCURRENT,
		ALTERNATING
	}

	public string           gameModeStr;
	public GameMode         gameMode;

	public List<GameObject> balloonPrefabs;
	public List<float>      probabilities;
	public GameObject       leftSpawn;
	public GameObject       rightSpawn;

	public int              maxNumBalloonsSpawnedAtOnce;

	public float            maxSpawnTime;
	public float            minSpawnTime;
	public float            nextSpawnTime;

	public int              numLives;
	public int              goal;

	public int              spawnPattern;

	public float            despawnCountdown;
}
