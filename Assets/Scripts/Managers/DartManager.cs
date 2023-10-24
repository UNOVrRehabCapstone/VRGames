/**
 * The DartManager class handles the spawning and despawning of darts in the scene.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartManager : MonoBehaviour
{
    /* Singleton pattern. Holds a reference to the dart manager object. */
    public static DartManager Instance {get; private set;}
    
    [SerializeField] private Vector3    leftDartSpawn;
    [SerializeField] private Vector3    rightDartSpawn;
    [SerializeField] private GameObject dartPrefab;
    private GameObject                  leftDart;
    private GameObject                  rightDart;

    private void Awake()
    {
        /* Singleton pattern make sure there is only one dart manager. */
		if (Instance != null) {
			Debug.LogError("There should only be one balloon manager.");
		}
		Instance = this; 

        Debug.Log("Dart manager actived.");
    }
    
    private void Start()
    {
        this.SpawnDart(leftDart);
        this.SpawnDart(rightDart);
    }

    /**
     * Destroys the dart and automatically spawns another dart in the appropriate location depending 
     * on whether the passed in dart is the left or right dart. 
     */
    public void DestroyDart(GameObject dart)
    {
        /* For debugging purposes. */
        string dartStr = (dart == leftDart ? "left" : "right");

        /* Order matters here */
        this.SpawnDart(dart);
        Debug.Log("Destroyed " + dartStr + " dart.");
        Destroy(dart);
    }

    /**
     * Spawns a dart. Either spawns a dart at the left or right spawn, depending on whether the 
     * passed in object is the left or right dart. Intended to be used in conjuction with the 
     * DestroyDart method.
     */
    private void SpawnDart(GameObject dart)
    {
        if (dart == this.leftDart) {
            Debug.Log("Left dart spawned.");
            this.leftDart = Instantiate(dartPrefab);
            this.leftDart.transform.position = leftDartSpawn;
        } else if (dart == this.rightDart) {
            Debug.Log("Right dart spawned.");
            this.rightDart = Instantiate(dartPrefab);
            this.rightDart.transform.position = rightDartSpawn;
        }
    }
}
