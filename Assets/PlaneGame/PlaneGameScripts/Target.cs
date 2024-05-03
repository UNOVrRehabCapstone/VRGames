using System;
using System.Collections;
using UnityEngine;

namespace Classes
{
    public class Target : MonoBehaviour
    {

        GameObject TargetManager;

        private void Start()
        {
            GameObject TargetManager = GameObject.FindGameObjectWithTag("TargetManager");

            TargetManager.GetComponent<TargetSpawner>().AddTarget();
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

        public void HitTarget()
        {

            this.GetComponent<MeshCollider>().enabled = false;
            this.GetComponent<MeshRenderer>().enabled = false;
            foreach (var r in gameObject.GetComponentsInChildren<MeshRenderer>())
            {
                r.GetComponent<MeshRenderer>().enabled = false;
            }

            TargetManager = GameObject.FindGameObjectWithTag("TargetManager");
            TargetManager.GetComponent<TargetSpawner>().RemoveTarget();

            

            Destroy(gameObject, 3.0f);
        }


    }
}


