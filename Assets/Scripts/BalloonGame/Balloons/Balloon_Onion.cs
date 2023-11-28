/**
 * The Balloon Onion class contains the logic for the onion special balloon. The onion special 
 * balloon is like an onion; there are three layers where each layer is worth more points than 
 * the previous layer.
 */

using System.Collections;
using System.Collections.Generic;
using Classes.Managers;
using UnityEngine;

public class Balloon_Onion : Balloon
{
    public override void PopEffects(Collider other)
    {
        this.PopBalloonEvent();
        this.AddPoints();
        this.ExtraPopEffects();
        this.PlayEffects(false);
        Destroy(gameObject);
        if (other != null)
        {
            DartManager.Instance.DespawnDart(other.gameObject.transform.parent.gameObject);
        }
    }

    public override void ExtraPopEffects()
    {
        /* If the final layer is popped, make sure to remove the unit balloon (the parent object)
        from the scene. */
        if (gameObject.CompareTag("OnionLayer3"))
        {
            BalloonManager.Instance.KillBalloon(gameObject.transform.parent.gameObject);
        }
    }
}
