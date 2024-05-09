using System;
using System.Collections;
using UnityEngine;
using PlanesGame;

namespace Classes
{
    public class Target : MonoBehaviour
    {

        GameObject TargetManager;

        //used for scoring only
        //GameObject Manager;
        

        private void Start()
        {
            //Manager = GameObject.FindGameObjectWithTag("Manager");
            TargetManager = GameObject.FindGameObjectWithTag("TargetManager");

            TargetManager.GetComponent<TargetSpawner>().AddTarget();
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("RightPlane") || other.gameObject.CompareTag("LeftPlane") || other.gameObject.CompareTag("plane"))
            {
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
            
            TargetManager.GetComponent<TargetSpawner>().RemoveTarget();

            //Manager.GetComponent<PointsManager>().addPoints(1);
            PointsManager.addPoints(1);
            

            Destroy(gameObject, 3.0f);
        }


    }
}


