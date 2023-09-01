using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Responsible for calling functions related to timers.
//Useful for games where time is used for scoring instead of points.
public class TimerManager : MonoBehaviour
{
    
    public float timerUpdateInterval = .1f;
    private static GameObject scoreboardTimer;
    private float timerStartTime;
    private float timerEndTime;
    private float timer;

    void Start()
    {
        scoreboardTimer = GameObject.FindGameObjectWithTag("TimerText");
    }
    //Starts counting from current value of timer
    public void resumeTimer()
    {
        timerStartTime = -timer;
        StartCoroutine(countUp());
    }

    //Stops counting
    public void pauseTimer() { StopCoroutine(countUp()); }

    //Starts counting from 0
    public void startTimer()
    {
        stopTimer();
        timerStartTime = Time.realtimeSinceStartup;
        resumeTimer();
    }

    //Stops counting and sets timer to 0
    public void stopTimer()
    {
        pauseTimer();
        timer = 0;
    }

    //Coroutine for incrementing timer over time
    private IEnumerator countUp()
    {
        while (true)
        {
            timer = Time.realtimeSinceStartup - timerStartTime;
            print(timer);
            yield return new WaitForSeconds(timerUpdateInterval);
        }
    }

    private IEnumerator countDown(float time)
    {
        while (true)
        {
            scoreboardTimer.GetComponentInChildren<TextMesh>().text = time.ToString();
            yield return new WaitForSeconds(.1f);
        }
    }

    public void startCountdown(float time)
    {
        stopTimer();
        StartCoroutine(countDown(time));
    }
}
