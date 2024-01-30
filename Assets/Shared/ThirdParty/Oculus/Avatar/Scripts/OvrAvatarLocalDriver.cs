using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using Oculus.Avatar;
using Classes.Managers;

public class OvrAvatarLocalDriver : OvrAvatarDriver {

    Vector3 centerEyePosition = Vector3.zero;
    Quaternion centerEyeRotation = Quaternion.identity;

    ControllerPose GetMalibuControllerPose(OVRInput.Controller controller)
    {
        ovrAvatarButton buttons = 0;
        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, controller)) buttons |= ovrAvatarButton.One;

        return new ControllerPose
        {
            buttons = buttons,
            touches = OVRInput.Get(OVRInput.Touch.PrimaryTouchpad) ? ovrAvatarTouch.One : 0,
            joystickPosition = OVRInput.Get(OVRInput.Axis2D.PrimaryTouchpad, controller),
            indexTrigger = 0f,
            handTrigger = 0f,
            isActive = (OVRInput.GetActiveController() & controller) != 0,
        };
    }

    float voiceAmplitude = 0.0f;
    ControllerPose GetControllerPose(OVRInput.Controller controller)
    {
        ovrAvatarButton buttons = 0;
        if (OVRInput.Get(OVRInput.Button.One, controller)) buttons |= ovrAvatarButton.One;
        if (OVRInput.Get(OVRInput.Button.Two, controller)) buttons |= ovrAvatarButton.Two;
        if (OVRInput.Get(OVRInput.Button.Start, controller)) buttons |= ovrAvatarButton.Three;
        if (OVRInput.Get(OVRInput.Button.PrimaryThumbstick, controller)) buttons |= ovrAvatarButton.Joystick;

        ovrAvatarTouch touches = 0;
        if (OVRInput.Get(OVRInput.Touch.One, controller)) touches |= ovrAvatarTouch.One;
        if (OVRInput.Get(OVRInput.Touch.Two, controller)) touches |= ovrAvatarTouch.Two;
        if (OVRInput.Get(OVRInput.Touch.PrimaryThumbstick, controller)) touches |= ovrAvatarTouch.Joystick;
        if (OVRInput.Get(OVRInput.Touch.PrimaryThumbRest, controller)) touches |= ovrAvatarTouch.ThumbRest;
        if (OVRInput.Get(OVRInput.Touch.PrimaryIndexTrigger, controller)) touches |= ovrAvatarTouch.Index;
        if (!OVRInput.Get(OVRInput.NearTouch.PrimaryIndexTrigger, controller)) touches |= ovrAvatarTouch.Pointing;
        if (!OVRInput.Get(OVRInput.NearTouch.PrimaryThumbButtons, controller)) touches |= ovrAvatarTouch.ThumbUp;

        return new ControllerPose
        {
            buttons = buttons,
            touches = touches,
            joystickPosition = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, controller),
            indexTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryIndexTrigger, controller),
            handTrigger = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, controller),
            isActive = (OVRInput.GetActiveController() & controller) != 0,
        };
    }

    private void CalculateCurrentPose()
    {
        // Platform and device agnostic calls to return center eye pose, used to pass in head pose to sdk
        OVRNodeStateProperties.GetNodeStatePropertyVector3(UnityEngine.XR.XRNode.CenterEye, NodeStatePropertyType.Position,
            OVRPlugin.Node.EyeCenter, OVRPlugin.Step.Render, out centerEyePosition);
        OVRNodeStateProperties.GetNodeStatePropertyQuaternion(UnityEngine.XR.XRNode.CenterEye, NodeStatePropertyType.Orientation,
            OVRPlugin.Node.EyeCenter, OVRPlugin.Step.Render, out centerEyeRotation);

        Vector3 hlPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch),
                hrPos = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);

        Quaternion hlRot = OVRInput.GetLocalControllerRotation(OVRInput.Controller.LTouch),
                   hrRot = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);

        ControllerPose clPose = GetControllerPose(OVRInput.Controller.LTouch),
                       crPose = GetControllerPose(OVRInput.Controller.RTouch);

        if (CalibrationManager.Instance != null && CalibrationManager.Instance.calibrationType == CalibrationManager.CalibrationType.Mirror)
        {
            if (CalibrationManager.Instance.affectedController == OVRInput.Controller.LTouch)
            {
                hlPos = CalibrationManager.Instance.affectedHand.Grabber.transform.localPosition;
                hlRot = CalibrationManager.Instance.affectedHand.Grabber.transform.localRotation;
                clPose = GetControllerPose(OVRInput.Controller.RTouch);
            }
            else
            {
                hrPos = CalibrationManager.Instance.affectedHand.Grabber.transform.localPosition;
                hrRot = CalibrationManager.Instance.affectedHand.Grabber.transform.localRotation;
                crPose = GetControllerPose(OVRInput.Controller.LTouch);
            }
        }
        else if (CalibrationManager.Instance != null)
        {
            if (CalibrationManager.Instance.affectedController == OVRInput.Controller.LTouch)
            {
                hlPos = Vector3.Scale(OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch), CalibrationManager.Instance.affectedHand.GrabberScript.GetAnchorScaleVector());
            }
            else if (CalibrationManager.Instance.affectedController == OVRInput.Controller.RTouch)
            {
                hrPos = Vector3.Scale(OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch), CalibrationManager.Instance.affectedHand.GrabberScript.GetAnchorScaleVector());
            }
        }

        CurrentPose = new PoseFrame
        {
            voiceAmplitude = voiceAmplitude,
            headPosition = centerEyePosition,
            headRotation = centerEyeRotation,

            handLeftPosition = hlPos,
            handLeftRotation = hlRot,
            handRightPosition = hrPos,
            handRightRotation = hrRot,

            controllerLeftPose = clPose,
            controllerRightPose = crPose,

        };
    }

    public override void UpdateTransforms(IntPtr sdkAvatar)
    {
        CalculateCurrentPose();
        UpdateTransformsFromPose(sdkAvatar);
    }
}
