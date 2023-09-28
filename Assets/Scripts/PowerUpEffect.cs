using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpEffect : MonoBehaviour
{
    public float lifeSpan = 1;
    public float floatStrength = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        lifeSpan -= Time.deltaTime;
        if(lifeSpan <= 0)
        {
            Destroy(gameObject);
        }
        transform.position = Vector3.Lerp(transform.position, transform.position
                                                            + new Vector3(0f, 1f, 0f), Time.deltaTime * floatStrength);

    }
}
