using System.Collections;
using System.Collections.Generic;
using Classes.Managers;
using UnityEngine;

public class Balloon_Onion : MonoBehaviour
{
    [SerializeField] private float floatStrength;

    private void Update()
    {
        Transform transform = gameObject.transform;

        if (SpecialBalloonManager.Instance.slowBalloonActivated) {
            /* Cut the float speed in half. */
            transform.position = Vector3.Lerp(transform.position, transform.position 
                                           + new Vector3(0f, 1f, 0f), Time.deltaTime * floatStrength * 0.5f);
        } else {
            transform.position = Vector3.Lerp(transform.position, transform.position 
                                           + new Vector3(0f, 1f, 0f), Time.deltaTime * floatStrength);
        }
    }
}
