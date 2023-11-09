/**
 * The Balloon Restore Life class contains the logic for the restore life special balloon.
 * The Restore Life balloon gives the player an extra life when popped.
 */

using Classes.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balloon_RestoreLife : Balloon
{
    public override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("DartPoint") && this.IsCorrectDart(other.gameObject.transform.parent.gameObject))
        {
            this.PlayEffects();
            BalloonGameplayManager.Instance.playerLives++;
            this.AddPoints();


            //Debug.Log("Gained a life. Remaining lives: " + BalloonGameplayManager.Instance.playerLives);

            BalloonManager.Instance.KillBalloon(gameObject);
            DartManager.Instance.DespawnDart(other.gameObject.transform.parent.gameObject);
        }
    }

    /* For testing purposes. Useful for testing on the computer rather than in the headset. */
    public override void OnMouseDown()
    {
        Debug.Log(this.ToString() + " popped. Worth " + this.pointValue + " points.");

        this.PlayEffects();
        BalloonGameplayManager.Instance.playerLives++;
        this.AddPoints();

        //Debug.Log("Gained a life. Remaining lives: " + BalloonGameplayManager.Instance.playerLives);

        BalloonManager.Instance.KillBalloon(gameObject);
    }
}
