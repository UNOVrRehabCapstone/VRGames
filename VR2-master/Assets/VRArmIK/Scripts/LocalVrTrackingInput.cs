using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

namespace VRArmIK
{
	public class LocalVrTrackingInput : MonoBehaviour
	{
		public XRNode node;

		private InputDevice device;


        void Awake()
        {
			device = InputDevices.GetDeviceAtXRNode(node);

        }

        [System.Obsolete]
        void Update()
		{
            //if (device.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 position) && position.magnitude < 0.000001f)
            //	return;

            //device.TryGetFeatureValue(CommonUsages.deviceRotation, out Quaternion rotation);

            //transform.localPosition = position;
            //transform.localRotation = rotation;

            if (InputTracking.GetLocalPosition(node).magnitude < 0.000001f)
                return;

			transform.localPosition = InputTracking.GetLocalPosition(node);
			transform.localRotation = InputTracking.GetLocalRotation(node); 
		}
	}
}