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
    }
    
    private void Start()
    {
        this.SpawnDart(leftDart);
        this.SpawnDart(rightDart);
    }

    public void DestroyDart(GameObject dart)
    {
        this.SpawnDart(dart);
        Destroy(dart);
    }

    private void SpawnDart(GameObject dart)
    {
        if (dart == this.leftDart) {
            this.leftDart = Instantiate(dartPrefab);
            this.leftDart.transform.position = leftDartSpawn;
        } else if (dart == this.rightDart) {
            this.rightDart = Instantiate(dartPrefab);
            this.rightDart.transform.position = rightDartSpawn;
        }
    }
}
