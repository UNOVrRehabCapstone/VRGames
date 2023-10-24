using System.Collections;
using System.Collections.Generic;
using Classes.Managers;
using UnityEngine;

public class BalloonKillZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        /* Note that what I am doing here is slightly dangerous. I am assuming the other game 
           object is a balloon. This should not be a problem since where the collider is positioned,
           the only objects that should be colliding with the kill zone are balloons.*/
        Debug.Log(other + " collided with the kill zone.");

        if (other.gameObject.CompareTag("Balloon_General")) {
            /* Lose a life only for the general balloon. */
            --BalloonGameplayManager.Instance.playerLives;
            Debug.Log("Lost a life. Remaining lives: " + BalloonGameplayManager.Instance.playerLives);
        }

        BalloonManager.Instance.KillBalloon(other.gameObject);
    }
}
