using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneSound : MonoBehaviour
{

    public AudioSource audioSource;

    public AudioClip[] audioClipArray;


    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSource.clip = audioClipArray[Random.Range(0, audioClipArray.Length)];
    }

    public void PlayThrowSound()
    {
        audioSource.PlayOneShot(audioSource.clip);
    }
}
