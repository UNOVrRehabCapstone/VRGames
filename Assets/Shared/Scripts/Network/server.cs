using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using Kinect = Windows.Kinect;
using Classes.Managers;
using TMPro;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using Object = System.Object;
using UnityEngine.XR;

public class server : MonoBehaviour
{
    public GameObject prefab;
    List<int> toInstantiate = new List<int>();
    private TcpListener tcpServer;
    private string scene;
    private string activeScene;
    private bool startGame;
    private bool pauseGame;
    private bool backToServer;
    private string jsonKinectData;
    private Kinect.Joint kinectJoint;
    private Process oculusMirror;
    private bool restartGame;
    private bool reposition;
    private bool resumeGame;
    private Queue<string[]> commandQueue = new Queue<string[]>();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Thread t = new Thread(StartServer);
        t.Start();
        activeScene = SceneManager.GetActiveScene().name;
        PlayerManager.movePlayer(GameObject.FindGameObjectWithTag("SpawnPoint"));
        GameObject clinicianMessage = GameObject.FindGameObjectWithTag("ClinicianMessage");
        clinicianMessage.GetComponent<TextMeshPro>().text = "Your Clinician Is \n Selecting A Game";
    }

    void Update()
    {
        if (toInstantiate.Count > 0)
        {
			Instantiate(prefab);
			toInstantiate.RemoveAt(0);
		}
        
        if (SceneManager.GetActiveScene().name == scene && !restartGame)
        {
            startGame = false;
        }
        
        if (startGame)
        {
            Debug.Log("Attempting to switch games");
            SwitchGames();
            if (restartGame)
            {
                restartGame = false;
            }
        }
        
        if (pauseGame)
        {
            GameplayManager.getManager().PauseGame();
            pauseGame = false;
        }
        
        if (resumeGame)
        {
            GameplayManager.getManager().ResumeGame();
            resumeGame = false;
        }
        
        if (backToServer)
        {
            ElectronSceneManager.GoToServer();
            backToServer = false;
        }

        if (commandQueue.Count > 0)
        {
            HandleClientCommands(commandQueue.Dequeue());
        }
    }

    void StartServer()
    {
        try
        {
            Debug.Log("One");
            Debug.Log("Two");
            var ipAddresses = Dns.GetHostEntry("localhost").AddressList;
            foreach (var address in ipAddresses)
            {
                Debug.Log(address);
            }

            var ipAddress = Dns.GetHostEntry("localhost").AddressList[1];
            tcpServer = new TcpListener(ipAddress, 1100);
            tcpServer.Start();

            Debug.Log("Started Server");

            while (true)
            {
                Debug.Log("Waiting for a client");
                TcpClient client = tcpServer.AcceptTcpClient();
                Debug.Log("Got a client");
                HandleClient(client);
                Thread t = new Thread(HandleClient);
                t.Start(client);
            }
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }

    void HandleClient(Object obj)
    {
        //Instantiate(prefab);
        toInstantiate.Add(0);
        //Kinect.Body[] data;
        
        
        TcpClient client = (TcpClient)obj;
        StreamReader clientIn = new StreamReader(client.GetStream());
        StreamWriter clientOut = new StreamWriter(client.GetStream());
        clientOut.AutoFlush = true;
        clientOut.Write(activeScene);
        /*do {
            data = kinectManager.GetBodies();
            clientOut.Write(jsonKinectData);
            Thread.Sleep(100);
        } while (data != null);*/
        string msg;
        while ((msg = clientIn.ReadLine()) != null)
        {
            string[] commands = msg.Split('|');
            commandQueue.Enqueue(commands);
        }
    }
    void HandleClientCommands(string[] commands)
    {
        switch (commands[0])
        {
            case "Command":
                HandleCommands(commands[1]);
                break;
            case "Message":
                Debug.Log(commands[1]);
                break;
            default:
                Debug.Log("Client Sent: " + commands[0]);
                break;
        }
    }
    
    private void HandleCommands(string commands)
    {
        string[] split = commands.Split('#');
        string value = "";
        if (split.Length > 1)
        {
            value = split[1];
        }
        switch (split[0])
        {
            case "Play":   
                startGame = true;
                pauseGame = false;
                scene = value;
                break;
            case "Pause":
                pauseGame = true;
                Debug.Log("Pause Command Received");
                break;
            case "Resume":
                resumeGame = true;
                break;
            case "Stop":
                startGame = false;
                Debug.Log("Stop Command Received");
                backToServer = true;
                break;
            case "Restart": 
                startGame = true;
                restartGame = true;
                Debug.Log("Restart Command Received");
                break;
            case "UpDown":
                PlayerManager.movePlayerY(float.Parse(value));
                break;
            case "LeftRight":
                PlayerManager.movePlayerX(float.Parse(value));
                break;
            case "ForwardBack":
                PlayerManager.movePlayerZ(float.Parse(value));
                break;
            default: 
                Debug.Log("Command Sent: " + commands[0]);
                break;
        }
    }

    //Here is where you add the functionality to the command buttons given in the clinician view
    //To add more, go to the clinician view and add your command to the list of commands already given per game.
    //Once that is added, come back here and add your case and give functionality to it.
    void HandleSubCommands(string cmd)
    {
        string[] split = cmd.Split('#');
        string direction = split[0];
        Debug.Log(direction);
        Debug.Log(commandQueue.Count);
        switch (direction)
        {
            case "UpDown":
                var amountY = float.Parse(split[1]);
                Debug.Log(amountY);
                PlayerManager.setPlayerPositionY(amountY);
                break;
            case "ForwardBack":
                var amountZ = float.Parse(split[1]);
                Debug.Log(amountZ);
                PlayerManager.setPlayerPositionZ(amountZ);
                break;
            case "LeftRight":
                var amountX = float.Parse(split[1]);
                Debug.Log(amountX);
                PlayerManager.setPlayerPositionX(amountX);
                break;
            case "ResetPosition":
                PlayerManager.resetPosition();
                //UnityEngine.XR.InputTracking.Recenter();
                // Modern input recentering found at: https://forum.unity.com/threads/any-example-of-the-new-2019-1-xr-input-system.629824/page-2
                List<XRInputSubsystem> subsystems = new List<XRInputSubsystem>();
                SubsystemManager.GetInstances(subsystems);
                foreach (XRInputSubsystem sys in subsystems)
                {
                    sys.TrySetTrackingOriginMode(TrackingOriginModeFlags.Device);
                    sys.TryRecenter();
                }
                break;
            /*
            case "Add Balloon":
                GameObject.Find("BalloonManager").GetComponent<BalloonManager>().SpawnBalloon();
                break; */
            case "Toggle Timed Targets":
                GameObject.Find("Manager").GetComponent<PlaneGameplayManager>().ToggleTimedTargets();
                break;
            case "Increase Difficulty":
                GameObject.Find("Manager").GetComponent<GameplayManager>().IncreaseDifficulty();
                break;
            case "Decrease Difficulty":
                GameObject.Find("Manager").GetComponent<GameplayManager>().DecreaseDifficulty();
                break;
            default:
                break;
        }
    }

    //Here is where you add games to the internal game engine.
    private void SwitchGames()
    {
        switch (scene)
        {
            case "Blocks":
                ElectronSceneManager.GoToBoxAndBlocks();
                break;
            case "Climbing":
                ElectronSceneManager.GoToClimbingGame();
                break;
            case "Balloons":
                ElectronSceneManager.GoToBalloonGame();
                break;
            case "Planes":
                ElectronSceneManager.GoToPlaneGame();
                break;
            case "TestAddition":
                ElectronSceneManager.GoToTestGame();
                break;
            default:
                Debug.Log("No valid game selected");
                break;
        }
    }

    public void convertKinectJointToJson(Kinect.Joint joint)
    {
        jsonKinectData = JsonConvert.SerializeObject(joint);
        Debug.Log(jsonKinectData);
    }
}