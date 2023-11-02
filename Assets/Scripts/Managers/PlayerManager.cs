﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRArmIK;

public class PlayerManager : MonoBehaviour
{
    public GameObject playerPrefab;
    public GameObject playerIK;

    public static bool isIKRendering;
    
    private static GameObject player;
    private static GameObject avatar;
    private static GameObject playerSpawn;

    private static RepTrackerIK repTracker; 
    private static Renderer[] ikRenderers;
    private static PoseManager poseManager; 

    //Create Player before any Start methods are called
    void Awake()
    {
        isIKRendering = false;
        player = Instantiate(playerPrefab);
        avatar = Instantiate(playerIK);
        playerSpawn = GameObject.FindGameObjectWithTag("SpawnPoint");

        ikRenderers = avatar.GetComponentsInChildren<Renderer>();
        repTracker = player.GetComponentInChildren<RepTrackerIK>();
        poseManager = player.GetComponentInChildren<PoseManager>();
    }

    // TODO: Remove this eventually. Currently in place for testing purposes.
    private void FixedUpdate()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        {
            ToggleIKSkeleton();
        }

        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            SetExtendArmThreshold(repTracker.extendedRot + 10);
        }

        if (Input.GetKeyUp(KeyCode.DownArrow))
        {
            SetExtendArmThreshold(repTracker.extendedRot - 10);
        }

        if (Input.GetKeyUp(KeyCode.LeftArrow))
        {
            SetIKArmLength(poseManager.playerArmLength - 0.01f);
        }

        if (Input.GetKeyUp(KeyCode.RightArrow))
        {
            SetIKArmLength(poseManager.playerArmLength + 0.01f);
        }

        //if (Input.GetKeyUp(KeyCode.LeftArrow))
        //{
        //    SetIKShoulderWidth(poseManager.playerWidthShoulders - 0.01f);
        //}

        //if (Input.GetKeyUp(KeyCode.RightArrow))
        //{
        //    SetIKShoulderWidth(poseManager.playerWidthShoulders + 0.01f);
        //}
    }

    public static GameObject getPlayer(){ return player; }

    public static void resetPosition()
    {
        player.transform.position = playerSpawn.transform.position;
        avatar.transform.position = playerSpawn.transform.position;
    }

    public static GameObject getLeftHand()
    {
        return GameObject.FindGameObjectWithTag("LeftGrabber");
    }
    
    public static GameObject getRightHand()
    {
        return GameObject.FindGameObjectWithTag("RightGrabber");
    }

    public static void movePlayerX(float x)
    {
        player.transform.position = new Vector3(x, player.transform.position.y, player.transform.position.z);
    }
    
    public static void movePlayerY(float y)
    {
        player.transform.position = new Vector3(player.transform.position.x, y, player.transform.position.z);
    }
    
    public static void movePlayerZ(float z)
    {
        player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, z);
    }
    
    public static void movePlayer(GameObject warp)
    {
        player.transform.position = warp.transform.position;
    }

    public static void offsetMovePlayer(GameObject warp, Vector3 offset)
    {
        player.transform.position = warp.transform.position - offset;
    }
    
    public static void setPlayerPositionX(float x)
    {
        player.transform.position = new Vector3(x, player.transform.position.y, player.transform.position.z);
    }
    public static void setPlayerPositionY(float y)
    {
        player.transform.position = new Vector3(player.transform.position.x, y, player.transform.position.z);
    }
    public static void setPlayerPositionZ(float z)
    {
        player.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, z);
    }


    // TODO: Implement functionalities in the clinician view for the following methods...
    public static void SetIKShoulderWidth (float width)
    {
        poseManager.playerWidthShoulders = width;
        poseManager.Calibrate();
    }
    
    public static void SetIKHeadHeight(float height)
    {
        poseManager.playerHeightHmd = height;
        poseManager.Calibrate();
    }

    public static void SetIKArmLength(float length)
    {
        poseManager.playerArmLength = length;
        poseManager.Calibrate();
    }

    public static void SetExtendArmThreshold(float extendRot)
    {
        repTracker.extendedRot = extendRot;
    }
    
    public static void SetRetractArmThreshold(float retractRot)
    {
        repTracker.retractedRot = retractRot;
    }

    // NOTE: Either of these work, it is to be determined which is preferred. 
    /// <summary>
    /// Shows or hides the player IK rig based on the passed boolean value.
    /// </summary>
    /// <param name="show">If True, shows the skeleton. If False, hides the skeleton</param>
    public static void ShowIKSkeleton(bool show)
    {
        isIKRendering = show;
        foreach (Renderer r in ikRenderers)
        {
            r.enabled = isIKRendering;
        }
    }

    /// <summary>
    /// When called, toggles the view of the player's IK skeleton.
    /// </summary>
    public static void ToggleIKSkeleton()
    {
        ShowIKSkeleton(!isIKRendering);
    }
}
