/************************************************************************************
Copyright : Copyright (c) Facebook Technologies, LLC and its affiliates. All rights reserved.

Licensed under the Oculus Utilities SDK License Version 1.31 (the "License"); you may not use
the Utilities SDK except in compliance with the License, which is provided at the time of installation
or download, or which otherwise accompanies this software in either electronic or hard copy form.

You may obtain a copy of the License at
https://developer.oculus.com/licenses/utilities-1.31

Unless required by applicable law or agreed to in writing, the Utilities SDK distributed
under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF
ANY KIND, either express or implied. See the License for the specific language governing
permissions and limitations under the License.
************************************************************************************/

using System.Collections.Generic;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

/// <summary>
/// Allows grabbing and throwing of objects with the OVRGrabbable component on them.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class OVRGrabber : MonoBehaviour
{
    // Grip trigger thresholds for picking up objects, with some hysteresis.
    public float grabBegin = 0.55f;
    public float grabEnd = 0.35f;

    // Demonstrates parenting the held object to the hand's transform when grabbed.
    // When false, the grabbed object is moved every FixedUpdate using MovePosition.
    // Note that MovePosition is required for proper physics simulation. If you set this to true, you can
    // easily observe broken physics simulation by, for example, moving the bottom cube of a stacked
    // tower and noting a complete loss of friction.
    [SerializeField]
    protected bool m_parentHeldObject = false;

    // Child/attached transforms of the grabber, indicating where to snap held objects to (if you snap them).
    // Also used for ranking grab targets in case of multiple candidates.
    [SerializeField]
    protected Transform m_gripTransform = null;
    // Child/attached Colliders to detect candidate grabbable objects.
    [SerializeField]
    protected Collider[] m_grabVolumes = null;

    // Should be OVRInput.Controller.LTouch or OVRInput.Controller.RTouch.
    [SerializeField]
    protected OVRInput.Controller m_controller;
    protected OVRInput.Controller m_otherController;

    protected OVRGrabber m_otherGrabber = null;

    // True if this hand is the mirror image of the opposite hand. False otherwise.
    private bool isMirroring;

    protected Vector3 scaleVector;

    [SerializeField]
    protected Transform m_parentTransform;

    protected bool m_grabVolumeEnabled = true;
    protected Vector3 m_lastPos;
    protected Quaternion m_lastRot;
    protected Quaternion m_anchorOffsetRotation;
    protected Vector3 m_anchorOffsetPosition;
    protected float m_prevFlex;
    protected OVRGrabbable m_grabbedObj = null;
    protected Vector3 m_grabbedObjectPosOff;
    protected Quaternion m_grabbedObjectRotOff;
    protected Dictionary<OVRGrabbable, int> m_grabCandidates = new Dictionary<OVRGrabbable, int>();
    protected bool operatingWithoutOVRCameraRig = true;

    /// <summary>
    /// The currently grabbed object.
    /// </summary>
    public OVRGrabbable grabbedObject
    {
        get { return m_grabbedObj; }
    }

    // This is a custom method that allows us to set the scaling amount for hand movement
    public void SetAnchorScaleVector(Vector3 offset)
    {
        scaleVector = offset;
    }

    public Vector3 GetAnchorScaleVector()
    {
        return scaleVector;
    }

    public float ChangeAnchorScaleVector(Vector3 offset)
    {
        scaleVector += offset;
        return scaleVector.x;
    }

    public void IsMirroring(bool val)
    {
        isMirroring = val;
    }

    public void SetOtherGrabber(OVRGrabber grabber)
    {
        m_otherGrabber = grabber;
    }

	public void ForceRelease(OVRGrabbable grabbable)
    {
        bool canRelease = (
            (m_grabbedObj != null) &&
            (m_grabbedObj == grabbable)
        );
        if (canRelease)
        {
            GrabEnd();
        }
    }

    protected virtual void Awake()
    {
        m_anchorOffsetPosition = transform.localPosition;
        m_anchorOffsetRotation = transform.localRotation;

		// If we are being used with an OVRCameraRig, let it drive input updates, which may come from Update or FixedUpdate.

		OVRCameraRig rig = null;
		if (transform.parent != null && transform.parent.parent != null)
			rig = transform.parent.parent.GetComponent<OVRCameraRig>();

		if (rig != null)
		{
			rig.UpdatedAnchors += (r) => {OnUpdatedAnchors();};
			operatingWithoutOVRCameraRig = false;
		}
    }

    protected virtual void Start()
    {
        m_lastPos = transform.position;
        m_lastRot = transform.rotation;
        if(m_parentTransform == null)
        {
            if(gameObject.transform.parent != null)
            {
                m_parentTransform = gameObject.transform.parent.transform;
            }
            else
            {
                m_parentTransform = new GameObject().transform;
                m_parentTransform.position = Vector3.zero;
                m_parentTransform.rotation = Quaternion.identity;
            }
        }

        scaleVector = Vector3.one;

        // get the opposite controller
        m_otherController = m_controller == OVRInput.Controller.LTouch ? OVRInput.Controller.RTouch : OVRInput.Controller.LTouch;
    }

	void FixedUpdate()
	{
        if (operatingWithoutOVRCameraRig)
        {
            OnUpdatedAnchors();
        }	
	}

    // Hands follow the touch anchors by calling MovePosition each frame to reach the anchor.
    // This is done instead of parenting to achieve workable physics. If you don't require physics on
    // your hands or held objects, you may wish to switch to parenting.
    void OnUpdatedAnchors()
    {
        
        Vector3 handPos = OVRInput.GetLocalControllerPosition(m_controller);
        Quaternion handRot = OVRInput.GetLocalControllerRotation(m_controller);
        //Transform the grabber's position, scaling its moevement based on our offset vector
        Vector3 destPos = m_parentTransform.TransformPoint(m_anchorOffsetPosition + Vector3.Scale(handPos, scaleVector));
        Quaternion destRot = (m_parentTransform.rotation * handRot * m_anchorOffsetRotation).normalized;

        if (isMirroring)
        {
            // Move the affected hand mirrored on the X-axis to the non-affected hand
            destPos = Vector3.Scale(m_otherGrabber.transform.position, new Vector3(-1, 1, 1));

            // mirror its rotations on all axis except X.
            destRot = Quaternion.Euler(Vector3.Scale(m_otherGrabber.transform.localEulerAngles, new Vector3(1, -1, -1)));
        }

        GetComponent<Rigidbody>().MovePosition(destPos);
        GetComponent<Rigidbody>().MoveRotation(destRot);
        if (!m_parentHeldObject)
        {
            MoveGrabbedObject(destPos, destRot);
        }
        m_lastPos = transform.position;
        m_lastRot = transform.rotation;

		float prevFlex = m_prevFlex;

        if (!isMirroring)
        {
		    // Update values from inputs
		    m_prevFlex = OVRInput.Get(OVRInput.Axis1D.PrimaryHandTrigger, m_controller);
        }
        else
        {
            m_prevFlex = m_otherGrabber.m_prevFlex;
        }
        

		CheckForGrabOrRelease(prevFlex);        
    }

    void OnDestroy()
    {
        if (m_grabbedObj != null)
        {
            GrabEnd();
        }
    }

    void OnTriggerEnter(Collider otherCollider)
    {
        // Get the grab trigger
		OVRGrabbable grabbable = otherCollider.GetComponent<OVRGrabbable>() ?? otherCollider.GetComponentInParent<OVRGrabbable>();
        if (grabbable == null) return;

        // Add the grabbable
        int refCount = 0;
        m_grabCandidates.TryGetValue(grabbable, out refCount);
        m_grabCandidates[grabbable] = refCount + 1;
    }

    void OnTriggerExit(Collider otherCollider)
    {
		OVRGrabbable grabbable = otherCollider.GetComponent<OVRGrabbable>() ?? otherCollider.GetComponentInParent<OVRGrabbable>();
        if (grabbable == null) return;

        // Remove the grabbable
        int refCount = 0;
        bool found = m_grabCandidates.TryGetValue(grabbable, out refCount);
        if (!found)
        {
            return;
        }

        if (refCount > 1)
        {
            m_grabCandidates[grabbable] = refCount - 1;
        }
        else
        {
            m_grabCandidates.Remove(grabbable);
        }
    }

    protected void CheckForGrabOrRelease(float prevFlex)
    {
        if ((m_prevFlex >= grabBegin) && (prevFlex < grabBegin))
        {
            GrabBegin();
        }
        else if ((m_prevFlex <= grabEnd) && (prevFlex > grabEnd))
        {
            GrabEnd();
        }
    }

    protected virtual void GrabBegin()
    {
        float closestMagSq = float.MaxValue;
		OVRGrabbable closestGrabbable = null;
        Collider closestGrabbableCollider = null;

        // Iterate grab candidates and find the closest grabbable candidate
		foreach (OVRGrabbable grabbable in m_grabCandidates.Keys)
        {
            bool canGrab = !(grabbable.isGrabbed && !grabbable.allowOffhandGrab);
            if (!canGrab)
            {
                continue;
            }

            for (int j = 0; j < grabbable.grabPoints.Length; ++j)
            {
                Collider grabbableCollider = grabbable.grabPoints[j];
                // Store the closest grabbable
                Vector3 closestPointOnBounds = grabbableCollider.ClosestPointOnBounds(m_gripTransform.position);
                float grabbableMagSq = (m_gripTransform.position - closestPointOnBounds).sqrMagnitude;
                if (grabbableMagSq < closestMagSq)
                {
                    closestMagSq = grabbableMagSq;
                    closestGrabbable = grabbable;
                    closestGrabbableCollider = grabbableCollider;
                }
            }
        }

        // Disable grab volumes to prevent overlaps
        GrabVolumeEnable(false);

        if (closestGrabbable != null)
        {
            if (closestGrabbable.isGrabbed)
            {
                closestGrabbable.grabbedBy.OffhandGrabbed(closestGrabbable);
            }

            m_grabbedObj = closestGrabbable;
            m_grabbedObj.GrabBegin(this, closestGrabbableCollider);
            //send a grab message to the grabbed object's scripts
            m_grabbedObj.SendMessage("onGrab", gameObject , SendMessageOptions.DontRequireReceiver);

            m_lastPos = transform.position;
            m_lastRot = transform.rotation;

            // Set up offsets for grabbed object desired position relative to hand.
            if(m_grabbedObj.snapPosition)
            {
                if (m_grabbedObj.snapOffset)
                {
                    Vector3 snapOffset = -m_grabbedObj.snapOffset.localPosition;
                    Vector3 snapOffsetScale = m_grabbedObj.snapOffset.lossyScale;
                    snapOffset = new Vector3(snapOffset.x * snapOffsetScale.x, snapOffset.y * snapOffsetScale.y, snapOffset.z * snapOffsetScale.z);
                    if (m_controller == OVRInput.Controller.LTouch)
                    {
                        snapOffset.x = -snapOffset.x;
                    }
                    m_grabbedObjectPosOff = snapOffset;
                }
                else
                {
                    m_grabbedObjectPosOff = Vector3.zero;
                }
            }
            else
            {
                Vector3 relPos = m_grabbedObj.transform.position - transform.position;
                relPos = Quaternion.Inverse(transform.rotation) * relPos;
                m_grabbedObjectPosOff = relPos;
            }

            if (m_grabbedObj.snapOrientation)
            {
                if (m_grabbedObj.snapOffset)
                {
                    m_grabbedObjectRotOff = Quaternion.Inverse(m_grabbedObj.snapOffset.localRotation);
                }
                else
                {
                    m_grabbedObjectRotOff = Quaternion.identity;
                }
            }
            else
            {
                Quaternion relOri = Quaternion.Inverse(transform.rotation) * m_grabbedObj.transform.rotation;
                m_grabbedObjectRotOff = relOri;
            }

            // Note: force teleport on grab, to avoid high-speed travel to dest which hits a lot of other objects at high
            // speed and sends them flying. The grabbed object may still teleport inside of other objects, but fixing that
            // is beyond the scope of this demo.
            MoveGrabbedObject(m_lastPos, m_lastRot, true);
            if(m_parentHeldObject)
            {
                m_grabbedObj.transform.parent = transform;
            }
        }
    }

    protected virtual void MoveGrabbedObject(Vector3 pos, Quaternion rot, bool forceTeleport = false)
    {
        if (m_grabbedObj == null)
        {
            return;
        }

        Rigidbody grabbedRigidbody = m_grabbedObj.grabbedRigidbody;
        Vector3 grabbablePosition = pos + rot * m_grabbedObjectPosOff;
        Quaternion grabbableRotation = rot * m_grabbedObjectRotOff;

        if (forceTeleport)
        {
            grabbedRigidbody.transform.position = grabbablePosition;
            grabbedRigidbody.transform.rotation = grabbableRotation;
        }
        else
        {
            grabbedRigidbody.MovePosition(grabbablePosition);
            grabbedRigidbody.MoveRotation(grabbableRotation);
        }
    }

    protected void GrabEnd()
    {
        if (m_grabbedObj != null)
        {
            OVRPose localPose = new OVRPose { position = OVRInput.GetLocalControllerPosition(isMirroring ? m_otherController: m_controller), 
                                                orientation = OVRInput.GetLocalControllerRotation(isMirroring? m_otherController : m_controller) };
            OVRPose offsetPose = new OVRPose { position = m_anchorOffsetPosition, orientation = m_anchorOffsetRotation };

            localPose = localPose * offsetPose;

			OVRPose trackingSpace = (isMirroring ? m_otherGrabber.transform.ToOVRPose() : transform.ToOVRPose()) * localPose.Inverse();

            Vector3 linearVelocity;
            Vector3 angularVelocity;

            if (isMirroring)
            {
                // If we're mirroring, we need to use the velocity of the other controller and mirror it on the x axis.
                linearVelocity = trackingSpace.orientation * Vector3.Scale(OVRInput.GetLocalControllerVelocity(m_otherController), new Vector3(-1, 1, 1));
                angularVelocity = trackingSpace.orientation * Vector3.Scale(OVRInput.GetLocalControllerAngularVelocity(m_otherController), new Vector3(-1, 1, 1));
            }
            else
            {
                // If we're scaling, scale the controller's velocity by our scale factor. Angular velociy remains the same
                linearVelocity = trackingSpace.orientation * Vector3.Scale(OVRInput.GetLocalControllerVelocity(m_controller), scaleVector);
                angularVelocity = trackingSpace.orientation * OVRInput.GetLocalControllerAngularVelocity(m_controller);

            }


            GrabbableRelease(linearVelocity, angularVelocity);
        }

        // Re-enable grab volumes to allow overlap events
        GrabVolumeEnable(true);
    }

    protected void GrabbableRelease(Vector3 linearVelocity, Vector3 angularVelocity)
    {
        m_grabbedObj.GrabEnd(linearVelocity, angularVelocity);
        if (!m_grabbedObj.isDestroying)
        {
            if (m_parentHeldObject) m_grabbedObj.transform.parent = null;
            //send a release message to the grabbed object
            m_grabbedObj.SendMessage("onRelease", gameObject, SendMessageOptions.DontRequireReceiver);
        }
        m_grabbedObj = null;
    }

    protected virtual void GrabVolumeEnable(bool enabled)
    {
        if (m_grabVolumeEnabled == enabled)
        {
            return;
        }

        m_grabVolumeEnabled = enabled;
        for (int i = 0; i < m_grabVolumes.Length; ++i)
        {
            Collider grabVolume = m_grabVolumes[i];
            grabVolume.enabled = m_grabVolumeEnabled;
        }

        if (!m_grabVolumeEnabled)
        {
            m_grabCandidates.Clear();
        }
    }

	protected virtual void OffhandGrabbed(OVRGrabbable grabbable)
    {
        if (m_grabbedObj == grabbable)
        {
            GrabbableRelease(Vector3.zero, Vector3.zero);
        }
    }

    public void ResetCalibration()
    {
        // Should be OVRInput.Controller.LTouch or OVRInput.Controller.RTouch.
        //m_controller = OVRInput.Controller.None;
        m_otherController = OVRInput.Controller.None;

        m_otherGrabber = null;

        isMirroring = false;

        //scaleVector = Vector3.one;
}
}
