/**
 * A scriptable object template that holds the game settings. Scriptable obejcts can be created 
 * from this to create different game settings. 
 *
 * Authors: Dante Lawrence
 **/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(menuName = "ScriptableObjects/GameSettings", fileName = "GameSettingsSO")]
public class GameSettingsSO : ScriptableObject
{
	public enum GameMode {
		RELAXED,               /* No way to lose and no negative balloons. Game ends when the goal is met or the patient 
								  or clincian quit. */
		
		NORMAL,                /* Patient has lives and there is a goal. All balloons enabled. Game ends when either the 
		                          patient runs out of lives or the goal is met (or by intervention of the clinician). */
	
		ENDLESS                /* Patient has lives, but there is no goal. Score as many points as you can! All balloons 
							      enabled. Game ends when patient loses all lives (or by intervention of the clinician). */
	}

	public enum SpawnPattern {
		CONCURRENT,            /* Balloons spawn from the left and the right at the same time. */
		ALTERNATING            /* Balloons spawn in an alternating pattern. */
	}

	/* TODO: A lot of mixed settings here. Would it be better to separate the 
	   settings into their own files? For example, a file for spawn settings, 
	   a file for player settings, and a file for game mode settings. Hmmm 
	   not sure. */

	/*************************************************************************/
	/* GAME MODE SETTINGS                                                    */
	/*************************************************************************/
	public GameMode         gameMode;

	public int              numLives;
	public int              goal;

	/*************************************************************************/
	/* SPAWN SETTINGS                                                        */
	/*************************************************************************/
	public SpawnPattern     spawnPattern;

	public List<GameObject> balloonPrefabs;

	public int				specialBalloonSpawnChance;

	public int              maxNumBalloonsSpawnedAtOnce;

	public float            spawnTime;
}
