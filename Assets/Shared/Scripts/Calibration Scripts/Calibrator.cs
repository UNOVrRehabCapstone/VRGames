using Classes.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Calibrator : MonoBehaviour
{

    public void ResetScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    public void ResetCalibration()
    {
        CalibrationManager.Instance?.Reset();
    }
    public void Calibrate(CalibrationManager.CalibrationType calibrationType, OVRInput.Controller unaffectedController)
    {
        CalibrationManager.Instance?.Calibrate(calibrationType, unaffectedController);
    }

    public void LHandMirrorCalibrate()
    {
        Calibrate(CalibrationManager.CalibrationType.Mirror, OVRInput.Controller.LTouch);
    }

    public void RHandMirrorCalibrate()
    {
        Calibrate(CalibrationManager.CalibrationType.Mirror, OVRInput.Controller.RTouch);
    }

    public void LHandScaleCalibrate()
    {
        Calibrate(CalibrationManager.CalibrationType.Scale, OVRInput.Controller.LTouch);
    }

    public void RHandScaleCalibrate()
    {
        Calibrate(CalibrationManager.CalibrationType.Scale, OVRInput.Controller.RTouch);
    }
}


