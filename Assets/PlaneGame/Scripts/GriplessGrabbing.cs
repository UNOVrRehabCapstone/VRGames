/**
 * \file GriplessGrabbing.cs
 * \brief This script enables objects to be grabbed and attached to a specified hand transform without using physics-based grip mechanisms.
 *
 * The GriplessGrabbing class allows for the detection of collisions with hand objects, enabling the attaching of the colliding object to the specified hand. It provides functionality to switch between right and left hand grabbing based on the collision detected.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * \class GriplessGrabbing
 * \brief Handles the gripless grabbing and attaching of objects to a hand transform.
 *
 * This class is responsible for detecting when the object it is attached to collides with a hand grabber object. When a collision is detected with either a left or right hand grabber, it parents the object to the corresponding hand's transform.
 */
public class GriplessGrabbing : MonoBehaviour
{
    /**
     * \brief Transform of the player's hand to which objects will be attached.
     * 
     * This should be assigned in the inspector with the hand transform to which grabbed objects will be parented.
     */
    public Transform handTransform;

    /**
     * \brief Called when an object enters the trigger collider.
     * 
     * This method checks the tag of the colliding object. If the object is tagged as a hand grabber (either left or right),
     * it sets the handTransform to the colliding object's transform and parents the current object to this hand transform,
     * effectively grabbing it.
     * 
     * \param other The collider that this object has collided with.
     */
    private void OnTriggerEnter(Collider other)
    {
        // Print the tag of the colliding object
        Debug.Log("Collided with object of tag: " + other.tag);

        // Set the handTransform based on whether the colliding object is a right or left grabber
        if (other.CompareTag("RightGrabber")) {
            handTransform = GameObject.FindGameObjectWithTag("RightGrabber").transform;
        } else if (other.CompareTag("LeftGrabber")) {
            handTransform = GameObject.FindGameObjectWithTag("LeftGrabber").transform;
        }

        // If the object is a hand grabber, parent this object to the handTransform
        if (other.CompareTag("RightGrabber") || other.CompareTag("LeftGrabber"))
        {
            // Parent the object to the hand
            this.transform.SetParent(handTransform);

            // Set local position and rotation to zero (adjust as needed for correct appearance in hand)
            this.transform.localPosition = Vector3.zero;
            this.transform.localRotation = Quaternion.identity;

            // Optionally disable physics to avoid unwanted physics interactions after grabbing
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }
        }
    }
}
