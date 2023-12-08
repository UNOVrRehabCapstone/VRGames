using System.Collections;
using System.Collections.Generic;
using Classes.Managers;
using UnityEngine;

/**
 * The Balloon Stream Powerup class contains the logic for the balloon stream powerup balloon. The balloon stream powerup behaves like a regular balloon, but it spawn a stream of 5 more balloons when popped.
 * The dart will not be destroyed to allow the stream to be popped easily.
 */
public class Balloon_Stream_Powerup : Balloon
{
    public GameObject BalloonStream;
    private GameObject spawnPoint;


    public override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("DartPoint") && this.IsCorrectDart(other.gameObject.transform.parent.gameObject))
        {
            this.AddPoints();
            this.PlayEffects(this.isPersistent);
            this.ExtraPopEffects();

        }
    }

    public override void ExtraPopEffects()
    {
        this.PopBalloonEvent();

        /* Grabs the spawn location of the powerup (either left or right side) */
        spawnPoint = this.spawnLoc;

        /* Pauses automatic spawner to prevent the stream of balloons from overlapping with natural spawns */
        if(BalloonGameplayManager.Instance.gameSettings.gameMode == GameSettingsSO.GameMode.CUSTOM)
        {
            BalloonManager.Instance.StopAutomaticSpawner();
        }
        /* Spawns the special stream of balloons on the same side as the powerup */
        BalloonManager.Instance.SpawnBalloon(BalloonStream, spawnPoint);
        /* Restarts the automatic spawner after 5 seconds have elapsed */
        if (BalloonGameplayManager.Instance.gameSettings.gameMode == GameSettingsSO.GameMode.CUSTOM)
        {
            BalloonManager.Instance.StartAutomaticSpawner(5);
        }
        BalloonManager.Instance.KillBalloon(gameObject);

    }
}
