using System;
using System.Collections;
using Firesplash.UnityAssets.SocketIO;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Classes.Managers;
using System.Globalization;


namespace Network
{
    //[RequireComponent(typeof(SocketIOCommunicator))]
    public class NetworkManager : MonoBehaviour
    {   
        public static NetworkManager Instance { get; private set; }
        private string clinicianId;

        private SocketIOInstance _socket;
        private GameObject       socket;

        [SerializeField] private TMP_InputField patientNameInput;
        [SerializeField] private TextMeshProUGUI patientName;
        [SerializeField] private TextMeshProUGUI clinicianName;
        [SerializeField] private GameObject setName;
        [SerializeField] private TextMeshProUGUI connText;
        [SerializeField] private GameObject keyboard;
        [SerializeField] private GameObject socketPrefab;


        private int counter = 0;
        private void Awake()
        { 
            if (Instance != null && Instance != this) {
                Destroy(this);
            } else {
                DontDestroyOnLoad(this);
                socket  = Instantiate(socketPrefab);
                DontDestroyOnLoad(socket);
                _socket = socket.GetComponent<SocketIOCommunicator>().Instance;

                Instance = this;
            }
        }

        private void Start()
        {
            _socket.Connect();
            HandleServerEvents();
            StartCoroutine(PatientConnect());
            SetPatientName("Patient");
        }

        public static NetworkManager getManager() { return Instance; }

        private IEnumerator PatientConnect()
        {
            yield return new WaitUntil(() => _socket.IsConnected());
            FetchName();
        }

        private IEnumerator FlagBalloonStartAfterDelay()
        {
            yield return new WaitForSeconds(1);
            SocketClasses.BalloonGameSettingsValues.gameIsRunning = true;
            SocketClasses.BalloonGameSettingsValues.balloonStart = true;


        }

        private void FetchName()
        {
            if (PlayerPrefs.HasKey("PatientName"))
            {
                var name = PlayerPrefs.GetString("PatientName");
                Debug.Log("Has Name: " + name);
                _socket.Emit("unityConnect", name, true);
                patientName.SetText(name);
                SwitchToViewName();
            }
            else
            {
                SwitchToSetName();
            }
        }
        
