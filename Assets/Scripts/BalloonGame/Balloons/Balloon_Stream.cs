/**
 * The Balloon Stream class contains the logic for the balloon stream. The balloon stream is simply a series of 5 regular balloons that are spawned simultaneously.
 * These balloons do not destroy the dart so the player can pop the balloons in quick succession.
 */

using Classes.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balloon_Stream : Balloon
{
    public override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("DartPoint") && this.IsCorrectDart(other.gameObject.transform.parent.gameObject))
        {
            this.PlayEffects();
            this.AddPoints();

            /**
             * This tag is applied to the last balloon in the balloon stream.
             * The parent object should be destroyed after this balloon is popped.
             */
            if (gameObject.CompareTag("SpawnStreamMemberLast"))
            {
                /**
                 * Destroys the parent object of the stream of balloons 3 seconds after the last balloon in the stream is popped.
                 * A delay is required to prevent the rest of the stream from being immediately destroyed if the last balloon is popped before some/all of the prior balloons in the stream.
                 */
                BalloonManager.Instance.KillBalloonDelay(gameObject.transform.parent.gameObject, 3);
            }
            BalloonManager.Instance.KillBalloon(gameObject);
        }
    }

    /* For testing purposes. Useful for testing on the computer rather than in the headset. */
    public override void OnMouseDown()
    {
        Debug.Log(this.ToString() + " popped. Worth " + this.pointValue + " points.");

        this.PlayEffects();
        this.AddPoints();

        /**
         * This tag is applied to the last balloon in the balloon stream.
         * The parent object should be destroyed after this balloon is popped.
         */
        if (gameObject.CompareTag("SpawnStreamMemberLast"))
        {
            /**
             * Destroys the parent object of the stream of balloons 3 seconds after the last balloon in the stream is popped.
             * A delay is required to prevent the rest of the stream from being immediately destroyed if the last balloon is popped before some/all of the prior balloons in the stream.
             */
            BalloonManager.Instance.KillBalloonDelay(gameObject.transform.parent.gameObject, 3);
        }
        BalloonManager.Instance.KillBalloon(gameObject);
    }
}
