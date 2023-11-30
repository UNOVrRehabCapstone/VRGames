using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementPopup : MonoBehaviour
{
    //achievement popup display duration
    public float displayDuration = 2f;



    public void ShowPopUp(string nameOfAchievement,string descriptionOfAchievement)
    {

        Invoke("HidePopup", displayDuration);

    }
    private void HidePopup()
    {
        // Hide the popup
        gameObject.SetActive(false);
    }
}
