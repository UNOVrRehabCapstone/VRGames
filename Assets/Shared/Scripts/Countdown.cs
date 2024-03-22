using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void CountdownDelegate ();

//I'm stuck on this, not working currently
public class Countdown : MonoBehaviour
{
    public float updateInterval = .1f;
    private float endTime;

    void Start()
    {
        //countDownAndExecute(10, print("hello"));
    }

    //Starts counting from 0
    public void countDownAndExecute(float duration, CountdownDelegate functionToExecute)
    {
        StopCoroutine( countDown( functionToExecute ) );
        endTime = duration + Time.realtimeSinceStartup;
        StartCoroutine( countDown( functionToExecute ) );
    }

    //Coroutine for incrementing timer over time
    private IEnumerator countDown( CountdownDelegate functionToExecute )
    {
        while ( Time.realtimeSinceStartup < endTime )
        {
            print( endTime - Time.realtimeSinceStartup );
            yield return new WaitForSeconds(updateInterval);
        }
        functionToExecute();
    }
}
