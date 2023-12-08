using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * The Balloon_Target_Bullseye class handles the logic for targets that appear on balloons.
 */
public class Balloon_Target_Bullseye : MonoBehaviour
{

    private Balloon_Target_Base baseScript;
    private void Start()
    {
        baseScript = transform.parent.transform.parent.GetComponent<Balloon_Target_Base>();
    }

    private void Update()
    {// NOTE THIS MAY NOT WORK IN VR, HAVE NOT TESTED YET

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit))
            {
                if(hit.collider.gameObject == gameObject)
                {
                    Destroy(gameObject);
                    transform.parent.transform.parent.GetComponent<Balloon_Target_Base>().TargetHit();
                }
            }
        }
    }

    /**
     * The OnTriggerEnter method handles the logic for when another object collides with the object 
     * this script is attached to.
     *
     * @param other The object that collided with the collider of the gameobject this script is 
     * attached to.
     */
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("DartPoint") && this.IsCorrectDart(other.gameObject.transform.parent.gameObject))
        {
            baseScript.TargetHit();
            Destroy(gameObject);
            if (other != null)
            {
                if( gameObject.CompareTag("Balloon_Stream_Powerup") || 
                    gameObject.CompareTag("SpawnStreamMember")      ||
                    gameObject.CompareTag("SpawnStreamMemberLast"))
                    {
                    Debug.Log("Do not destroy dart");
                }
                else
                {
                    DartManager.Instance.DespawnDart(other.gameObject.transform.parent.gameObject);
                }
                
            }
        }
    }

    /**
     * The IsCorrectDart method returns true or false depending on whether the balloon and the dart 
     * are on the same side. 
     *
     * @param dart The dart to be checked against.
     */
    protected bool IsCorrectDart(GameObject dart)
    {
        GameObject spawnLocation = baseScript.GetSpawnLoc();
        return

            (spawnLocation.CompareTag("BalloonSpawn_Left") && DartManager.Instance.IsLeftDart(dart))
         || (spawnLocation.CompareTag("BalloonSpawn_Right") && DartManager.Instance.IsRightDart(dart));
    }

}
