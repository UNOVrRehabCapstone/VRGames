using Windows.Kinect;

namespace Classes
{
    public class JointJson
    {
        private JointType JointType;
        private CameraSpacePoint Position;
        private TrackingState TrackingState;
        private string timeStamp;

        public JointJson(Joint jnt)
        {
            JointType = jnt.JointType;
            Position = jnt.Position;
            TrackingState = jnt.TrackingState;
            timeStamp = System.DateTime.Now.ToString("u");
        }
    }
}