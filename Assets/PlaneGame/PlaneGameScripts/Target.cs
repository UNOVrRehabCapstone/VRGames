using System;
using System.Collections;
using UnityEngine;

namespace Classes
{
    public class Target : MonoBehaviour
    {
        public static int targetsInScene = 0;
        private void Start()
        {
            Target.targetsInScene++;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("RightPlane") || other.gameObject.CompareTag("LeftPlane") || other.gameObject.CompareTag("plane"))
            {
                PointsManager.addPoints(1);
                GetComponentInChildren<ParticleSystem>().Play();
                foreach (var r in gameObject.GetComponentsInChildren<MeshRenderer>())
                {
                    r.enabled = false;
                }
                //manager.KillHoop(gameObject);
                //manager.StartSpawningHoops();
            }
        }
    }
}


