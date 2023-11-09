/**
 * The BalloonKillZone is a class that handles the logic for when a balloon collides with the kill 
 * zone. 
 */

using System.Collections;
using System.Collections.Generic;
using Classes.Managers;
using UnityEngine;

public class BalloonKillZone : MonoBehaviour
{
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
                BalloonManager.Instance.KillBalloon(other.gameObject.transform.parent.gameObject);
                break;
            case "SpawnStreamMember":
            case "SpawnStreamMemberLast":
                BalloonManager.Instance.KillBalloonDelay(other.gameObject.transform.parent.gameObject, 2);
                break;
            case "Balloon_Stream_Powerup":
                BalloonManager.Instance.KillBalloon(other.gameObject);
                break;
            default:
                Debug.Log(  "No tag match. If this is a balloon, make sure to tag the balloon, and "
                          + "add a case for it in the BalloonKillZone class.");
                break;
        }
    }
}
