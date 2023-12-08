using Classes.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * The Balloon Restore Life class contains the logic for the restore life special balloon.
 * The Restore Life balloon gives the player an extra life when popped.
 */
public class Balloon_RestoreLife : Balloon
{
    public override void ExtraPopEffects()
    {
        BalloonGameplayManager.Instance.playerLives++;
        PointsManager.updateScoreboard();
        this.PopBalloonEvent();
        BalloonManager.Instance.KillBalloon(gameObject);
    }
}
