using System;
using System.Collections;
using Classes.Managers;
using UnityEngine;

public class IncreaseLifeBalloon : MonoBehaviour
{
    public float floatStrength;
    public GameObject scorePopupPrefab;
    private BalloonGameplayManager manager;

    void Start() 
    {
        manager = (BalloonGameplayManager) GameplayManager.getManager();
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, transform.position 
                                                              + new Vector3(0f, 1f, 0f), Time.deltaTime * floatStrength);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Popped increase life balloon.");
        if (other.gameObject.CompareTag("DartPoint"))
        {
            GetComponent<AudioSource>().Play();
            GetComponentInChildren<ParticleSystem>().Play();
            GetComponentInParent<Rigidbody>().useGravity = true;

            ShowScorePopup();
        }
    }

    void ShowScorePopup()
    {
        GameObject popup = Instantiate(scorePopupPrefab);
        Vector3 newVector = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
        popup.transform.position = newVector;
    }
}