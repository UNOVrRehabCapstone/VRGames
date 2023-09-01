using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Random = System.Random;

public class PlaneGameplayManager : GameplayManager
{
    public int maxHoops;
    public GameObject planePrefab;
    public GameObject hoopPrefab;
    public GameObject hoopSpawn;
    public GameObject leftTable;
    public GameObject rightTable;
    public float secondsTilDespawn;
    private int currHoops = 0;

    [SerializeField]
    private List<GameObject> planes = new List<GameObject>();

    private List<GameObject> hoops = new List<GameObject>();
    private bool timedTargets;
    private bool gameIsOver;


   
    new void Start()
    {
        base.Start();
        /*PointsManager.addPointTrigger( "==", winConditionPoints, "onWinConditionPointsReached" );
        PointsManager.addPointTrigger( "==", 1, "onFirstPointReached" );*/
        PointsManager.updateScoreboardMessage("Grab A Plane To Start!");
        SpawnPlanes();
    }

    /*private void Update()
    {
        if (gameIsOver)
        {
            Debug.Log("resetting game, should clear targets");
            reset();
        }
    }*/

    private void FixedUpdate()
    {
        if (hoops.Count == 0)
        {
            StartSpawningHoops();
        }
    }

    public void ToggleTimedTargets()
    {
        timedTargets = !timedTargets;
        reset();
        SpawnPlanes();
    }

    public void OnFirstPointReached(){
        print( "Congrats on your 1st point!!!" );
    }

    /*public override void onWinConditionPointsReached(){
       PointsManager.updateScoreboardMessage("You beat the Plane Game!");
       gameIsOver = true;
       reset();
    }*/

    //Called by Grabber when some Plane in list is grabbed
    public void OnPlaneGrabbed( GameObject plane )
    {
        PointsManager.updateScoreboardMessage("Hit " + winConditionPoints + " Targets To Win!");
    }
    
    public void OnPlaneReleased( GameObject plane )
    {
        if (!gameIsOver)
        {
            if (plane.CompareTag("RightPlane"))
            {
                SpawnRightPlane();
            }
            else if (plane.CompareTag("LeftPlane"))
            {
                SpawnLeftPlane();
            }
        }
        StartCoroutine( DespawnCountdown( plane ) );
    }

    //Removes a Plane from the list and destroys it
    public void KillPlane( GameObject plane )
    {
        planes.Remove(plane);
        Destroy(plane);
    }

    public void KillHoop(GameObject hoop)
    {
        hoops.Remove(hoop);
        Destroy(hoop, .5f);
    }

    //Kills the passed plane and creates a new one
    public void RespawnPlane( GameObject plane )
    {
        KillPlane(plane);
        SpawnPlanes();
    }

    //Kills all planes in scene
    public override void reset()
    {
        ClearHoops();
        ClearPlanes();
    }

    void ClearHoops()
    {
        foreach (GameObject hoop in hoops.ToList())
        {
            KillHoop(hoop);
        }
    }

    void ClearPlanes()
    {
        foreach (GameObject plane in planes.ToList())
        {
            KillPlane(plane);
        }
    }

    //Creates a new Plane GameObject and adds it to the list
    private void SpawnPlanes()
    {
        SpawnRightPlane();
        SpawnLeftPlane();
    }

    void SpawnRightPlane()
    {
        GameObject rightPlane = Instantiate(planePrefab);
        rightPlane.tag = "RightPlane";
        rightPlane.transform.position = rightTable.transform.position + new Vector3(0, rightTable.transform.localScale.y + .5f, 0);
        planes.Add(rightPlane);
    }

    void SpawnLeftPlane()
    {
        GameObject leftPlane = Instantiate(planePrefab);
        leftPlane.tag = "LeftPlane";
        leftPlane.transform.position = leftTable.transform.position + new Vector3(0, leftTable.transform.localScale.y + .5f, 0);
        planes.Add(leftPlane);
    }

    //Coroutine for counting down and despawning plane after certain amount of time
    private IEnumerator DespawnCountdown( GameObject obj )
    {
        float endTime = Time.realtimeSinceStartup + secondsTilDespawn;
        while ( Time.realtimeSinceStartup < endTime )
        {
            yield return new WaitForSeconds( .5f );
        }
        KillPlane( obj );
    }
    
    private IEnumerator DespawnHoopCountdown( GameObject obj )
    {
        float endTime = Time.realtimeSinceStartup + 4;
        while ( Time.realtimeSinceStartup < endTime )
        {
            yield return new WaitForSeconds( 2.2f );
        }
        KillHoop( obj );
    }

    private void SpawnHoop()
    {
        Random rand = new Random();
        GameObject hoop = Instantiate(hoopPrefab);
        hoop.transform.position =
            hoopSpawn.transform.position + new Vector3(rand.Next(-6, 6), rand.Next( 2) , rand.Next(-1, 5));
        hoops.Add(hoop);
        currHoops++;
        if (timedTargets)
        {
            StartCoroutine(DespawnHoopCountdown(hoop));
            if (currHoops % maxHoops == 0)
            {
                PointsManager.updateScoreboardMessage("That's it for this round!");
                gameIsOver = true;
                reset();
            }
        }
    }

    public void StartSpawningHoops()
    {
        if (!gameIsOver)
        {
           if (timedTargets)
           {
               do
               {
                   SpawnHoop();
               } while (hoops.Count == 0);
           }
           else if (hoops.Count == 0)
           {
               SpawnHoop();
           } 
        }
    }
}
