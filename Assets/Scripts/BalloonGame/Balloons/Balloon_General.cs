using System.Collections;
using System.Collections.Generic;
using Classes.Managers;
using UnityEngine;

public class Balloon_General : _BaseBalloon
{
    private void Update()
    {
        Transform transform = gameObject.transform;

        if (SpecialBalloonManager.Instance.slowBalloonActivated) {
            /* Cut the float speed in half. */
            transform.position = Vector3.Lerp(transform.position, transform.position 
                                           + new Vector3(0f, 1f, 0f), Time.deltaTime * floatStrength * 0.5f);
        } else {
            transform.position = Vector3.Lerp(transform.position, transform.position 
                                           + new Vector3(0f, 1f, 0f), Time.deltaTime * floatStrength);
        }
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("DartPoint") && this.IsCorrectDart(other.gameObject.transform.parent.gameObject))
        {
            this.PlayEffects();
            this.AddPoints();
            BalloonManager.Instance.KillBalloon(gameObject);
            DartManager.Instance.DestroyDart(other.gameObject.transform.parent.gameObject);
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
        if (this.spawnLocation.CompareTag("BalloonSpawn_Left")) {
            PointsManager.addLeftPoints(this.pointValue);
        } else {
            PointsManager.addRightPoints(this.pointValue);
        }
        PointsManager.addPoints(this.pointValue);

        Debug.Log(  "Left points: " + PointsManager.getLeftPoints() 
                  + ". Right points: " + PointsManager.getRightPoints() 
                  + ". Total points: " + PointsManager.getPoints() + ".");
    }

    private bool IsCorrectDart(GameObject dart)
    {
        return 
        
            (this.spawnLocation.CompareTag("BalloonSpawn_Left")  && DartManager.Instance.IsLeftDart(dart)) 
         || (this.spawnLocation.CompareTag("BalloonSpawn_Right") && DartManager.Instance.IsRightDart(dart));
    }

    /* For testing purposes. Useful for testing on the computer rather than in the headset. */
    public virtual void OnMouseDown()
    {
        Debug.Log(this.ToString() + " popped. Worth " + this.pointValue + " points.");
        
        this.PlayEffects();
        this.AddPoints();

        BalloonManager.Instance.KillBalloon(gameObject);
    }
}
