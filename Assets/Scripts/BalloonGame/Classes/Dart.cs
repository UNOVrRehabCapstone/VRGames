using System;
using System.Collections;
using Classes.Managers;
using UnityEngine;

public class Dart : MonoBehaviour, IGrabEvent
{
    private BalloonGameplayManager manager;

    void Start() 
    {
        manager = (BalloonGameplayManager) GameplayManager.getManager();
    }

    public void onGrab(GameObject hand)
    {
        
    }
    public void onRelease(GameObject hand){
        
    }

    public void OnTriggerEnter(Collider other)
    {
        /*
        if (other.gameObject.CompareTag("LeftTarget"))
        {
            Debug.Log("Hit the left balloon!");
            /* Interesting bug I found. You need other.gameObject.transform.parent.gameObject
               because the collider is NOT the balloon. The collider is attached to the sphere 
               which is a child of the balloon game object. In other words, other.gameObject is 
               a sphere, not a balloon, so attempting to call KillBalloon(other.gameObject) will 
               not work. */ 
            /*
            BalloonManager.Instance.KillBalloon(other.gameObject.transform.parent.gameObject);
            KillDart(gameObject);
            PointsManager.addLeftPoints(1);
            PointsManager.addPoints(1);
        } 
        if (other.gameObject.CompareTag("RightTarget"))
        {
            Debug.Log("Hit the right balloon!");
            BalloonManager.Instance.KillBalloon(other.gameObject.transform.parent.gameObject);
            KillDart(gameObject);
            PointsManager.addRightPoints(1);
            PointsManager.addPoints(1);
        }*/
    }

    private void KillDart(GameObject dart)
    {
        DartRespawn.disableDart();
        Destroy(dart);
    }
}