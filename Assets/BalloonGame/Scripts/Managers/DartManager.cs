/*
 * Internal explanation: Internally, the darts are never destroyed unless the method DestroyDarts 
 * is called. This is most easily understood by comparing what is done here with a tag team 
 * wrestling match. There are a pair of left darts and a pair of right darts. Whenever the 
 * DespawnDart method is called the passed in dart is tagged out with its dart partner (transferred
 * outside the view of the user and its dart partner is placed at the respective dart spawner).
 *
 * Why not just destroy the darts and instantiate another one in its place? We had a problem where 
 * occasionaly the dart would get destroyed and the OVRGrabber would still be referencing the old,
 * destroyed object preventing the user from picking up another dart unless the game was restarted. 
 * The OVRGrabber provides some methods for forcing the release of an item, but using their 
 * provided methods presented some difficulties, so the above method was chosen. Also, you could
 * argue that this method is more efficient since we avoid repeatedly instantiating and destroying 
 * objects. 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * The DartManager class handles the spawning and despawning of darts in the scene.
 */
public class DartManager : MonoBehaviour
{
    /* Singleton pattern. Holds a reference to the dart manager object. */
    public static DartManager Instance {get; private set;}
    
    [SerializeField] private GameObject leftDartSpawn;
    [SerializeField] private GameObject rightDartSpawn;
    [SerializeField] private GameObject dartPrefab;
    [SerializeField] private GameObject hiddenDartLoc; /* Where "tagged" out darts will be placed. */

    private GameObject                  leftDart;  /* Left dart pair */
    private GameObject                  leftDart2;

    private GameObject                  rightDart;  /* Right dart pair */
    private GameObject                  rightDart2; 

    private void Awake()
    {
        /* Singleton pattern make sure there is only one dart manager. */
		if (Instance != null && Instance != this) {
			//Debug.LogError("There should only be one balloon manager.");
            Destroy(this);
		} else {
            Instance = this;
        } 
    }
    
    private void Start()
    {
        this.leftDart   = Instantiate(dartPrefab);
        this.leftDart2  = Instantiate(dartPrefab);
        this.rightDart  = Instantiate(dartPrefab);
        this.rightDart2 = Instantiate(dartPrefab);

        this.leftDart.transform.position   = this.leftDartSpawn.transform.position;
        this.leftDart2.transform.position  = this.hiddenDartLoc.transform.position;
        this.rightDart.transform.position  = this.rightDartSpawn.transform.position;
        this.rightDart2.transform.position = this.hiddenDartLoc.transform.position;
    }

    /**
     * Despawns the dart and automatically spawns another dart in the appropriate location depending 
     * on whether the passed in dart is the left or right dart. 
     *
     * @param dart The dart to despawn.
     */
    public void DespawnDart(GameObject dart)
    {
        Debug.Assert(this.IsLeftDart(dart) || this.IsRightDart(dart));
        /* For debugging purposes. */
        string dartStr = (this.IsLeftDart(dart) ? "left" : "right");
        Debug.Log("Despawned " + dartStr + " dart.");

        GameObject dartPartner;
        GameObject dartSpawn;
        
        /* Get the appropriate partner and dart spawn. */
        if (this.IsLeftDart(dart)) {
            dartPartner = (dart == this.leftDart ? this.leftDart2 : this.leftDart);
            dartSpawn   = leftDartSpawn;
        } else {
            dartPartner = (dart == this.rightDart ? this.rightDart2 : this.rightDart);
            dartSpawn   = rightDartSpawn;
        }

        /* Tag the partner out. */
        dart.transform.position = this.hiddenDartLoc.transform.position;
        dartPartner.transform.SetPositionAndRotation(dartSpawn.transform.position, dartSpawn.transform.rotation);
    }

    /**
     * Checks to see if the dart is the left dart.
     * 
     * @param dart The dart to be checked.
     */
    public bool IsLeftDart(GameObject dart) 
    {
        return (dart == this.leftDart || dart == this.leftDart2);
    }

    /**
     * Checks to see if the dart is the right dart.
     *
     * @param dart The dart to be checked.
     */
    public bool IsRightDart(GameObject dart)
    {
        return (dart == this.rightDart || dart == this.rightDart2);
    }

    /**
     * Destroys all of the darts in the scene.
     */
    public void DestroyDarts()
    {
        Destroy(this.leftDart);
        Destroy(this.leftDart2);
        Destroy(this.rightDart);
        Destroy(this.rightDart2);
    }

    /**
     * The AdjustLeftSpawn adjusts the left spawn of the darts using the given offsets.
     *
     * @param x The offset in the x direction.
     * @param y The offset in the y direction.
     * @param z The offset in the z direction.
     */
    public void AdjustLeftSpawn(float x, float y, float z)
    {
        Utils.AdjustPosition(this.leftDartSpawn, x, y, z);
        Utils.AdjustPosition(this.leftDart, x, y, z);
        Utils.AdjustPosition(this.leftDart2, x, y, z);
    }

    /**
     * The AdjustRightSpawn adjusts the right spawn of the darts using the given offsets.
     *
     * @param x The offset in the x direction.
     * @param y The offset in the y direction.
     * @param z The offset in the z direction.
     */
    public void AdjustRightSpawn(float x, float y, float z)
    {
        Utils.AdjustPosition(this.rightDartSpawn, x, y, z);
        Utils.AdjustPosition(this.rightDart, x, y, z);
        Utils.AdjustPosition(this.rightDart2, x, y, z);
    }

    /**
     * The AdjustLeftSpawn adjusts the left and right spawns of the darts using the given offsets.
     *
     * @param x The offset in the x direction.
     * @param y The offset in the y direction.
     * @param z The offset in the z direction.
     */
    public void AdjustBothSpawns(float x, float y, float z)
    {
        AdjustLeftSpawn(x, y, z);
        AdjustRightSpawn(x, y, z);
    }

    private void OnDestroy()
    {
        Destroy(this.leftDart);
        Destroy(this.leftDart2);
        Destroy(this.rightDart);
        Destroy(this.rightDart2);
    }
}
