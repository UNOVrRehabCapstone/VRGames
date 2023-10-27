using System.Collections;
using System.Collections.Generic;
using Classes.Managers;
using UnityEngine;

public class Balloon_OnionLayer : MonoBehaviour
{
    [SerializeField] private int pointValue;

	public virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("DartPoint"))
        {
            this.PlayEffects();

            /* If the final layer is popped, make sure to remove the parent balloon from the scene. */
            if (gameObject.CompareTag("Balloon_OnionLayer3")) {
                BalloonManager.Instance.KillBalloon(gameObject.transform.parent.gameObject);
            }
            Destroy(gameObject);
            DartManager.Instance.DestroyDart(other.gameObject.transform.parent.gameObject);
            PointsManager.addPoints(this.pointValue);
        }
    }

    public virtual void PlayEffects()
    {
        this.PlaySound();
        this.PlayParticles();
    }

    private void PlaySound()
    {
        GameObject audioSource = this.transform.Find("AudioSource").gameObject;
        /* Decouple the child object from the parent to avoid destroying the parent (along with
           the child audio object) before the audio is done playing. */
        audioSource.transform.parent = null;
        audioSource.GetComponent<AudioSource>().Play();
        /* Then, make sure to destroy the audio object, but with a delay. */
        Destroy(audioSource, audioSource.GetComponent<AudioSource>().clip.length);
    }

    private void PlayParticles()
    {
        GameObject particleEffect = this.transform.Find("ParticleEffects").gameObject;
        particleEffect.transform.parent = null;
        particleEffect.GetComponent<ParticleSystem>().Play();
        Destroy(particleEffect, particleEffect.GetComponent<ParticleSystem>().main.duration);
    }

    private void AddPoints()
    {
        GameObject onion = gameObject.transform.parent.gameObject;
        if (onion.GetComponent<_BaseBalloon>().spawnLocation.CompareTag("BalloonSpawn_Left")) {
            PointsManager.addLeftPoints(this.pointValue);
        } else {
            PointsManager.addRightPoints(this.pointValue);
        }
        PointsManager.addPoints(this.pointValue);

        Debug.Log(  "Left points: " + PointsManager.getLeftPoints() 
                  + ". Right points: " + PointsManager.getRightPoints() 
                  + ". Total points: " + PointsManager.getPoints() + ".");
    }

    /* For testing purposes. Useful for testing on the computer rather than in the headset. */
    public virtual void OnMouseDown()
    {
        Debug.Log(this.ToString() + " popped. Worth " + this.pointValue + " points.");
        
        this.PlayEffects();
        this.AddPoints();
        
        /* If the final layer is popped, make sure to remove the parent balloon from the scene. */
        if (gameObject.CompareTag("Balloon_OnionLayer3")) {
            BalloonManager.Instance.KillBalloon(gameObject.transform.parent.gameObject);
        }
        Destroy(gameObject);
    }
}
