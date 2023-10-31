using System.Collections;
using System.Collections.Generic;
using Classes.Managers;
using UnityEngine;

public class Balloon_Onion : Balloon
{
	public override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("DartPoint") && this.IsCorrectDart(other.gameObject.transform.parent.gameObject))
        {
            this.PlayEffects();
            this.AddPoints();

            /* If the final layer is popped, make sure to remove the unit balloon (the parent object)
               from the scene. */
            if (gameObject.CompareTag("OnionLayer3")) {
                BalloonManager.Instance.KillBalloon(gameObject.transform.parent.gameObject);
            }
            Destroy(gameObject);
            DartManager.Instance.DestroyDart(other.gameObject.transform.parent.gameObject);
        }
    }

    /* For testing purposes. Useful for testing on the computer rather than in the headset. */
    public override void OnMouseDown()
    {
        Debug.Log(this.ToString() + " popped. Worth " + this.pointValue + " points.");
        
        this.PlayEffects();
        this.AddPoints();
        
        /* If the final layer is popped, make sure to remove the parent balloon from the scene. */
        if (gameObject.CompareTag("OnionLayer3")) {
            BalloonManager.Instance.KillBalloon(gameObject.transform.parent.gameObject);
        }
        Destroy(gameObject);
    }
}
