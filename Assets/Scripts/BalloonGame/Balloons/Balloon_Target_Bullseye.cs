using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balloon_Target_Bullseye : MonoBehaviour
{
    private void Update()
    {
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

    public  void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("DartPoint") && this.IsCorrectDart(other.gameObject.transform.parent.gameObject))
        {
            Destroy(gameObject);
        }
    }


    protected bool IsCorrectDart(GameObject dart)
    {
        GameObject spawnLocation = transform.parent.GetComponent<Balloon>().GetSpawnLoc();
        return

            (spawnLocation.CompareTag("BalloonSpawn_Left") && DartManager.Instance.IsLeftDart(dart))
         || (spawnLocation.CompareTag("BalloonSpawn_Right") && DartManager.Instance.IsRightDart(dart));
    }

}
