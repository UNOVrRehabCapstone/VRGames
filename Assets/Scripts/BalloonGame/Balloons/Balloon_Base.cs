using System.Collections;
using System.Collections.Generic;
using Classes.Managers;
using UnityEngine;

public class Balloon_Base : MonoBehaviour
{
    [SerializeField] private float floatStrength;

    private void Update()
    {
        transform.position = Vector3.Lerp(transform.position, transform.position 
                                                              + new Vector3(0f, 1f, 0f), Time.deltaTime * floatStrength);
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("DartPoint"))
        {
            /* There are more efficient ways of doing this (notice that every time a balloon is 
               popped we end up calling the destroy method 3 times!), but considering this game
               is fairly small, I don't think this will be a huge performance hit. We can always 
               optimize later if needed */

            GameObject audioSource = this.transform.Find("AudioSource").gameObject;
            /* Decouple the child object from the parent to avoid destroying the parent (along with
               the child audio object) before the audio is done playing. */
            audioSource.transform.parent = null;
            audioSource.GetComponent<AudioSource>().Play();
            /* Then, make sure to destroy the audio object, but with a delay. */
            Destroy(audioSource, audioSource.GetComponent<AudioSource>().clip.length);

            /* Same thing as above, but with the particle effects. */
            GameObject particleEffect = this.transform.Find("ParticleEffects").gameObject;
            particleEffect.transform.parent = null;
            particleEffect.GetComponent<ParticleSystem>().Play();
            Destroy(particleEffect, particleEffect.GetComponent<ParticleSystem>().main.duration); 

            Debug.Log("Balloon hit!");
            BalloonManager.Instance.KillBalloon(gameObject);

            DartManager.Instance.DestroyDart(other.gameObject.transform.parent.gameObject);

            PointsManager.addPoints(1);
            //ShowScorePopup();
        }
    }

    /* For testing purposes. Useful for testing on the computer rather than in the headset. */
    public virtual void OnMouseDown()
    {
        GameObject audioSource = this.transform.Find("AudioSource").gameObject;
        audioSource.transform.parent = null;
        audioSource.GetComponent<AudioSource>().Play();
        Destroy(audioSource, audioSource.GetComponent<AudioSource>().clip.length);

        GameObject particleEffect = this.transform.Find("ParticleEffects").gameObject;
        particleEffect.transform.parent = null;
        particleEffect.GetComponent<ParticleSystem>().Play();
        Destroy(particleEffect, particleEffect.GetComponent<ParticleSystem>().main.duration);
        
        BalloonManager.Instance.KillBalloon(gameObject);
        PointsManager.addPoints(1);
        Debug.Log("Total points = " + PointsManager.getPoints());
    }
}