        private void HandleServerEvents()
        {
             _socket.On("userJoined", (string payload) =>
            {
                Debug.Log("Joined Room!" + payload);
                var obj = JsonConvert.DeserializeObject<SocketClasses.JoinRoom>(payload);
                Debug.Log(obj.clinician);
                clinicianName.SetText(obj.clinician);
                connText.gameObject.SetActive(true);
            });
             _socket.On("startGame", (string payload) =>
             {
                 Debug.Log("Started Game");
                 var obj = JsonConvert.DeserializeObject<SocketClasses.StartGame>(payload);
                 SocketClasses.BalloonGameSettingsValues.environment = obj.environment;
                 switch(obj.game) {
                    case "3":
                        SceneManager.LoadScene("Planes");
                        break;
                    case "2":
                         SocketClasses.BalloonGameSettingsValues.clinicianIsControlling = true;
                         SocketClasses.BalloonGameSettingsValues.gameIsRunning = false;
                         SocketClasses.BalloonGameSettingsValues.balloonStart = false;
                         SceneManager.LoadScene("Balloons");

                         break;
                    case "1":
                        SceneManager.LoadScene("Blocks");
                        break;
                    default:
                        SceneManager.LoadScene("Initialize");
                        break;
                 }

                 
             });

            _socket.On("test", (string payload) => {
                Debug.Log("Recieved Test!Recieved Test!Recieved Test!Recieved Test!Recieved Test!Recieved Test!");
            });
            _socket.On("balloonSettings", (string payload) =>
            {
                Debug.Log(payload);
                var obj = JsonConvert.DeserializeObject<SocketClasses.BalloonGameSettings>(payload);
                SocketClasses.BalloonGameSettingsValues.balloonGameMode = obj.mode;
                SocketClasses.BalloonGameSettingsValues.balloonGameGoal = obj.target;

                string frequency;
                switch (obj.freq)
                {
                    case "None":
                        frequency = "0";
                        break;
                    case "Low":
                        frequency = "15";
                        break;
                    case "Medium":
                        frequency = "20";
                        break;
                    case "High":
                        frequency = "40";
                        break;
                    default:
                        frequency = "15";
                        break;
                }
                SocketClasses.BalloonGameSettingsValues.balloonGameSpecialBalloonFrequency = frequency;
                SocketClasses.BalloonGameSettingsValues.balloonGameLeftRightRatio = obj.ratio;
                SocketClasses.BalloonGameSettingsValues.balloonGamePattern = obj.pattern;
                if(obj.mode == "0")
                {
                    SocketClasses.BalloonGameSettingsValues.balloonGameMaxLives = "3";
                }
                else {

                    SocketClasses.BalloonGameSettingsValues.balloonGameMaxLives = obj.lives;
                }
                
                SocketClasses.BalloonGameSettingsValues.balloonGameHandSetting = obj.hand;
                Debug.Log("Setting mode to play");
                //SocketClasses.BalloonGameSettingsValues.environment = obj.environment;
                SocketClasses.BalloonGameSettingsValues.speedModifier = obj.modifier;
                SocketClasses.BalloonGameSettingsValues.numBalloonsSpawnedAtOnce = obj.numBalloonsSpawnedAtOnce;
                SocketClasses.BalloonGameSettingsValues.spawnTime = obj.timeBetweenSpawns;

                SocketClasses.BalloonGameSettingsValues.careerModeLevelToPlay = obj.careerModeLevelToPlay;

                if(SocketClasses.BalloonGameSettingsValues.gameIsRunning == true)
                {
                    BalloonGameplayManager.Instance.RefreshSettings();

                }
                else
                {
                    StartCoroutine(FlagBalloonStartAfterDelay());
                }

            });

            _socket.On("balloonSpawn", (string payload) => {
                // Find the BalloonManager GameObject in the current scene
                GameObject balloonManagerObject = GameObject.Find("BalloonManager");

                // Check if the BalloonManager GameObject is found
                if (balloonManagerObject != null)
                {
                    // Try to get the BalloonManager script attached to the BalloonManager GameObject
                    BalloonManager balloonManagerScript = balloonManagerObject.GetComponent<BalloonManager>();

                    // Check if the script is found
                    if (balloonManagerScript != null)
                    {
                        // Call the PickSpawn() method in the BalloonManager script
                            balloonManagerScript.ManualSpawn(BalloonGameplayManager.Instance.gameSettings.spawnPattern, BalloonGameplayManager.Instance.gameSettings.balloonPrefabs[0]);
                    }
                    else
                    {
                        Debug.LogError("BalloonManager script not found on BalloonManager GameObject.");
                    }
                }
                else
                {
                    Debug.LogError("BalloonManager GameObject not found in the scene.");
                }
            });

            _socket.On("balloonData", (string payload) =>
            {
                Debug.Log("Recieving BalloonData on interation: " + counter);
                Debug.Log(payload);
                counter++;
                var obj = JsonConvert.DeserializeObject<SocketClasses.BalloonGameData>(payload);
                Debug.Log(obj.ach9);
                if (obj.userName != null)
                {
                    SocketClasses.BalloonGameDataValues.userName = obj.userName;
                    //SetPatientName(obj.userName);
                    //patientName.SetText(obj.userName);
                }
                SocketClasses.BalloonGameDataValues.levelOneScore = obj.levelOneScore;
                SocketClasses.BalloonGameDataValues.levelTwoScore = obj.levelTwoScore;
                SocketClasses.BalloonGameDataValues.levelThreeScore = obj.levelThreeScore;
                SocketClasses.BalloonGameDataValues.levelFourScore = obj.levelFourScore;
                SocketClasses.BalloonGameDataValues.levelFiveScore = obj.levelFiveScore;
                Debug.Log(patientName.text);
                if (obj.ach0 == true)
                {
                    SocketClasses.Achievements.AllAchievements[0].isAchieved = true;
                }
                else
                {
                    SocketClasses.Achievements.AllAchievements[0].isAchieved = false;
                }
                
                if (obj.ach1 == true)
                {
                    SocketClasses.Achievements.AllAchievements[1].isAchieved = true;
                }
                else
                {
                    SocketClasses.Achievements.AllAchievements[1].isAchieved = false;
                }

                if (obj.ach2 == true)
                {
                    SocketClasses.Achievements.AllAchievements[2].isAchieved = true;
                }
                else
                {
                    SocketClasses.Achievements.AllAchievements[2].isAchieved = false;
                }

                if (obj.ach3 == true)
                {
                    SocketClasses.Achievements.AllAchievements[3].isAchieved = true;
                }
                else
                {
                    SocketClasses.Achievements.AllAchievements[3].isAchieved = false;
                }

                if (obj.ach4 == true)
                {
                    SocketClasses.Achievements.AllAchievements[4].isAchieved = true;
                }
                else
                {
                    SocketClasses.Achievements.AllAchievements[4].isAchieved = false;
                }


                if (obj.ach5 == true)
                {
                    SocketClasses.Achievements.AllAchievements[5].isAchieved = true;
                }
                else
                {
                    SocketClasses.Achievements.AllAchievements[5].isAchieved = false;
                }


                if (obj.ach6 == true)
                {
                    SocketClasses.Achievements.AllAchievements[6].isAchieved = true;
                }
                else
                {
                    SocketClasses.Achievements.AllAchievements[6].isAchieved = false;
                }

                if (obj.ach7 == true)
                {
                    SocketClasses.Achievements.AllAchievements[7].isAchieved = true;
                }
                else
                {
                    SocketClasses.Achievements.AllAchievements[7].isAchieved = false;
                }


                if (obj.ach8 == true)
                {
                    SocketClasses.Achievements.AllAchievements[8].isAchieved = true;
                }
                else
                {
                    SocketClasses.Achievements.AllAchievements[8].isAchieved = false;
                }

                if (obj.ach9 == true)
                {
                    SocketClasses.Achievements.AllAchievements[9].isAchieved = true;
                }
                else
                {
                    SocketClasses.Achievements.AllAchievements[9].isAchieved = false;
                }
                if (AchievementManager.Instance)
                {
                    AchievementManager.Instance.SetupAchievementList();
                }

            });

            _socket.On("pauseGame", (string payload) => {
                GameplayManager.getManager().PauseGame();
                Debug.Log("Paused");
            });

            _socket.On("moveDartUp", (string payload) =>
            {
                Debug.Log("Moving");
                if (DartManager.Instance)
                {
                    DartManager.Instance.AdjustBothSpawns(0.00f, 0.05f, 0.00f);
                }
            });

            _socket.On("moveDartDown", (string payload) =>

            {
                Debug.Log("Moving");
                if (DartManager.Instance)
                {
                    DartManager.Instance.AdjustBothSpawns(0.00f, -0.05f, 0.00f);
                }
            });

            _socket.On("resumeGame", (string payload) => {
                 GameplayManager.getManager().ResumeGame();
                 Debug.Log("Unpaused");
             });

             _socket.On("updatePatientPosition", (string payload) => {
                 Debug.Log(payload);
                 var axisPosition = JsonConvert.DeserializeObject<SocketClasses.Position>(payload);
                 PlayerManager.movePlayerX(axisPosition.playerXPosition);
                 PlayerManager.movePlayerY(axisPosition.playerYPosition);
                 PlayerManager.movePlayerZ(axisPosition.playerZPosition);
             });

            _socket.On("kickPatient", (string payload) => {
                Debug.Log(payload);
                Destroy(this);
                SceneManager.LoadScene("Initialize");
            });

             _socket.On("handMirror", (string payload) => {
                Debug.Log(payload);
                switch (payload) {
                    case "LEFT":
                        CalibrationManager.Instance?.Calibrate(CalibrationManager.CalibrationType.Mirror, OVRInput.Controller.LTouch);
                        break;
                    case "RIGHT":
                        CalibrationManager.Instance?.Calibrate(CalibrationManager.CalibrationType.Mirror, OVRInput.Controller.RTouch);
                        break;
                    default:
                        CalibrationManager.Instance?.Reset();
                        break;
                }                
             });
             
             _socket.On("IKRig", (string payload) => {
                var ikRigMeasurements = JsonConvert.DeserializeObject<SocketClasses.IKRig>(payload);
                SetIKTunning(ikRigMeasurements.shoulderWidth, ikRigMeasurements.headHeight, ikRigMeasurements.armLength, ikRigMeasurements.extendedArmThreshold, ikRigMeasurements.retractedArmThreshold);
             });

             _socket.On("toggleSkeleton", (string payload) => {
                PlayerManager.ToggleIKSkeleton();
             });
             
             _socket.On("handScale", (string payload) => {
                Debug.Log(payload);
                var handScale = JsonConvert.DeserializeObject<SocketClasses.HandScaling>(payload);
                Debug.Log("Scaling: " + handScale.handToScale + "\nAmount: " + handScale.scaleAmount);
                switch (handScale.handToScale)
                {
                    case "LEFT":
                        CalibrationManager.Instance?.Calibrate(CalibrationManager.CalibrationType.Scale, OVRInput.Controller.LTouch);
                        CalibrationManager.Instance?.changeScaleAmt(handScale.scaleAmount);
                        break;
                    case "RIGHT":
                        CalibrationManager.Instance?.Calibrate(CalibrationManager.CalibrationType.Scale, OVRInput.Controller.RTouch);
                        CalibrationManager.Instance?.changeScaleAmt(handScale.scaleAmount);
                        break;
                    default:
                        CalibrationManager.Instance?.Reset();
                        break;
                }
             });
            _socket.On("changePatientName", (string payload) =>
            {
                Debug.Log("Changesd name to: " + payload);
                PlayerPrefs.SetString("PatientName", payload);
                patientName.SetText(payload);
                SwitchToViewName();
            });

            _socket.On("setClinicianID", (string payload) =>
            {
                clinicianId = payload;
            });
        }
        
