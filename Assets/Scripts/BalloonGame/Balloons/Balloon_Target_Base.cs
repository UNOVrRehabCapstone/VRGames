using System.Collections;
using System.Collections.Generic;
using Classes.Managers;
using UnityEngine;

/**
 * The Balloon_Target_Base class handels the logic for the target balloon.
 */
public class Balloon_Target_Base : Balloon
{
    private int numOfTargetsRemaining = 6;

    public int testInt = 100;

    private void Start()
    {
        this.isPersistent = true;
    }

    public override void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Child"))
        {
            Debug.Log("Child layer entered");
        }
        if(other.gameObject.layer == LayerMask.NameToLayer("Parent"))
        {
            Debug.Log("Parent layer entered");
        }

    }

    public override void OnMouseDown()
    {

    }


    public override void ExtraPopEffects()
    {
        this.PopBalloonEvent();
        if(this.numOfTargetsRemaining <= 0)
        {
            BalloonManager.Instance.KillBalloon(gameObject);
        }

    }


    public void TargetHit()
    {
        this.numOfTargetsRemaining--;
        
        if (this.numOfTargetsRemaining <= 0)
        {
            this.AddPoints();
            this.messageOverride = "Target Balloon Fully Popped";
            this.isPersistent = false;
        }
        this.PlayEffects(isPersistent);
        this.ExtraPopEffects();
    }



    
}
