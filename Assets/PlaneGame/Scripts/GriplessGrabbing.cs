using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GriplessGrabbing : MonoBehaviour
{
    public Transform handTransform; // Assign the player's hand transform in the inspector

    private void OnTriggerEnter(Collider other)
    {
        // Print the tag of the colliding object
        Debug.Log("Collided with object of tag: " + other.tag);

        if (other.CompareTag("RightGrabber")) {
            handTransform = handTransform = GameObject.FindGameObjectWithTag("RightGrabber").transform;
        } else if (other.CompareTag("LeftGrabber")) {
            handTransform = handTransform = GameObject.FindGameObjectWithTag("LeftGrabber").transform;
        }

        // Check if the colliding object has the tag "PlayerHand"
        if (other.CompareTag("RightGrabber") || other.CompareTag("LeftGrabber"))
        {
            // Parent the airplane to the hand
            this.transform.SetParent(handTransform);

            // Set local position and rotation to zero
            // Adjust these values to make the airplane appear correctly in the hand
            this.transform.localPosition = Vector3.zero;
            this.transform.localRotation = Quaternion.identity;

            // Optionally disable physics if needed
            Rigidbody rb = GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
            }
        }
    }
}