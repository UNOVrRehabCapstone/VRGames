using System.Collections;
using System.Collections.Generic;
using Classes.Managers;
using UnityEngine;

public class Balloon_Target_Base : Balloon
{
    private int numOfTargetsRemaining = 6;

    private void Start()
    {
        this.isPersistent = true;
    }

    public override void OnTriggerEnter(Collider other)
    {

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
        this.AddPoints();
        if (this.numOfTargetsRemaining <= 0)
        {
            this.messageOverride = "Target Balloon Fully Popped";
            this.isPersistent = false;
        }
        this.PlayEffects(isPersistent);
        this.ExtraPopEffects();
    }



    
}
