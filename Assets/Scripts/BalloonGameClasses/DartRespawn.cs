using Classes.Managers;
using UnityEngine;

public class DartRespawn : MonoBehaviour
{
    private BalloonGameplayManager manager;
    private static bool leftDartSpawned;
    private static bool rightDartSpawned;
    

    void Start() 
    {
        manager = (BalloonGameplayManager) GameplayManager.getManager();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("LeftGrabber") && leftDartSpawned == false && gameObject.CompareTag("YellowDartSpawn"))
        {
            leftDartSpawned = true;
            manager.SpawnDart(gameObject);
        }
        else if (other.gameObject.CompareTag("RightGrabber") && rightDartSpawned == false && gameObject.CompareTag("BlueDartSpawn"))
        {
            rightDartSpawned = true;
            manager.SpawnDart(gameObject);
        }
    }

    public static void disableDart()
    {
        if (leftDartSpawned)
        {
            leftDartSpawned = false;
        }
        if (rightDartSpawned)
        {
            rightDartSpawned = false;
        }
    }
}