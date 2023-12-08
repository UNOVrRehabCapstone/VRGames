using System.Collections;
using System.Collections.Generic;
using Classes.Managers;
using UnityEngine;

/**
 * The Balloon Onion class contains the logic for the onion special balloon. The onion special 
 * balloon is like an onion; there are three layers where each layer is worth more points than 
 * the previous layer.
 */
public class Balloon_Onion : Balloon
{
    public override void ExtraPopEffects()
    {
        this.PopBalloonEvent();
        /* If the final layer is popped, make sure to remove the unit balloon (the parent object)
        from the scene. */

        if (gameObject.CompareTag("OnionLayer3"))
        {
            BalloonManager.Instance.KillBalloon(gameObject.transform.parent.gameObject);
        }
        BalloonManager.Instance.KillBalloon(gameObject);
    }
}
