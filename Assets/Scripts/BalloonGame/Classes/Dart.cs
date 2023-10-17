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
        if (other.gameObject.CompareTag("LeftTarget"))
        {
            Debug.Log("Hit the left balloon!");
            BalloonManager.Instance.KillBalloon(other.gameObject);
            KillDart(gameObject);
            PointsManager.addLeftPoints(1);
        } 
        if (other.gameObject.CompareTag("RightTarget"))
        {
            Debug.Log("Hit the right balloon!");
            BalloonManager.Instance.KillBalloon(other.gameObject);
            KillDart(gameObject);
            PointsManager.addRightPoints(1);
        }
    }

    private void KillDart(GameObject dart)
    {
        DartRespawn.disableDart();
        Destroy(dart);
    }
}