        private void OnDestroy()
        {
            _socket?.Close();
        }

        public void SetPatientName()
        {
            
            PlayerPrefs.SetString("PatientName", patientNameInput.text);
            FetchName();
        }
        void SetPatientName(string name)
        {
            
            PlayerPrefs.SetString("PatientName", name);
            FetchName();
        }

        public void SwitchToSetName()
        {
            Debug.Log("SetName!");
            setName.SetActive(true);
            patientName.gameObject.SetActive(false);
            keyboard.SetActive(true);
        }

        private void SwitchToViewName()
        {
            setName.SetActive(false);
            patientName.gameObject.SetActive(true);
            keyboard.SetActive(false);
        }

        private void SetIKTunning(float shoulderWidth, float headHeight, float armLength, float extendedArmThreshold, float retractedArmThreshold)
        {
            Debug.Log("Shoulder Width: " + shoulderWidth);
            Debug.Log("Head Height: " + headHeight);
            Debug.Log("Arm Length: " + armLength);
            Debug.Log("Extended Arm Threshold: " + extendedArmThreshold);
            Debug.Log("Retracted Arm Threshold: " + retractedArmThreshold);
            PlayerManager.SetIKShoulderWidth(shoulderWidth);
            PlayerManager.SetIKHeadHeight(headHeight);
            PlayerManager.SetIKArmLength(armLength);
            PlayerManager.SetExtendArmThreshold(extendedArmThreshold);
            PlayerManager.SetRetractArmThreshold(retractedArmThreshold);
        }

