using System;
using System.Collections;
using Classes.Managers;
using UnityEngine;

public class Balloon : MonoBehaviour
{
    public float floatStrength;
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
        if (other.gameObject.CompareTag("DartPoint"))
        {
            GetComponent<AudioSource>().Play();
            GetComponentInChildren<ParticleSystem>().Play();
            GetComponentInParent<Rigidbody>().useGravity = true;
        }
    }
}