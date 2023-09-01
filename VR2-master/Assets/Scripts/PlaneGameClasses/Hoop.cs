using System;
using System.Collections;
using UnityEngine;

namespace Classes
{
    public class Hoop : MonoBehaviour
    {
        private PlaneGameplayManager manager;
        private void Start()
        {
            manager = (PlaneGameplayManager)GameplayManager.getManager();
        }

        void OnTriggerEnter(Collider other) 
        {
            if (other.gameObject.CompareTag("RightPlane") || other.gameObject.CompareTag("LeftPlane"))
            {
                PointsManager.addPoints( 1 );
                GetComponentInChildren<ParticleSystem>().Play();
                foreach (var r in gameObject.GetComponentsInChildren<MeshRenderer>())
                {
                    r.enabled = false;
                }
                manager.KillHoop(gameObject);
                manager.StartSpawningHoops();
            }
        }
    }
}


