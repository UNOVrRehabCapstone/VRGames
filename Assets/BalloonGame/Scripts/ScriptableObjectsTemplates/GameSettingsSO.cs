using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 * A scriptable object template that holds the game settings. Scriptable obejcts can be created 
 * from this to create different game settings. 
 *
 * Authors: Dante Lawrence
 */
[CreateAssetMenu(menuName = "ScriptableObjects/GameSettings", fileName = "GameSettingsSO")]
public class GameSettingsSO : ScriptableObject
{
	/**
	 * Can be used to represent one of the game modes.
	 */
	public enum GameMode {
		CAREER,               /* No way to lose and no negative balloons. Game ends when the goal is met or the patient 
								  or clincian quit. */
		
		CUSTOM,                /* Patient has lives and there is a goal. All balloons enabled. Game ends when either the 
		                          patient runs out of lives or the goal is met (or by intervention of the clinician). */
	
		MANUAL                /* Clinician manually spawns balloons one at a time
		                       * THIS IS NOW AN INTERNAL MODE ONLY, CLINICIANS 
		                          DO NOT SEE THIS ANYMORE*/
	}

	/**
	 * Can be used to represent one of the spawn patterns.
	 */
	public enum SpawnPattern {
		CONCURRENT,            /* Balloons spawn from the left and the right at the same time. */
		ALTERNATING,           /* Balloons spawn in an alternating pattern. */
		RANDOM				   /* Balloons randomly spawn on the left or right based on leftSpawnChance. */
	}

	/**
	 * Can be used to represent the goal type.
	 */
	public enum HandSetting {
		LEFT_HAND,			/* The goal is affected only by the left hand. */
		RIGHT_HAND,			/* The goal is affected only by the right hand. */
		BOTH_HANDS			/* The goal is affected by both hands. */
	}

	/**
	 * Can be used to represent one of the environments.
	 */
	public enum Environment {
		ORIGINAL,
		MEADOW
	}

	/* TODO: A lot of mixed settings here. Would it be better to separate the 
	   settings into their own files? For example, a file for spawn settings, 
	   a file for player settings, and a file for game mode settings. Hmmm 
	   not sure. */

	/*************************************************************************/
	/* GAME MODE SETTINGS                                                    */
	/*************************************************************************/
	public GameMode         gameMode; 
	public HandSetting 		handSetting;

	public int              maxLives;
	public int              goal;

	/*************************************************************************/
	/* SPAWN SETTINGS                                                        */
	/*************************************************************************/
	public SpawnPattern     spawnPattern;

	public List<GameObject> balloonPrefabs;

	public int				specialBalloonSpawnChance;

	public int              maxNumBalloonsSpawnedAtOnce;


	public float            spawnTime;

    public float			rightSpawnChance;       /* leftSpawnChance is not needed because it is 1 - rightSpawnChance.
                			                        * i.e. A rightSpawnChance of 0.6 indicates a leftSpawnChance of 0.4. */

	public float			floatStrengthModifier; /* BE CAREFUL! Allowing this value to be set too high will cause
	            			                        * the balloons to skip right over the killzone barrier. This
	            			                        * prevents any further balloons from spawning once the number of
	            			                        * balloons that skipped the barrier is equal to maxNumBalloonsSpawnedAtOnce */
	
	/*************************************************************************/
	/* ENVIRONMENT SETTINGS                                                  */
	/*************************************************************************/
	public Environment      environment;
}
