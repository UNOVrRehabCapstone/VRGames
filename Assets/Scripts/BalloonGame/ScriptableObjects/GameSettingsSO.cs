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

	public string           gameModeStr;
	public GameMode         gameMode;

	public List<GameObject> balloonPrefabs;
	public List<float>      probabilities;

	public int              maxNumBalloonsSpawnedAtOnce;

	public float            maxSpawnTime;
	public float            minSpawnTime;

	public int              numLives;
	public int              goal;

	public int              difficulty;
}
