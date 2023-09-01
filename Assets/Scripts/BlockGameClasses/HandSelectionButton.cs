using System;
using Classes.Managers;
using UnityEngine;

namespace Classes
{
    public class HandSelectionButton : MonoBehaviour
    {
        public GameObject otherButton;
        private BoxAndBlocksGameplayManager manager;

        void Start() {
            manager = (BoxAndBlocksGameplayManager) GameplayManager.getManager();
        }

        private void OnTriggerEnter(Collider other)
        {
            if ((other.gameObject.CompareTag("RightGrabber") || other.gameObject.CompareTag("LeftGrabber")) && gameObject.CompareTag("RightButton")) {
                //PointsManager.updateScoreboardMessage("Get Ready With Your Right Hand!");
                gameObject.SetActive(false);
                otherButton.gameObject.SetActive(false);
                manager.setStartSide(GameObject.FindGameObjectWithTag("RightSpawn"));
                manager.setGoalSide(GameObject.FindGameObjectWithTag("LeftSpawn"));
                manager.leftHand = false;
                manager.startSpawning();
            } 
            else if ((other.gameObject.CompareTag("LeftGrabber") || other.gameObject.CompareTag("RightGrabber")) && gameObject.CompareTag("LeftButton")) {
                //PointsManager.updateScoreboardMessage("Get Ready With Your Left Hand!");
                gameObject.SetActive(false);
                otherButton.gameObject.SetActive(false);
                manager.setStartSide(GameObject.FindGameObjectWithTag("LeftSpawn"));
                manager.setGoalSide(GameObject.FindGameObjectWithTag("RightSpawn"));
                manager.leftHand = true;
                manager.startSpawning();
            }
        }
    }
}
        
           
        
    
