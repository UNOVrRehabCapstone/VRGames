using Classes.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using Network;
using UnityEngine;

public class RepTracker : MonoBehaviour
{
    private Transform leftHand;
    private Transform rightHand;
    private Transform head;

    public float retractedDist = 0.23f;
    public float extendedDist = 0.55f;

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
        leftHand = GameObject.FindGameObjectWithTag("LeftGrabber").transform;
        rightHand = GameObject.FindGameObjectWithTag("RightGrabber").transform;
        head = Camera.main.transform;
        Debug.Log("Rep");
    }

    void FixedUpdate()
    {
        if (lRender == null)
        {
            try
            {
                lRender = GameObject.Find("hand_left_renderPart_0").GetComponent<Renderer>().material;
            }
            catch (NullReferenceException)
            {

            }
        }
        if (rRender == null)
        {
            try
            {
                rRender = GameObject.Find("hand_right_renderPart_0").GetComponent<Renderer>().material;
            }
            catch (NullReferenceException)
            {

            }
        }

      //  if (lRender != null)
           // TrackLeftReps();

       // if (rRender != null)
          //  TrackRightReps();
    }

    private void TrackLeftReps()
    {
        float dist = Vector3.Distance(head.position, leftHand.position);
        //float dist = Vector2.Distance(new Vector2(head.position.x, head.position.z), new Vector2(leftHand.position.x, leftHand.position.z));
        //Debug.Log("LeftHand Dist: " + dist);

        lRender.SetColor("_BaseColor", Color.white);
        if (dist > extendedDist) //Arm is fully extended
        {
            lPastStart = true;
            lRender.SetColor("_BaseColor", color_extend);
        }
        else if (dist < retractedDist) // Arm is retracted
        {
            lRender.SetColor("_BaseColor", color_retract);
            if (lPastStart) // Was fully extended?
            {
                lPastStart = false;
                Debug.Log("Left Rep " + ++lRepCt + " Counted!");
                NetworkManager.getManager().SendRepTrackingData("Left:" + lRepCt);
            }
        }
        else
        {
            rRender.SetColor("_BaseColor", Color.white);
        }
    }

    private void TrackRightReps()
    {
        float dist = Vector3.Distance(head.position, rightHand.position);
        //Debug.Log("LeftHand Dist: " + dist);

        if (dist > extendedDist) //Arm is fully extended
        {
            rPastStart = true;
            rRender.SetColor("_BaseColor", color_extend);
        }
        else if (dist < retractedDist) // Arm is retracted
        {
            rRender.SetColor("_BaseColor", color_retract);
            if (rPastStart) // Was fully extended?
            {
                rPastStart = false;
                Debug.Log("Right Rep " + ++rRepCt + " Counted!");
                NetworkManager.getManager().SendRepTrackingData("Right:" + rRepCt);
            }
        }
        else
        {
            rRender.SetColor("_BaseColor", Color.white);
        }
    }
}
