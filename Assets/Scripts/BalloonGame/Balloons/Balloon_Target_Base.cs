using System.Collections;
using System.Collections.Generic;
using Classes.Managers;
using UnityEngine;

public class Balloon_Target_Base : Balloon
{
    private int numOfTargetsRemaining = 6;


    public override void OnTriggerEnter(Collider other)
    {

    }

    public override void OnMouseDown()
    {

    }

    public override void PopEffects(Collider other)
    {
        this.AddPoints();
        if(numOfTargetsRemaining <= 0)
        {
            this.messageOverride = "Target Balloon Fully Popped";
        }
        this.PopBalloonEvent();
        this.ExtraPopEffects();

    }




    
    public override void ExtraPopEffects()
    {
        if (numOfTargetsRemaining <= 0)
        {
            this.PlayEffects(false);
            BalloonManager.Instance.KillBalloon(gameObject);
        }
        else
        {
            this.PlayEffects(true);
        }

    }
    public void TargetHit()
    {
        this.numOfTargetsRemaining--;
        this.PopEffects(null);
    }



    
}
