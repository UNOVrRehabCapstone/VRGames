using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

/**
 * The AchievmentManager class handle the achievements in the game.
 */
public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance { get; private set; }
    //TEMPORARY EFFECT FOR ACHIEVEMENT UNLOCK
    // We should add a notification + sound effect on achievement unlock, plus a way to see them all ingame
    [SerializeField] private ParticleSystem confettiSystem;

    public AudioSource audioSource;


    //Balloon stream achievement variable
    private int balloonStreamPopped = 0;

    //Double barreled timer variable
    private float popInterval = 0.00f;

    GameObject[] achievementObjects;

    [SerializeField]
    GameObject achievementList;

    [SerializeField]
    GameObject achievementPopUpPrefab;

    private bool isDisplayingAchievement = false;

    private Queue<string> achievementQueue = new Queue<string>();

    private void Awake()
    {
        //Singleton pattern make sure there is only one balloon gameplay manager.
        if (Instance != null)
        {
        }
        Instance = this;

    }
    void Start()
    {
 
        //Transform canvas = transform.Find("Achievement List").Find("Canvas").Find("Name");
        // TextMeshProUGUI textMesh = subobject.GetComponent<TextMeshProUGUI>();
        // textMesh.text = "Test";
        //Debug.Log(textMesh.text);
        this.SetupAchievementList();
        audioSource = GetComponent<AudioSource>();
    }


    /**
     * Sets up the achivement list that is displayed in game.
     */
    public void SetupAchievementList()
    {
        TextMeshProUGUI textMesh;
        Transform temp;
        
        //Setup username
        temp = transform.Find("Achievement List").Find("Canvas").Find("Name");
        textMesh = temp.GetComponent<TextMeshProUGUI>();
        textMesh.text = SocketClasses.BalloonGameDataValues.userName + "'s Achievements!";

        //Setup array of achievement objects
        achievementObjects = GameObject.FindGameObjectsWithTag("Achievement");
        Array.Sort(achievementObjects, (obj1, obj2) => String.Compare(obj1.name, obj2.name));
        for (int i = 0; i < achievementObjects.Length; i++)
        {
            //Setup names
            temp = achievementObjects[i].transform.Find("Name");
            textMesh = temp.GetComponent<TextMeshProUGUI>();
            textMesh.text = SocketClasses.Achievements.AllAchievements[i].name;

            //Setup Descriptions
            temp = achievementObjects[i].transform.Find("Description");
            textMesh = temp.GetComponent<TextMeshProUGUI>();
            textMesh.text = SocketClasses.Achievements.AllAchievements[i].description;

            //Setup Checkmarks
            temp = achievementObjects[i].transform.Find("Checkmark");
            if(SocketClasses.Achievements.AllAchievements[i].isAchieved)
            {
                temp.gameObject.SetActive(true);
            }
            else
            {
                temp.gameObject.SetActive(false);
            }

        }

    }

    /**
     * Hides the achievement list.
     */
    public void HideAchievementList()
    {
        if(achievementList != null)
        {
            achievementList.SetActive(false);
        }
    }

    /**
     * Displays the achievement list.
     */
    public void ShowAchievementList()
    {
        if (achievementList != null)
        {
            achievementList.SetActive(true);
            this.SetupAchievementList();
        }
    }

    private void OnEnable()
    {
        Balloon.OnPop += HandleBalloonPop;
        Classes.Managers.BalloonGameplayManager.OnAchievementUpdate += HandleGameUpdates;
    }
    private void OnDisable()
    {
        Balloon.OnPop -= HandleBalloonPop;
        Classes.Managers.BalloonGameplayManager.OnAchievementUpdate -= HandleGameUpdates;
    }


    private void FixedUpdate()
    {
        if(popInterval > 0)
        {
            popInterval = popInterval - Time.deltaTime;
        }

    }

    private void HandleBalloonPop(string message)
    {
        switch (message)
        {
            case "Balloon":
                {
                    // Achievement 0 - Pop one balloon
                    if (!SocketClasses.Achievements.PopOneBalloon.isAchieved)
                    {
                        UnlockAchievement(SocketClasses.Achievements.PopOneBalloon);
                    }
                    // Achievement 4 - Pop two balloons at once
                    if(popInterval > 0)
                    {
                        if (!SocketClasses.Achievements.PopTwoAtOnce.isAchieved)
                        {
                            UnlockAchievement(SocketClasses.Achievements.PopTwoAtOnce);
                            popInterval = 0;
                        }
                    }
                    // Reset pop interval
                    popInterval = 1.00f;
                    break;
                }
                // Achievement 1 - Fully pop a balloon stream
                // Also logs that balloon stream has been popped
            case "Balloon_Stream_Powerup": 
                {
                    this.balloonStreamPopped = 0;
                    break;
                }
            case "SpawnStreamMember": 
                {
                    this.balloonStreamPopped++;
                    break;
                }
            case "SpawnStreamMemberLast": 
                {
                    this.balloonStreamPopped++;
                    if(balloonStreamPopped >= 5)
                    {
                        if (!SocketClasses.Achievements.PopEntireBalloonStream.isAchieved)
                        {
                            UnlockAchievement(SocketClasses.Achievements.PopEntireBalloonStream);
                            SocketClasses.Achievements.SpecialsPopped[0] = true;
                            IsEverySpecialPopped();
                        }
                    }

                    break;
                }
                // Achievement 2 - Fully pop a target ballooon
                // Also logs that target balloon has been popped
            case "Target Balloon Fully Popped":
                {
                    if (!SocketClasses.Achievements.PopEntireTargetBalloon.isAchieved)
                    {
                        UnlockAchievement(SocketClasses.Achievements.PopEntireTargetBalloon);
                        SocketClasses.Achievements.SpecialsPopped[1] = true;
                        IsEverySpecialPopped();
                    }
                    break;
                }
                // Achievement 3 - Pop the core of an onion balloon
                // Also logs that Onion balloon has been popped
            case "OnionLayer3":
                {
                    if (!SocketClasses.Achievements.PopOnionCore.isAchieved)
                    {
                        UnlockAchievement(SocketClasses.Achievements.PopOnionCore);
                        SocketClasses.Achievements.SpecialsPopped[2] = true;
                        IsEverySpecialPopped();
                    }
                    break;
                }
                // Logs that restore life balloon has been popped
            case "RestoreLife":
                {
                    SocketClasses.Achievements.SpecialsPopped[3] = true;
                    IsEverySpecialPopped();
                    break;
                }
            default:
                break;
        }
    }


    private void HandleGameUpdates(string message)
    {
        switch (message)
        {
            // Achievement 6 - End with more lives than you started with
            case "Ended with more lives":
                {
                    if (!SocketClasses.Achievements.EndWithMoreLives.isAchieved)
                    {
                        UnlockAchievement(SocketClasses.Achievements.EndWithMoreLives);
                    }
                    break;
                }
                // Achievement 7 - Finish a custom game
            case "Custom game ended":
                {
                    if (!SocketClasses.Achievements.FinishedCustomGame.isAchieved)
                    {
                        UnlockAchievement(SocketClasses.Achievements.FinishedCustomGame);
                    }
                    break;
                }
            case "Played Both Environments":
                {
                    if (!SocketClasses.Achievements.PlayBothEnvironments.isAchieved)
                    {
                        UnlockAchievement(SocketClasses.Achievements.PlayBothEnvironments);
                    }
                    break;
                }
            case "Played All Career Levels":
                {
                    if (!SocketClasses.Achievements.FinishCareerMode.isAchieved)
                    {
                        UnlockAchievement(SocketClasses.Achievements.FinishCareerMode);
                    }
                    break;
                }
            default:
                {
                    Debug.Log("Huh? How did you get here??");
                    break;
                }
        }
    }


    private void UnlockAchievement(Achievement achievement)
    {

        if(audioSource != null)
        {
            audioSource.Play();
        }
        Debug.Log(achievement.name + " unlocked!");
        achievementQueue.Enqueue(achievement.name);
        if (!isDisplayingAchievement)
        {
            StartCoroutine(DisaplyNextAchievement());
        }
        achievement.isAchieved = true;
        if (Network.NetworkManager.Instance)
        {
            Network.NetworkManager.Instance.UpdateBalloonProgression();

        }

        //confettiSystem.Play();
    }

    private IEnumerator DisplayPopup(string name)
    {

        Vector3 spawnLoc = new Vector3(-2.05f, -0.7f, 2.376f);
        Quaternion spawnRot = Quaternion.Euler(new Vector3(0.0f, -30.0f, 0.0f));
        GameObject newPopup = Instantiate(achievementPopUpPrefab, spawnLoc, spawnRot);
        Transform infoTransform = newPopup.transform.Find("Info").Find("AchievementName");
        TextMeshProUGUI text = infoTransform.GetComponent<TextMeshProUGUI>();
        //This works!
        text.text = name;
        yield return new WaitForSeconds(1);
        

    }

    private IEnumerator DisaplyNextAchievement()
    {
        isDisplayingAchievement = true;

        while (achievementQueue.Count > 0)
        {
            string achievementName = achievementQueue.Dequeue();
            yield return DisplayPopup(achievementName);
        }

        isDisplayingAchievement = false;
    }



    // Achievement 5 - Pop all specials
    private void IsEverySpecialPopped()
    {
        for(int i = 0; i < SocketClasses.Achievements.SpecialsPopped.Length; i++)
        {
            if (!SocketClasses.Achievements.SpecialsPopped[i])
            {
                return;
            }
        }
        UnlockAchievement(SocketClasses.Achievements.PopAllSpecials);
    }
}



/**
 * The Achievment class can be used to represent an achievement.
 */
public class Achievement
{
    public int id;
    public string name;
    public string description;
    public bool isAchieved;
    
    /**
     * The constructor to create an achievement.
     *
     * @param id of the achievement.
     * @param name of the achievement.
     * @param description of the achievement.
     */
    public Achievement(int id, string name, string description)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.isAchieved = false;
    }

}

