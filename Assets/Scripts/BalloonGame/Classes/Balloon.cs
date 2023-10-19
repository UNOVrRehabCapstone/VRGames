using System;
using System.Collections;
using Classes.Managers;
using UnityEngine;

public class Balloon : MonoBehaviour
{
    public  float          floatStrength;
    public  GameObject     scorePopupPrefab;

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, transform.position 
                                                              + new Vector3(0f, 1f, 0f), Time.deltaTime * floatStrength);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("DartPoint"))
        {
            /* TODO: Bug fix. There is a bug here. Basically, none of the components will work. The source of 
               this problem is that the balloon is getting destroyed too quickly. So quick that the components 
               don't have time to run before the balloon is destroyed. */
            this.GetComponent<AudioSource>().Play();
            this.GetComponentInChildren<ParticleSystem>().Play();
            this.GetComponentInParent<Rigidbody>().useGravity = true;

            Debug.Log("Balloon hit!");
            BalloonManager.Instance.KillBalloon(gameObject);

            DartRespawn.disableDart();
            Destroy(other.gameObject.transform.parent.gameObject);

            PointsManager.addPoints(1);
            //ShowScorePopup();
        }
    }

    /* For testing purposes. */
    void OnMouseDown()
    {
        this.GetComponent<AudioSource>().Play();
        BalloonManager.Instance.KillBalloon(gameObject);
        PointsManager.addPoints(1);
        Debug.Log("Total points = " + PointsManager.getPoints());
    }

    void ShowScorePopup()
    {
        GameObject popup = Instantiate(scorePopupPrefab);
        Vector3 newVector = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
        popup.transform.position = newVector;
    }
}