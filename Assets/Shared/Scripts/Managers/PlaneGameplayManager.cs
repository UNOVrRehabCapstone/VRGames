/**
 * \file PlaneGameplayManager.cs
 * \brief Manages the gameplay logic specific to a plane-based game, including spawning, despawning, and interaction events for planes.
 *
 * PlaneGameplayManager extends the GameplayManager to provide specific functionalities for managing planes in the game. It includes methods to spawn planes, handle their lifecycle, and manage gameplay events related to plane interactions.
 */


using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Random = System.Random;

/**
 * \class PlaneGameplayManager
 * \brief Manages the gameplay logic for plane interactions, including spawning and lifecycle management.
 *
 * This class is responsible for spawning planes, handling their grab and release events, and managing their lifecycle within the game. It also provides mechanisms to clear planes and reset the gameplay state.
 */
public class PlaneGameplayManager : GameplayManager
{
    //public int maxHoops;
    //public GameObject hoopPrefab;
    //public GameObject hoopSpawn;
    //private int currHoops = 0;
    public GameObject planePrefab;
    // public GameObject leftTable;
    // public GameObject rightTable;
    // public GameObject centerTable;
    public GameObject newTable;
    public float secondsTilDespawn;
    Random rnd = new Random();

    [SerializeField]
    private List<GameObject> planes = new List<GameObject>();

    //private List<GameObject> hoops = new List<GameObject>();
    private bool timedTargets;
    private bool gameIsOver;


    /**
     * \brief Initializes the game, sets up the scoreboard message, and spawns initial planes.
     */   
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
        /*
        if (hoops.Count == 0)
        {
            StartSpawningHoops();
        }
        */
    }

    /**
     * \brief Toggles the timed target feature and resets the game state, respawning planes.
     */
    public void ToggleTimedTargets()
    {
        timedTargets = !timedTargets;
        reset();
        SpawnPlanes();
    }

    /**
     * \brief Callback for when the first point is reached in the game.
     */
    public void OnFirstPointReached(){
        print( "Congrats on your 1st point!!!" );
    }

    /*public override void onWinConditionPointsReached(){
       PointsManager.updateScoreboardMessage("You beat the Plane Game!");
       gameIsOver = true;
       reset();
    }*/

    /**
     * \brief Handles the event when a plane is grabbed, updating the scoreboard message.
     *
     * \param plane The plane GameObject that was grabbed.
     */
    //Called by Grabber when some Plane in list is grabbed
    public void OnPlaneGrabbed( GameObject plane )
    {
        PointsManager.updateScoreboardMessage("Hit " + winConditionPoints + " Targets To Win!");
    }
    
    /**
     * \brief Handles the event when a plane is released, spawning new planes as needed.
     *
     * \param plane The plane GameObject that was released.
     */
    public void OnPlaneReleased( GameObject plane )
    {
        // if (!gameIsOver)
        // {
        //     if (plane.CompareTag("RightPlane"))
        //     {
        //         SpawnRightPlane();
        //     }
        //     else if (plane.CompareTag("LeftPlane"))
        //     {
        //         SpawnLeftPlane();
        //     }
        // }
        // StartCoroutine( DespawnCountdown( plane ) );
<<<<<<< HEAD
=======
        plane.GetComponent<PlaneSound>().PlayThrowSound();
>>>>>>> SoundOverhaul
        SpawnPlanes();
    }

    //Removes a Plane from the list and destroys it
    /**
     * \brief Removes a plane from the game, destroying its GameObject and removing it from the active list.
     *
     * \param plane The plane GameObject to be removed.
     */
    public void KillPlane( GameObject plane )
    {
        planes.Remove(plane);
        Destroy(plane);
    }

    /*
    public void KillHoop(GameObject hoop)
    {
        hoops.Remove(hoop);
        Destroy(hoop, .5f);
    }
    */

    //Kills the passed plane and creates a new one
    /**
     * \brief Respawns a plane by first removing the old one and then spawning a new one.
     *
     * \param plane The plane GameObject to be respawned.
     */
    public void RespawnPlane( GameObject plane )
    {
        KillPlane(plane);
        SpawnPlanes();
    }

    //Kills all planes in scene
    /**
     * \brief Clears all planes from the game, removing their GameObjects and clearing the list.
     */
    public override void reset()
    {
        //ClearHoops();
        ClearPlanes();
    }

    /*
    void ClearHoops()
    {
        foreach (GameObject hoop in hoops.ToList())
        {
            KillHoop(hoop);
        }
    }
    */

    /**
     * \brief Clears all plane GameObjects from the scene.
     */
    void ClearPlanes()
    {
        foreach (GameObject plane in planes.ToList())
        {
            KillPlane(plane);
        }
    }

    //Creates a new Plane GameObject and adds it to the list
    /**
     * \brief Instantiates a new plane GameObject and adds it to the list of active planes.
     */
    private void SpawnPlanes()
    {
        GameObject plane = Instantiate(planePrefab);
        plane.tag = "Plane";
        // int i = rnd.Next();
        // if (i % 3 == 0){
        //     SpawnRightPlane(plane);
        // } else if (i % 3 == 1) {
        //     SpawnLeftPlane(plane);
        // } else {
        //     SpawnCenterPlane(plane);
        // }
        SpawnPlane(plane);
    }

    /**
     * \brief Spawns and positions a new plane in the game world.
     *
     * This method creates a new plane GameObject, sets its position and orientation, and adds it to the list of active planes.
     *
     * \param plane The plane GameObject to be spawned and positioned.
     */
    void SpawnPlane(GameObject plane)
    {
        plane.transform.position = new Vector3(0, 1, 0.65f);
        int degrees = rnd.Next(0, 180);
        degrees -= 90;
        plane.transform.RotateAround(new Vector3(0,0,0), Vector3.up, degrees);
        planes.Add(plane);
    }

    //  LEGACY SPAWN PLANE METHODS, DOES NOT WORK
    //
    // void SpawnRightPlane(GameObject plane)
    // {
    //     GameObject rightPlane = Instantiate(planePrefab);
    //     rightPlane.tag = "RightPlane";
    //     plane.transform.position = rightTable.transform.position + new Vector3(0, rightTable.transform.localScale.y + .5f, 0);
    //     planes.Add(plane);
    // }

    // void SpawnLeftPlane(GameObject plane)
    // {
    //     GameObject leftPlane = Instantiate(planePrefab);
    //     leftPlane.tag = "LeftPlane";
    //     plane.transform.position = leftTable.transform.position + new Vector3(0, leftTable.transform.localScale.y + .5f, 0);
    //     planes.Add(plane);
    // }

    // void SpawnCenterPlane(GameObject plane)
    // {
    //     plane.transform.position = centerTable.transform.position + new Vector3(0, centerTable.transform.localScale.y + .5f, 0);
    //     planes.Add(plane);
    // }

    //Coroutine for counting down and despawning plane after certain amount of time
    /**
     * \brief Initiates a countdown for an object, after which the object will be despawned.
     *
     * This coroutine waits for a specified amount of time (secondsTilDespawn) before calling KillPlane to remove the object from the game.
     *
     * \param obj The GameObject to despawn after the countdown.
     */
    private IEnumerator DespawnCountdown( GameObject obj )
    {
        float endTime = Time.realtimeSinceStartup + secondsTilDespawn;
        while ( Time.realtimeSinceStartup < endTime )
        {
            yield return new WaitForSeconds( .5f );
        }
        KillPlane( obj );
    }
    
    /*
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
    */
}
