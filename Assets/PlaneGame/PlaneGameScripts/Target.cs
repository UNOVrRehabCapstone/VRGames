/**
 * \file Target.cs
 * \brief Defines behavior for target objects in the game.
 * 
 *  The Target class manages the behavior of target objects in the game, including their initialization, collision handling, and removal from the scene upon being hit.
 */


using System;
using System.Collections;
using UnityEngine;
using PlanesGame;

namespace Classes
{
  /**
  * \class Target
  * \brief Manages the behavior of target objects in the game.
  *
  * The Target class handles the initialization of target objects, triggers when they are hit by other game objects, and manages their removal from the scene.
  */
  public class Target : MonoBehaviour
    {

        GameObject TargetManager;

    //used for scoring only
    //GameObject Manager;


     /**
      * \brief Initializes the target object by finding the TargetManager and adding itself to the target count.
      */
    private void Start()
        {
            //Manager = GameObject.FindGameObjectWithTag("Manager");
            TargetManager = GameObject.FindGameObjectWithTag("TargetManager");

            TargetManager.GetComponent<TargetSpawner>().AddTarget();
        }

    /**
     * \brief Handles collision events with the target object.
     * 
     * Triggers when the target is hit by other game objects such as planes, plays particle effects, hides the target's mesh renderers, and removes the target from the scene.
     * 
     * \param other The collider of the object colliding with the target.
     */
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

    /**
     *  \brief Handles the event when the target is hit.
     * 
     * Disables the target's collider and mesh renderer, removes the target from the scene, and updates the score.
     */
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


