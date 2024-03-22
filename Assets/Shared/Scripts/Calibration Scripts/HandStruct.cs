using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct HandStruct
{
    public HandStruct(OVRInput.Controller controller)
    {
        string side = "";
        if (controller == OVRInput.Controller.RTouch)
        {
            side = "Right";
        }
        else if (controller == OVRInput.Controller.LTouch)
        {
            side = "Left";
        }
        else
        {
            Debug.LogError("InvalidSetup: AffectedController must be 'L Touch' or 'R Touch'!");
            //Application.Quit(-1);
        }

        Grabber = GameObject.FindGameObjectWithTag(side + "Grabber");
        //Controller = GameObject.FindGameObjectWithTag(affectedSide + "HandController");
        Render = GameObject.Find("hand_" + side.ToLower() + "_renderPart_0");
        Debug.Log(" Renderer - hand_" + side.ToLower() + " is: " + Render.name);

        GrabberScript = Grabber.GetComponent<OVRGrabber>();
    }

    public GameObject Grabber { get; }
    //public GameObject Controller { get; }
    public GameObject Render { get; }
    public OVRGrabber GrabberScript { get; }
}
