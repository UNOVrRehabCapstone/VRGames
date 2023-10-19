using System;
using System.Collections;
using Classes.Managers;
using UnityEngine;

public class Dart : MonoBehaviour, IGrabEvent
{
    private BalloonGameplayManager manager;
    private Vector3 originalPosition;

    void Start() 
    {
        manager = (BalloonGameplayManager) GameplayManager.getManager();
        originalPosition = transform.position;
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
            ResetDart(gameObject);
            PointsManager.addLeftPoints(1);
        } 
        if (other.gameObject.CompareTag("RightTarget"))
        {
            Debug.Log("Hit the right balloon!");
            BalloonManager.Instance.KillBalloon(other.gameObject);
            ResetDart(gameObject);
            PointsManager.addRightPoints(1);
        }
    }

    private void ResetDart(GameObject dart)
    {
       dart.transform.position = originalPosition;
    }
}