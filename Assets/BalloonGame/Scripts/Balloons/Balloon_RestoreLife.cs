using Classes.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BalloonsGame
{
	/**
     * The Balloon Restore Life class contains the logic for the restore life special balloon.
     * The Restore Life balloon gives the player an extra life when popped.
     */
    public class Balloon_RestoreLife : Balloon
    {
        public override void ExtraPopEffects()
        {
            PlayerManager.Instance.IncrementLife();
            this.PopBalloonEvent();
            BalloonSpawnManager.Instance.KillBalloon(gameObject);
        }
    }
}

