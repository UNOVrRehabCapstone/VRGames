using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AchievementManager : MonoBehaviour
{

    //TEMPORARY EFFECT FOR ACHIEVEMENT UNLOCK
    // We should add a notification + sound effect on achievement unlock, plus a way to see them all ingame
    [SerializeField] private ParticleSystem confettiSystem;

    public AudioSource audioSource;


    //Balloon stream achievement variable
    private int balloonStreamPopped = 0;

    //Double barreled timer variable
    private float popInterval = 0.00f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
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
                    if (!SocketClasses.Achievements.PopEntireBalloonStream.isAchieved)
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
                Debug.Log("What????");
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
            default:
                {
                    Debug.Log("Huh? How did you get here??");
                    break;
                }
        }
    }


    private void UnlockAchievement(Achievement achievement)
    {
        achievement.isAchieved = true;
        if(audioSource != null)
        {
            audioSource.Play();
        }
        Debug.Log(achievement.name + " unlocked!");
        confettiSystem.Play();
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



public class Achievement
{
    public int id;
    public string name;
    public string description;
    public bool isAchieved;


    public Achievement(int id, string name, string description)
    {
        this.id = id;
        this.name = name;
        this.description = description;
        this.isAchieved = false;
    }

}

