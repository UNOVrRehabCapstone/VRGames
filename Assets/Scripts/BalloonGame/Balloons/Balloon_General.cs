using System.Collections;
using System.Collections.Generic;
using Classes.Managers;
using UnityEngine;

public class Balloon_General : MonoBehaviour
{
    [SerializeField] private float floatStrength;
    [SerializeField] private int      pointValue;
    

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
        if (other.gameObject.CompareTag("DartPoint"))
        {
            this.PlayEffects();
            BalloonManager.Instance.KillBalloon(gameObject);
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

    /* For testing purposes. Useful for testing on the computer rather than in the headset. */
    public virtual void OnMouseDown()
    {
        Debug.Log(this.ToString() + " popped. Worth " + this.pointValue + " points.");
        
        this.PlayEffects();
        BalloonManager.Instance.KillBalloon(gameObject);
        PointsManager.addPoints(this.pointValue);

        Debug.Log("Total points = " + PointsManager.getPoints());
    }
}
