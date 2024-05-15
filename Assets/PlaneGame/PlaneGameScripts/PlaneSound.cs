/**
 * \file PlaneSound.cs
 * \brief Manages plane-related sound effects.
 *
 * The PlaneSound class handles the audio aspects related to planes in the game, including the setup of audio sources and playing throw sounds.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PlanesGame{
  /**
   * \class PlaneSound
   * \brief Manages plane-related sound effects.
   *
   * The PlaneSound class handles the audio aspects related to planes in the game, including the setup of audio sources and playing throw sounds.
   */
  public class PlaneSound : MonoBehaviour
{

    public AudioSource audioSource;

    public AudioClip[] audioClipArray;

    /**
     * \brief Initializes the audio source component.
     */
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    /**
     * \brief Selects a random audio clip from the array and assigns it to the audio source.
     * 
     * Called before the first frame update.
     */
    // Start is called before the first frame update
    void Start()
    {
        audioSource.clip = audioClipArray[Random.Range(0, audioClipArray.Length)];
    }

    /**
     * \brief Plays a throw sound effect.
     * 
     * Plays a random audio clip from the assigned audio source.
     */
    public void PlayThrowSound()
    {
        audioSource.PlayOneShot(audioSource.clip);
    }
}}
