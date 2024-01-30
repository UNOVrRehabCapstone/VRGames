using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Achievements : MonoBehaviour
{
    public Image medal;
    public Text txt;
    public string[] achievementList =
    {
        "Get exactly 10 blocks",
        "Get No blocks!",
        "Get only blue blocks"
    };

    public int blue;
    public int red;
    public int green;

    public int[] completed =
    {
        0,
        0,
        0
    };



    void Start()
    {
        red = 0;
        green = 0;
        blue = 0;
       // PlayerPrefs.SetInt("CompletedOne", 0);
       // PlayerPrefs.SetInt("CompletedTwo", 0);
        //
       // PlayerPrefs.SetInt("CompletedThree", 0);
        loadResults();
    }
    public void checkAllAchievements(int count)
    {
        Debug.Log(blue);
        for (int i = 0 ; i < achievementList.Length; i++)
        {
            checkAchievement(i, count);
        }
        checkCompleted();

    }


    public void checkCompleted()
    {
        int all = 0;
        for (int i = 0; i < achievementList.Length; i++)
        {
            if (completed[i] == 1)
                all++;
        }
        if (all == completed.Length)
        {
            Debug.Log("CONGRATS");
            medal.gameObject.SetActive(true);
            txt.gameObject.SetActive(false);
        }
        else
        {
            medal.gameObject.SetActive(false);
            txt.gameObject.SetActive(true);
        }

    }
    

    public void checkAchievement(int index, int count)
    {

        switch (index)
        {
            case 0:
                if (count == 10)
                {
                    completed[0] = 1;

                }
                break;
            case 1:
                if (count == 0)
                {
                    completed[1] = 1;

                }
                    
                break;
            case 2:
                if (count == blue && count > 0)
                {
                    completed[2] = 1;
                }
                break;
            
        }
        saveResults(); 

    }
    public void saveResults()
    {
        PlayerPrefs.SetInt("CompletedOne", completed[0]);
        PlayerPrefs.SetInt("CompletedTwo", completed[1]);
        PlayerPrefs.SetInt("CompletedThree", completed[2]);


        PlayerPrefs.Save();
    }
    public void loadResults()
    {
        completed[0] = PlayerPrefs.GetInt("CompletedOne");
        completed[1] = PlayerPrefs.GetInt("CompletedTwo");
        completed[2] = PlayerPrefs.GetInt("CompletedThree");
        checkCompleted();
        //completed[PlayerPrefs.GetInt("CompletedIndex")] = PlayerPrefs.GetInt("CompletedValue");

    }

}