        public void SendPositionalData(string message)
        {
            _socket.Emit("positionalDataServer", clinicianId + ":" + message, true);
        }

        public void SendRepTrackingData(string message)
        {
            _socket.Emit("repTrackingDataServer", clinicianId + ":" + message, true);
        }

        public void SendGameEndedSignal()
        {
            SocketClasses.BalloonGameSettingsValues.gameIsRunning = false;
            _socket.Emit("gameEnded", clinicianId);
        }

        public void UpdateBalloonProgression()
        {

            Debug.Log("Sending Balloon Progression Update on iteration: " + counter);
            counter++;
            _socket.Emit("balloonProgressionUpdate",SocketClasses.BalloonGameDataValues.userName
                + ":" + SocketClasses.BalloonGameDataValues.levelOneScore
                + ":" + SocketClasses.BalloonGameDataValues.levelTwoScore
                + ":" + SocketClasses.BalloonGameDataValues.levelThreeScore
                + ":" + SocketClasses.BalloonGameDataValues.levelFourScore
                + ":" + SocketClasses.BalloonGameDataValues.levelFiveScore
                + ":" + SocketClasses.Achievements.PopOneBalloon.isAchieved
                + ":" + SocketClasses.Achievements.AllAchievements[1].isAchieved
                + ":" + SocketClasses.Achievements.AllAchievements[2].isAchieved
                + ":" + SocketClasses.Achievements.AllAchievements[3].isAchieved
                + ":" + SocketClasses.Achievements.AllAchievements[4].isAchieved
                + ":" + SocketClasses.Achievements.AllAchievements[5].isAchieved
                + ":" + SocketClasses.Achievements.AllAchievements[6].isAchieved
                + ":" + SocketClasses.Achievements.AllAchievements[7].isAchieved
                + ":" + SocketClasses.Achievements.AllAchievements[8].isAchieved
                + ":" + SocketClasses.Achievements.AllAchievements[9].isAchieved
                + ":" + clinicianId
                , true);
        }
    }
}
