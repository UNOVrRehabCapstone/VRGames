using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BalloonsGame
{
	public class CustomGameManager : GameManager
	{
		private void Start()
		{
			if (this.gameSettings.maxLives < 50) {
				PlayerManager.Instance.OnAllLivesLost += this.AllLivesLostHandler;
			}

			// Subscribe to watching the score
			// BalloonSpawnManager.Instance.StartAutomaticSpawner(3.0f);
		}
	}
}
