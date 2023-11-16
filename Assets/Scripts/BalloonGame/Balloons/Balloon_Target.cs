using System.Collections;
using System.Collections.Generic;
using Classes.Managers;
using UnityEngine;

public class Balloon_Target : Balloon
{
	public override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("DartPoint") && this.IsCorrectDart(other.gameObject.transform.parent.gameObject))
        {
            this.PlayEffects();
            this.AddPoints();
            DartManager.Instance.DespawnDart(other.gameObject.transform.parent.gameObject);
        }
    }

    public override void PlayEffects()
    {
        this.PlaySound();
        this.PlayParticles();
    }

    private void PlaySound()
    {
        GameObject audioSource = this.transform.Find("AudioSource").gameObject;
        audioSource.GetComponent<AudioSource>().Play();
    }

    private void PlayParticles()
    {
        GameObject particleEffect = this.transform.Find("ParticleEffects").gameObject;
        particleEffect.GetComponent<ParticleSystem>().Play();
    }
}
