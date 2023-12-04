using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementPopup : MonoBehaviour
{
    public float floatSpeed = 1.0f; // Adjust the speed of floating
    public float hoverTime = 3.0f; // Adjust the time the object hovers in seconds

    private float initialY;
    private float elapsedTime = 0.0f;


    void Start()
    {
        initialY = transform.position.y;
    }

    void Update()
    {
        // Move the object upwards
        transform.Translate(Vector3.up * floatSpeed * Time.deltaTime);

        // Check if the object has reached the desired height
        if (transform.position.y - initialY >= 2.0f) // Adjust the distance to hover
        {
            // Increment the elapsed time
            elapsedTime += Time.deltaTime;

            // Check if the object has hovered for the specified time
            if (elapsedTime >= hoverTime)
            {
                // Remove the object
                Destroy(gameObject);
            }
        }

    }



    public void ShowPopUp(string nameOfAchievement,string descriptionOfAchievement)
    {

       // Invoke("HidePopup", displayDuration);

    }
    private void HidePopup()
    {
        // Hide the popup
        gameObject.SetActive(false);
    }
}
