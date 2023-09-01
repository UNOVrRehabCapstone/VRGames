using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class MasterArchitectureGameplayManager : GameplayManager
{
    public int maxHoops;
    public GameObject planePrefab;
    public GameObject hoopPrefab;
    public GameObject hoopSpawn;
    public GameObject leftTable;
    public GameObject rightTable;
    public float secondsTilDespawn = 3;
    private int currHoops = 0;
    private List<GameObject> planes = new List<GameObject>();
    private List<GameObject> hoops = new List<GameObject>();

    new void Start(){
        base.Start();
        PointsManager.addPointTrigger( "==", winConditionPoints, "onWinConditionPointsReached" );
        PointsManager.addPointTrigger( "==", 1, "onFirstPointReached" );
        spawnPlane();
        startSpawning();
    }
    
    public void onFirstPointReached(){
        print( "Congrats on your 1st point!!!" );
    }

    public override void onWinConditionPointsReached(){
       PointsManager.updateScoreboardMessage("You beat the Plane Game!");
    }

    //Called by Grabber when some Plane in list is grabbed
    public void onPlaneGrabbed( GameObject plane )
    {
        
        reset();
    }
    
    public void onPlaneReleased( GameObject plane )
    {
        StartCoroutine( despawnCountdown( plane ) );
        spawnPlane();
    }

    //Removes a Plane from the list and destroys it
    public void killPlane( GameObject plane )
    {
        planes.Remove( plane );
        Destroy( plane );
    }

    //Kills the passed plane and creates a new one
    public void respawnPlane( GameObject plane )
    {
        killPlane( plane );
        spawnPlane();
    }

    //Kills all planes in scene
    public override void reset()
    {
        foreach (GameObject plane in planes)
        {
            killPlane( plane );
        }
    }

    //Creates a new Plane GameObject and adds it to the list
    private void spawnPlane()
    {
        GameObject rightPlane = Instantiate(planePrefab);
        GameObject leftPlane = Instantiate(planePrefab);
        rightPlane.transform.position = rightTable.transform.position + new Vector3(0, rightTable.transform.localScale.y, 0);
        leftPlane.transform.position = leftTable.transform.position + new Vector3(0, leftTable.transform.localScale.y, 0);
        planes.Add(rightPlane);
        planes.Add(leftPlane);
    }

    //Coroutine for counting down and despawning plane after certain amount of time
    private IEnumerator despawnCountdown( GameObject obj )
    {
        float endTime = Time.realtimeSinceStartup + secondsTilDespawn;
        while ( Time.realtimeSinceStartup < endTime )
        {
            yield return new WaitForSeconds( .5f );
        }
        killPlane( obj );
    }

    private void spawnHoop()
    {
        Random rand = new Random();
        GameObject hoop = Instantiate(hoopPrefab);
        hoop.transform.position =
            hoopSpawn.transform.position + new Vector3(rand.Next(-2, 2), rand.Next( 2) , rand.Next(-1, 2));
        hoops.Add(hoop);
        currHoops++;
        StartCoroutine(despawnCountdown(hoop));
        if (currHoops % maxHoops == 0)
        {
            CancelInvoke();
            PointsManager.updateScoreboardMessage("That's it for this round!");
        }
    }

    private void startSpawning()
    {
        InvokeRepeating("spawnHoop", 0, 10.0f);
    }
    
}
