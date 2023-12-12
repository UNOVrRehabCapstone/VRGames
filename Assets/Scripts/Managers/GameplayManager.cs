using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR;

public class GameplayManager : MonoBehaviour
{
    public int difficulty = 1;

    private GameObject clinicianPauseText;
    //public GameObject spawnWarp;
    public static GameplayManager Instance { get; private set; }
    public int winConditionPoints = 50;
    public int maxTime;

    public GameSettingsSO gameSettings;

    public int playerLives;

    //secretly abstract, but can't make it abstract for a few reasons
    public virtual void reset()
    {
        print("GameplayManager: OVERRIDE THE reset METHOD!!!!!!!!");
    }

    public GameSettingsSO GetGameSettings()
    {
        return this.gameSettings;
    }


    public static GameplayManager getManager()
    {
        return Instance;
    }

    //we only need this 
    public virtual void onWinConditionPointsReached()
    {
        print("GameplayManager: OVERRIDE THE onWinConditionPointsReached METHOD!!!!!!!!");
    }

    public void setDifficulty(int newDif){ difficulty = newDif; }
    public int getDifficulty() { return difficulty; }

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(Instance);
        }
            Instance = this;

    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) Application.Quit();
    }

    protected void Start() 
    {
        // Set the update time to match the headset's framerate
        // This fixes many jittering issues!        
        if (OVRManager.display != null)
        {
            Time.fixedDeltaTime = (Time.timeScale / OVRManager.display.displayFrequency);

        }
        else
        {
            Time.fixedDeltaTime = Time.timeScale / 60.0f;
        }

        // Change the log level
        OVRPlugin.SetLogCallback2(null);
        //OVRPlugin.lo

        PlayerManager.movePlayer(GameObject.FindGameObjectWithTag("SpawnPoint"));

        //InputTracking.Recenter();
        // Modern input recentering found at: https://forum.unity.com/threads/any-example-of-the-new-2019-1-xr-input-system.629824/page-2
        List<XRInputSubsystem> subsystems = new List<XRInputSubsystem>();
        SubsystemManager.GetInstances(subsystems);
        foreach (XRInputSubsystem sys in subsystems)
        {
            sys.TrySetTrackingOriginMode(TrackingOriginModeFlags.Device);
            sys.TryRecenter();
        }

        clinicianPauseText = GameObject.FindGameObjectWithTag("ClinicianMessage");
        if (clinicianPauseText != null)
        {
            clinicianPauseText.GetComponent<TextMeshPro>().text = "Your Clinician Has \n Paused The Game";
            clinicianPauseText.SetActive(false);
        }
        reset();
    }

    public virtual void RefreshSettings()
    {

    }


    public void PauseGame()
    {
        if (clinicianPauseText != null)
        {
            clinicianPauseText.SetActive(true);
        }
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        if (clinicianPauseText != null)
        {
            clinicianPauseText.SetActive(false);
        }
        Time.timeScale = 1;
    }

    public void IncreaseDifficulty()
    {
        if(this.difficulty < 3)
        {
            this.difficulty++;
        }
        
    }
    public void DecreaseDifficulty()
    {
        if(this.difficulty > 1)
        {
            this.difficulty--;
        }
        
    }



}
