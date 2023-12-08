using System.Collections;
using System.Collections.Generic;
using Classes.Managers;
using UnityEngine;

/**
 * The BalloonKillZone is a class that handles the logic for when a balloon collides with the kill 
 * zone. 
 */
public class BalloonKillZone : MonoBehaviour
{
    private OVRCameraRig cameraRig;
    
    private void Start()
    {
        cameraRig = OVRCameraRig.FindObjectOfType<OVRCameraRig>();
    }

    private void Update()
    {
        // Debug.Log(cameraRig.centerEyeAnchor.position.y);
        this.transform.position = new Vector3 (this.transform.position.x, cameraRig.centerEyeAnchor.position.y + 1.5f, this.transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other + " collided with the kill zone.");

        /* This switch differentiates between the different types of balloons. The reason for this 
           is that some balloons need to be handled differently when colliding with the kill zone.
           The default case ignores all other objects, so make sure that when you create a balloon 
           you tag it and add a case for it or else it won't be destroyed when it collides with the 
           kill zone. 

           TODO: It may be better just to put these in each of the different balloon classes. */
        switch(other.gameObject.tag) {
            case "Balloon":
                --BalloonGameplayManager.Instance.playerLives;
                PointsManager.updateScoreboard();
                BalloonManager.Instance.KillBalloon(other.gameObject);
                Debug.Log("Lost a life. Remaining lives: " + BalloonGameplayManager.Instance.playerLives);
                break;
            case "OnionLayer1":
            case "OnionLayer2":
            case "OnionLayer3":
                --BalloonGameplayManager.Instance.playerLives;
                BalloonManager.Instance.KillBalloon(other.gameObject.transform.parent.gameObject);
                break;
            case "SpawnStreamMember":
            case "SpawnStreamMemberLast":
                --BalloonGameplayManager.Instance.playerLives;
                BalloonManager.Instance.KillBalloonDelay(other.gameObject.transform.parent.gameObject, 2);
                break;
            case "Balloon_Stream_Powerup":
                --BalloonGameplayManager.Instance.playerLives;
                BalloonManager.Instance.KillBalloon(other.gameObject);
                break;
            case "RestoreLife":
                --BalloonGameplayManager.Instance.playerLives;
                BalloonManager.Instance.KillBalloon(other.gameObject);
                break;
            case "Target":
                --BalloonGameplayManager.Instance.playerLives;
                BalloonManager.Instance.KillBalloon(other.gameObject.transform.parent.parent.gameObject);
                break;
            default:
                Debug.Log(  "No tag match. If this is a balloon, make sure to tag the balloon, and "
                          + "add a case for it in the BalloonKillZone class.");
                break;
        }
        PointsManager.updateScoreboard();
    }
}
