using Classes.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// This script is depracted. Avoid using it! use the Calibrator or Calibration Manager instead!
/// </summary>
public class CalibrationSphereSelector : MonoBehaviour
{
    [SerializeField] private Material m_baseMat;
    [SerializeField] private Material m_hoverMat;

    [SerializeField]
    public OVRInput.Controller hand;

    [SerializeField]
    public int calibrationType;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setHandSelection(OVRInput.Controller controller)
    {
        hand = controller;
        GetComponentInChildren<TextMesh>().text = hand.ToString() + "\nhand";
    }

    public void setCalibrationSelection(int type)
    {
        // 1: Hand Scaling
        // 2: Hand Mirroring
        calibrationType = type;
        GetComponentInChildren<TextMesh>().text = (calibrationType == 1 ? "Scale" : "Mirror");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("Grab"))
        {
            gameObject.GetComponent<Renderer>().material = m_hoverMat;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Check if hand is grabbing this sphere
        if ( (other.tag == "LeftGrabber" && OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch)) || (other.tag == "RightGrabber" && OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch)))
        {
            ButtonCalibrate();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name.Contains("Grab"))
        {
            gameObject.GetComponent<Renderer>().material = m_baseMat;

        }
    }

    public void ButtonCalibrate()
    {
        CalibrationManager.Instance.Reset();

        // Set the unaffected controller
        //CalibrationManager.Instance.SetUnaffectedController(hand);

        // Set the type of calibration
        //CalibrationManager.Instance.SetCalibrationType(CalibrationManager.CalibrationType.Mirror);

        CalibrationManager.Instance.SetCalBtnsActive(false);
    }

}
