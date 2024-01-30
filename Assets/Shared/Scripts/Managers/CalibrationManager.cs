using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

namespace Classes.Managers
{
    public class CalibrationManager: MonoBehaviour
    {
        [SerializeField]
        public HandStruct affectedHand;

        [SerializeField]
        public HandStruct nonAffectedHand;

        private TextMesh ScalingText;

        private float currentOffset;

        // Should be OVRInput.Controller.LTouch or OVRInput.Controller.RTouch.
        [SerializeField]
        public OVRInput.Controller affectedController;

        [SerializeField]
        public OVRInput.Controller naController;

        public CalibrationType calibrationType;

        public bool spawnSpheres = false;

        [SerializeField]
        private GameObject calibrationSphere;
        private GameObject[] calibtrationButtons;

        private Transform spawnPos;

        public static CalibrationManager Instance { get; private set; }

        private void Awake()
        {
            // If there is an instance, and it's not me, delete myself.

            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }

            spawnPos = GameObject.FindGameObjectWithTag("SpawnPoint").transform;

            calibtrationButtons = GameObject.FindGameObjectsWithTag("CalibrationButton");

        }

        public static CalibrationManager getManager() { return Instance; }

        public void Start()
        {
            // Spawn Calibration selection spheres for unaffected hand

            if (spawnSpheres)
            {
                GameObject tmp;
                //Right Sphere
                tmp = Instantiate(calibrationSphere, spawnPos.position + new Vector3(.30f, .85f, .54f), Quaternion.identity);
                tmp.GetComponent<CalibrationSphereSelector>().setHandSelection(OVRInput.Controller.RTouch);
            
                // Center Sphere
                //tmp = Instantiate(calibrationSphere, spawnPos.position + new Vector3(0f, .75f, .54f), Quaternion.identity);
                //tmp.GetComponent<CalibrationSelector>().setHandSelection(OVRInput.Controller.None);

                // Left Sphere
                tmp = Instantiate(calibrationSphere, spawnPos.position + new Vector3(-.30f, .85f, .54f), Quaternion.identity);
                tmp.GetComponent<CalibrationSphereSelector>().setHandSelection(OVRInput.Controller.LTouch);

            }
            
        }

        private void Update()
        {
            if (affectedController == OVRInput.Controller.None)
            {
                return;
            }
            else
            {
                UpdateAnchor();
            }
        }

        private void SetUnaffectedController(OVRInput.Controller unaffected)
        {
            // Apply the unaffected hand and skip if it's None
            if ((naController = unaffected) != OVRInput.Controller.None)
            {
                // Spawn calibration selection spheres for calibration type

                //GameObject tmp;
                //// Right Sphere - Scale
                //tmp = Instantiate(calibrationSphere, spawnPos.position + new Vector3(.26f, .85f, .54f), Quaternion.identity);
                //tmp.GetComponent<CalibrationSelector>().setCalibrationSelection(1);

                //// Left Sphere - Mirror
                //tmp = Instantiate(calibrationSphere, spawnPos.position + new Vector3(-.26f, .85f, .54f), Quaternion.identity);
                //tmp.GetComponent<CalibrationSelector>().setCalibrationSelection(2);
            }
        }

        private void SetCalibrationType(CalibrationType type)
        {
            // Decide Calibration type
            //mirrorHands = type == CalibrationType.Mirror;
            calibrationType = type;

            // Calibration settings are complete, calibrate controllers.
            CalibrateControllers();
        }

        private void CalibrateControllers()
        {
            //if (naController == OVRInput.Controller.None)
            if (calibrationType == CalibrationType.None)
            {
                return;
            }

            // Initialize the affected hand and non-affected hands 
            affectedController = OVRInput.Controller.RTouch;
            if (naController == OVRInput.Controller.RTouch)
            {
                affectedController = OVRInput.Controller.LTouch;
            }
            affectedHand = new HandStruct(affectedController);
            nonAffectedHand = new HandStruct(naController);

            // Specify that the affected hand is mirroring the other hand.
            if (calibrationType == CalibrationType.Mirror)
            {
                affectedHand.GrabberScript.IsMirroring(true);
                //Destroy(affectedHand.Render.GetComponent<OvrAvatarHand>());
            }

            if (GameObject.FindGameObjectWithTag("ScalingText") != null)
                ScalingText = GameObject.FindGameObjectWithTag("ScalingText").GetComponent<TextMesh>();

            // Scaling offset always starts at 1...
            currentOffset = 1.0f;
            UpdateScalingText();

            // Give the affected controller a reference to the non-affected one for mirroring purposes
            affectedHand.GrabberScript.SetOtherGrabber(nonAffectedHand.GrabberScript);
            nonAffectedHand.GrabberScript.SetOtherGrabber(affectedHand.GrabberScript);
        }

        private void UpdateAnchor()
        {
            // Create the value to change our anchor. First float is change factor. Next part decides positive change for stick up, negative change for stick down.
            // Use the non-afflicted controller
            float changeAmt = 0.2f * 
                ((OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickUp, naController) ? 1 : 0) - (OVRInput.GetDown(OVRInput.Button.PrimaryThumbstickDown, naController) ? 1 : 0));
            // Change the AnchorScaling by the calculated changeAmt. Limit value from 1 to 4.
            if ((changeAmt > 0 && currentOffset < 3.0f) || (changeAmt < 0 && currentOffset > 1.0f))
            {
                currentOffset = affectedHand.GrabberScript.ChangeAnchorScaleVector(new Vector3(changeAmt, 0.0f, changeAmt));
                UpdateScalingText();
            }
        }

        private void UpdateScalingText()
        {
            if (ScalingText != null)
                ScalingText.text = string.Format("Scaling: {0:#.0#}", currentOffset);
        }

        private void ResetCalibration()
        {
            affectedHand.GrabberScript?.ResetCalibration();

            nonAffectedHand.GrabberScript?.ResetCalibration();

            currentOffset = 1.0f;
            affectedController = OVRInput.Controller.None;
            naController = OVRInput.Controller.None;
            affectedHand = new HandStruct();
            nonAffectedHand = new HandStruct();
            calibrationType = CalibrationType.None;

        }

        public void SetCalBtnsActive(bool val)
        {
            // Get and hide calibration buttons
            foreach (GameObject btn in calibtrationButtons) btn.SetActive(val);
        }

        public void Reset()
        {
            ResetCalibration();
            SetCalBtnsActive(true);
        }

        public void Calibrate(CalibrationType calibrationType, OVRInput.Controller unaffectedController)
        {
            ResetCalibration();

            // Set the unaffected controller
            SetUnaffectedController(unaffectedController);

            // Set the type of calibration
            SetCalibrationType(calibrationType);

            SetCalBtnsActive(false);
        }

        public void changeScaleAmt(float scaleAmount)
        {

        }

        public enum CalibrationType
        {
            None,
            Mirror,
            Scale,
        }
    }
}