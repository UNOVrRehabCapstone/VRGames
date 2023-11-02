/**
 * The Balloon Stream Powerup class contains the logic for the balloon stream powerup balloon. The balloon stream powerup behaves like a regular balloon, but it spawn a stream of 5 more balloons when popped.
 * The dart will not be destroyed to allow the stream to be popped easily.
 */

using System.Collections;
using System.Collections.Generic;
using Classes.Managers;
using UnityEngine;

public class Balloon_Stream_Powerup : Balloon
{
    public GameObject BalloonStream;
    private GameObject spawnPoint;
    public override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("DartPoint") && this.IsCorrectDart(other.gameObject.transform.parent.gameObject))
        {
            this.PlayEffects();
            this.AddPoints();

            /* Grabs the spawn location of the powerup (either left or right side) */
            spawnPoint = this.spawnLoc;

            /* Pauses automatic spawner to prevent the stream of balloons from overlapping with natural spawns */
            BalloonManager.Instance.StopAutomaticSpawner();
            /* Spawns the special stream of balloons on the same side as the powerup */
            BalloonManager.Instance.SpawnBalloon(BalloonStream, spawnPoint);
            /* Restarts the automatic spawner after 5 seconds have elapsed */
            BalloonManager.Instance.StartAutomaticSpawner(5);

            BalloonManager.Instance.KillBalloon(gameObject);
            //DartManager.Instance.DestroyDart(other.gameObject.transform.parent.gameObject);
        }
    }

    /* For testing purposes. Useful for testing on the computer rather than in the headset. */
    public override void OnMouseDown()
    {
        Debug.Log(this.ToString() + " popped. Worth " + this.pointValue + " points.");

        this.PlayEffects();
        this.AddPoints();

        /* Grabs the spawn location of the powerup (either left or right side) */
        spawnPoint = this.spawnLoc;

        /* Pauses automatic spawner to prevent the stream of balloons from overlapping with natural spawns */
        BalloonManager.Instance.StopAutomaticSpawner();
        /* Spawns the special stream of balloons on the same side as the powerup */
        BalloonManager.Instance.SpawnBalloon(BalloonStream, spawnPoint);
        /* Restarts the automatic spawner after 5 seconds have elapsed */
        BalloonManager.Instance.StartAutomaticSpawner(5);

        BalloonManager.Instance.KillBalloon(gameObject);
    }
}
