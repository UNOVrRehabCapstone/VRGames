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

                Debug.Log("Destroying Target");
                Target.targetsInScene--;
                gameObject.GetComponent<MeshRenderer>().enabled = false;
                gameObject.GetComponent<MeshCollider>().enabled = false;
                foreach (var r in gameObject.GetComponentsInChildren<MeshRenderer>())
                {
                    r.enabled = false;
                }

                manager.KillHoop(gameObject);
                manager.StartSpawningHoops();


                Destroy(gameObject, 3.0f);
            }
        }
        */
        public void HitTarget()
        {
            GetComponentInChildren<ParticleSystem>().Play();

            Debug.Log("DestroyingTarget");
            Target.targetsInScene--;
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            gameObject.GetComponent<MeshCollider>().enabled = false;

            foreach (var r in gameObject.GetComponentsInChildren<MeshRenderer>())
            {
                r.enabled = false;
            }

            Destroy(gameObject, 3.0f);
            PointsManager.addPoints(1);
    }
}


