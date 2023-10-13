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

	public GameMode         gameMode;

	public List<GameObject> balloonPrefabs;
	public List<float>      probabilities;

	public int              numBalloonsSpawnedAtOnce;

	public float            maxSpawnTime;
	public float            minSpawnTime;

	public int              numLives;
	public int              goal;
}
