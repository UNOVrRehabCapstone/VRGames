using Classes.Managers;
using UnityEngine;

public class DartRespawn : MonoBehaviour
{
    private BalloonGameplayManager manager;
    [SerializeField] private GameObject dartPrefab;
    private static bool leftDartSpawned;
    private static bool rightDartSpawned;
    

    void Start() 
    {
        manager = BalloonGameplayManager.Instance;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("LeftGrabber") && leftDartSpawned == false && gameObject.CompareTag("YellowDartSpawn"))
        {
            leftDartSpawned = true;
            this.SpawnDart(gameObject);
        }
        else if (other.gameObject.CompareTag("RightGrabber") && rightDartSpawned == false && gameObject.CompareTag("BlueDartSpawn"))
        {
            rightDartSpawned = true;
            this.SpawnDart(gameObject);
        }
    }

    //For Testing purposes only (to be deleted when finished)
    void OnMouseDown()
    {
        this.SpawnDart(gameObject);
    }

    // public static void disableDart()
    // {
    //     if (leftDartSpawned)
    //     {
    //         leftDartSpawned = false;
    //     }
    //     if (rightDartSpawned)
    //     {
    //         rightDartSpawned = false;
    //     }
    // }

    public void SpawnDart(GameObject dartSpawn)
    {
        GameObject temp = Instantiate(dartPrefab);
        MeshRenderer wing = temp.gameObject.GetComponentInChildren<MeshRenderer>();
        foreach (Transform child in temp.transform)
        { 
            if (child.CompareTag("DartColorMatch"))
            {
                wing = child.GetComponent<MeshRenderer>();
                if (dartSpawn.gameObject.CompareTag("YellowDartSpawn"))
                {
                    wing.material.color = Color.yellow;
                }
                else if (dartSpawn.gameObject.CompareTag("BlueDartSpawn"))
                {
                    wing.material.color = Color.blue;
                }
            }
        }
        temp.transform.position = dartSpawn.transform.position + new Vector3(0, 0, -.06f);
    }

    public static Vector3 GetLeftSpawnPosition()
    {
        return GameObject.FindGameObjectWithTag("YellowDartSpawn").transform.position;
    }

    public static Vector3 GetRightSpawnPosition()
    {
        return GameObject.FindGameObjectWithTag("BlueDartSpawn").transform.position;
    }

    public static void DisableLeftDart()
    {
        leftDartSpawned = false;
    }

    public static void DisableRightDart()
    {
        rightDartSpawned = false;
    }
}