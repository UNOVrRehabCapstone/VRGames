using System;
using Classes.Managers;
using UnityEngine;

namespace Classes
{
    public class BlockSpawn : MonoBehaviour
    {
        private BoxAndBlocksGameplayManager manager;
        private GameObject goalSide;
        
        void Start() 
        {
            manager = (BoxAndBlocksGameplayManager)GameplayManager.getManager();
        }
        
        private void OnTriggerEnter(Collider other)
        {
            goalSide = manager.getGoalSide();
            if (goalSide != null)
            {
                if (manager.ValidPoint)
                {
                   if (other.gameObject.CompareTag("Block") && gameObject.CompareTag(goalSide.tag))
                   {
                      if (manager.leftHand) 
                      {
                          PointsManager.addLeftPoints(1);
                      } 
                      else 
                      {
                          PointsManager.addRightPoints(1);
                      }
                   } 
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.CompareTag("Block"))
            {
                manager.ValidPoint = false;
            }
        }
    }
}