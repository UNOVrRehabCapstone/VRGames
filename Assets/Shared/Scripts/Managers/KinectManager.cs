using UnityEngine;
using Windows.Kinect;
using System.Linq;


namespace Classes.Managers
{
    public class KinectManager : MonoBehaviour
    {
        private KinectSensor _sensor;
        private BodyFrameReader _bodyFramereader;
        private Body[] _bodies = null;

        public Camera mainCamera;
        public OVRCameraRig ovrcamera;

        public static KinectManager instance = null;

        private GameObject rightHand;
        private GameObject leftHand;
        private GameObject server;

        //public OvrAvatarRightHand oculusRightHand;
        //public OvrAvatarLeftHand oculusLeftHand;

        public Body[] GetBodies()
        {
            return _bodies;
        }
        
	    // Use this for initialization
	    void Awake () {
            if (instance == null)
            { 
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }
        }
	    
	    void Start () {
            _sensor = KinectSensor.GetDefault();
            if (_sensor != null)
            {
                _bodyFramereader = _sensor.BodyFrameSource.OpenReader();
                if (!_sensor.IsOpen)
                {
                    _sensor.Open();
                }
                _bodies = new Body[_sensor.BodyFrameSource.BodyCount];
            }
            rightHand = GameObject.FindGameObjectWithTag("RightGrabber");
            leftHand = GameObject.FindGameObjectWithTag("LeftGrabber");
            server = GameObject.FindGameObjectWithTag("Server");
        }

        void Update()
        {
            
            if (_bodyFramereader != null)
            {
                var frame = _bodyFramereader.AcquireLatestFrame();
                if (frame != null)
                {
                    frame.GetAndRefreshBodyData(_bodies);
                    
                    foreach (var body in _bodies.Where(b => b.IsTracked))
                    {
                        
                        Windows.Kinect.Joint head = body.Joints[JointType.Head];
                        // total local Y position of oculus headset relative to ovrplayer object
                        float oculusCameraY = mainCamera.transform.localPosition.y + ovrcamera.transform.localPosition.y;

                        // total local Z position of oculus headset relative to ovrplayer object
                        float oculusCameraZ = mainCamera.transform.localPosition.z + ovrcamera.transform.localPosition.z;

                        // finding the Y position difference between the kinect head position and the headset position
                        float oculusAndKinectDiffY = head.Position.Y - oculusCameraY;

                        Windows.Kinect.Joint pelvis = body.Joints[JointType.SpineBase];
                        if (body.HandRightConfidence == TrackingConfidence.Low)
                        {
                            Windows.Kinect.Joint kinectHandRight = body.Joints[JointType.HandRight];
                            Windows.Kinect.Joint kinectElbowRight = body.Joints[JointType.ElbowRight];

                            // offset kinect hand position with calculated difference between kinect head tracking and oculus headset position
                            rightHand.transform.localPosition = new Vector3(kinectHandRight.Position.X, kinectHandRight.Position.Y - oculusAndKinectDiffY, (head.Position.Z - kinectHandRight.Position.Z) + oculusCameraZ);

                            // offset kinect hand Z rotation relative to X distance away from body
                            Vector3 dist_from_pelvis_X = new Vector3(pelvis.Position.X - kinectHandRight.Position.X, 0,0);
                            if (dist_from_pelvis_X != Vector3.zero)
                            {
                                Quaternion YRotation = Quaternion.LookRotation(new Vector3(-(kinectElbowRight.Position.X - kinectHandRight.Position.X),
                                    -(kinectElbowRight.Position.Y - kinectHandRight.Position.Y), kinectElbowRight.Position.Z - kinectHandRight.Position.Z));
                                YRotation.z += Mathf.Abs(dist_from_pelvis_X.x);
                                rightHand.transform.localRotation = YRotation;
                            }

                            /*
                            if (body.HandRightState == HandState.Open)
                            {
                                
                            }
                            else if (body.HandRightState == HandState.Closed)
                            {
                            }
                            */
                            
                            
                            //server.GetComponent<server>().convertKinectJointToJson(kinectHandRight);
                        }
                        
                        if (body.HandLeftConfidence == TrackingConfidence.Low)
                        {
                            Windows.Kinect.Joint kinectHandLeft = body.Joints[JointType.HandLeft];
                            Windows.Kinect.Joint kinectElbowLeft = body.Joints[JointType.ElbowLeft];
                            // offset kinect hand position with calculated difference between kinect head tracking and oculus headset position
                            leftHand.transform.localPosition = new Vector3(kinectHandLeft.Position.X, kinectHandLeft.Position.Y - oculusAndKinectDiffY, (head.Position.Z - kinectHandLeft.Position.Z) + oculusCameraZ);
                            
                            // offset kinect hand Z rotation relative to X distance away from body
                            Vector3 dist_from_pelvis_X = new Vector3(pelvis.Position.X - kinectHandLeft.Position.X, 0, 0);
                            if (dist_from_pelvis_X != Vector3.zero)
                            {
                                Quaternion YRotation = Quaternion.LookRotation(new Vector3(-(kinectElbowLeft.Position.X - kinectHandLeft.Position.X),
                                    -(kinectElbowLeft.Position.Y - kinectHandLeft.Position.Y), kinectElbowLeft.Position.Z - kinectHandLeft.Position.Z));
                                YRotation.z -= Mathf.Abs(dist_from_pelvis_X.x);
                                leftHand.transform.localRotation = YRotation;
                            }
                           

                            /*
                            if (body.HandLeftState == HandState.Open)
                            {
                            }
                            else if (body.HandLeftState == HandState.Closed)
                            {
                            }
                            */
                            //server.GetComponent<server>().convertKinectJointToJson(kinectHandLeft);
                        }

                        foreach (var jnt in body.Joints.Values)
                        {
                            server.GetComponent<server>().convertKinectJointToJson(jnt);
                        }
                    }
                    frame.Dispose();
                    frame = null;
                }
            }
        }

        public Vector3 GetRightKinectHandPosition()
        {
            return rightHand.transform.localPosition;
        }

        public Vector3 GetLeftKinectHandPosition()
        {
            return leftHand.transform.localPosition;
        }

        public Quaternion GetRightKinectHandRotation()
        {
            return rightHand.transform.localRotation;
        }

        public Quaternion GetLeftKinectHandRotation()
        {
            return leftHand.transform.localRotation;
        }

        void OnApplicationQuit()
        {
            if (_bodyFramereader != null)
            {
                _bodyFramereader.IsPaused = true;
                _bodyFramereader.Dispose();
                _bodyFramereader = null;
            }
            if (_sensor != null)
            {
                if (_sensor.IsOpen)
                {
                    _sensor.Close();
                }
                _sensor = null;
            }
        }
    }
}