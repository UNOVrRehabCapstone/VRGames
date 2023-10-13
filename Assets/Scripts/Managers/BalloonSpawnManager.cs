using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonSpawnManager : MonoBehaviour
{	
	public static BalloonSpawnManager Instance {get; private set;}

	private List<GameObject> balloons = new List<GameObject>();

	private void Awake()
	{
		if (Instance != null) {
			Debug.LogError("There should be only one balloon spawn manager.");
		}
		Instance = this;
	}

	public void SpawnBalloons(GameSettingSO chosenGameSettings)
	{
		this.Hello();
	}

	private void Hello()
	{
		Debug.Log("Hello");
	}
}
