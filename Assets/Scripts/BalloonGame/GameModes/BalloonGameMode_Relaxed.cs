using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonGameMode_Relaxed : BalloonGameMode
{
	public override GameStatus UpdateGame()
	{
		return GameStatus.PLAYING;
	}
}
