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
            BalloonSpawnManager.Instance.KillBalloon(other.gameObject);
            manager.killDart(gameObject);
            PointsManager.addLeftPoints(1);
        } 
        if (other.gameObject.CompareTag("RightTarget"))
        {
            Debug.Log("Hit the right balloon!");
            BalloonSpawnManager.Instance.KillBalloon(other.gameObject);
            manager.killDart(gameObject);
            PointsManager.addRightPoints(1);
        }
    }
}