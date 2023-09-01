using System;
using Classes.Managers;
using UnityEngine;

public class Block : MonoBehaviour, IGrabEvent
{
    private BoxAndBlocksGameplayManager manager;
    private GameObject goalSide;

    void Start() 
    {
        manager = null;
        if (GameplayManager.getManager() is BoxAndBlocksGameplayManager)
        {
            manager = (BoxAndBlocksGameplayManager)GameplayManager.getManager();
        }

    }
    
    public void onGrab(GameObject hand)
    {
        if (hand.CompareTag("RightGrabber") && manager.getGoalSide().CompareTag("RightSpawn"))
        {
            forceReleaseBlock();
        }
        else if (hand.CompareTag("LeftGrabber") && manager.getGoalSide().CompareTag("LeftSpawn"))
        {
            forceReleaseBlock();
        }
        else if (gameObject.CompareTag("DeadBlock"))
        {
            forceReleaseBlock();
        }
        else
        {
            manager.onBlockGrabbed( gameObject );
        }
    }
    
    public void onRelease(GameObject hand)
    {
        manager.onBlockReleased( gameObject );
    }
    
    void forceReleaseBlock()
    {
        OVRGrabber grabbed = GetComponent<OVRGrabbable>().grabbedBy;
        if (grabbed != null)
        {
            grabbed.ForceRelease(grabbed.grabbedObject);
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        goalSide = manager.getGoalSide();
        if (goalSide != null)
        {
            if (other.gameObject.CompareTag(goalSide.tag) && manager.ValidPoint) 
            {
                GetComponent<MeshRenderer>().material = manager.grabbedMat;
                gameObject.tag = "DeadBlock";
                forceReleaseBlock();
            }
        }
        if (other.gameObject.CompareTag("Divider") || other.gameObject.CompareTag("BoxWall")) 
        {
            forceReleaseBlock();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (manager != null && manager.Grabbed)
        {
            if (other.gameObject.CompareTag("PartitionPointTrigger"))
            {
                manager.ValidPoint = true;
                
            }
        }
    }
}