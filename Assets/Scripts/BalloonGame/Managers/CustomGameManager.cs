using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BalloonsGame
{
	public class CustomGameManager : BalloonGameManager
	{
		private void Start()
		{
			if (this.gameSettings.maxLives < 50) {
				// Subscribe to watching player lives
			}

			// Subscribe to watching the score
			//BalloonManager.Instance.StartAutomaticSpawner(3.0f);
		}
	}
}
