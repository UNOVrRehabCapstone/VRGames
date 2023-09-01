using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayScore : MonoBehaviour {

    public GameObject achievements;
    public GameObject score;

    public Text leftHighScore;
    public Text rightHighScore;
    public Text data;

    public void Start()
    {
        
        leftHighScore = GameObject.Find("LeftScore").GetComponent<Text>();
        rightHighScore = GameObject.Find("RightScore").GetComponent<Text>();
        //data = GameObject.Find("DataText").GetComponent<Text>();
        
    }

public void updateText(int maxScoreLeft, int maxScoreRight)
    {
        Achievements a = achievements.GetComponent<Achievements>();
        //a.loadResults();
        leftHighScore.text = "Top Score: " + maxScoreLeft;
        rightHighScore.text = "Top Score: " + maxScoreRight;
        data.text = "\n";
        for (int i = 0; i < a.achievementList.Length; i++)
        {
            string s = a.achievementList[i];
            Debug.Log("COMPLETED: " + a.completed[i]);
            if (a.completed[i] == 0)
            {
                data.text += "• " + s + "\n";
            }
            else
            {
                data.text += "• " + StrikeThrough(s) + "\n";
            }

        }


    }
    public string StrikeThrough(string s)
    {
        string strikethrough = "";
        foreach (char c in s)
        {
            strikethrough = strikethrough + c + '\u0336';
        }
        return strikethrough;
    }

}
