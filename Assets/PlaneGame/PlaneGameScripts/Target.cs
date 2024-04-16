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

        public void hitTarget()
        {

            this.GetComponent<MeshCollider>().enabled = false;
            this.GetComponent<MeshRenderer>().enabled = false;
            foreach (var r in gameObject.GetComponentsInChildren<MeshRenderer>())
            {
                r.GetComponent<MeshRenderer>().enabled = false;
            }

            Destroy(gameObject, 3.0f);
        }
    }
}


