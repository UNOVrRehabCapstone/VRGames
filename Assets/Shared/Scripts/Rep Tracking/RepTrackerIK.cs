using Classes.Managers;
using Network;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepTrackerIK : MonoBehaviour
{
    private GameObject leftElbow;
    private GameObject rightElbow;

    public float retractedRot = 110;
    public float extendedRot = 60;

    bool lPastStart = false;
    bool rPastStart = false;

    int lRepCt = 0;
    int rRepCt = 0;

    Material lRender;
    Material rRender;

    public Color color_retract = new Color(.5f, .5f, 1f);
    public Color color_extend = new Color(1f, .5f, .5f);


    void Start()
    {
        leftElbow = GameObject.FindGameObjectWithTag("LeftElbow");
        rightElbow = GameObject.FindGameObjectWithTag("RightElbow");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // If we don't have our renderers, try to get them.
        if (lRender == null)
        {
            try
            {
                //lRender = GameObject.Find("hand_left_renderPart_0").GetComponent<Renderer>().material;
            }
            catch (NullReferenceException)
            {
                //Debug.LogError(e.Message);
            }
        }
        if (rRender == null)
        {
            try
            {
               // rRender = GameObject.Find("hand_right_renderPart_0").GetComponent<Renderer>().material;
            }
            catch (NullReferenceException)
            {
                //Debug.LogError(e.Message);
            }
        }

        //Debug.Log("Left: " + leftElbow.transform.localRotation.eulerAngles.y +
        //          "\nRight: " + rightElbow.transform.localRotation.eulerAngles.y);
        //Left arm min: 11.5, max = 140
        if (lRender != null && rRender != null)
        {
           // TrackLeftReps();
           // TrackRightReps();
        }
    }
    
    private void TrackLeftReps()
    {
        float lRot = leftElbow.transform.localRotation.eulerAngles.y;

        lRender.SetColor("_BaseColor", Color.white);
        if (lRot < extendedRot) //Arm is fully extended
        {
            lPastStart = true;
            lRender.SetColor("_BaseColor", color_extend);
        }
        else if (lRot > retractedRot) // Arm is retracted and was fully extended
        {
            lRender.SetColor("_BaseColor", color_retract);
            if (lPastStart) // Was fully extended?
            {
                lPastStart = false;
                Debug.Log("Left Rep " + ++lRepCt + " Counted!");
                NetworkManager.getManager().SendRepTrackingData("Left:" + lRepCt);
            }
        }
    }

    private void TrackRightReps()
    {
        float rRot = 360 - rightElbow.transform.localRotation.eulerAngles.y;

        rRender.SetColor("_BaseColor", Color.white);
        if (rRot < extendedRot) //Arm is fully extended
        {
            rPastStart = true;
            rRender.SetColor("_BaseColor", color_extend);
        }
        else if (rRot > retractedRot) // Arm is retracted
        {
            rRender.SetColor("_BaseColor", color_retract);
            if (rPastStart) // Was fully extended?
            {
                rPastStart = false;
                Debug.Log("Right Rep " + ++rRepCt + " Counted!");
                NetworkManager.getManager().SendRepTrackingData("Right:" + rRepCt);
            }
        }
    }
}
