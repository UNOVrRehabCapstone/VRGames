using System.Collections;
using System.Collections.Generic;
using Classes.Managers;
using UnityEngine;

public class Balloon_Onion : _BaseBalloon
{
    private void Update()
    {
        Transform transform = gameObject.transform;
        transform.position = Vector3.Lerp(transform.position, transform.position 
                                        + new Vector3(0f, 1f, 0f), Time.deltaTime * floatStrength);
    }
}
