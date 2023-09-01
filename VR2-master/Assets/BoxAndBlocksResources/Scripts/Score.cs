using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour {

    public int maxScoreLeft;
    public int maxScoreRight;

    public Text leftHighScore;
    public Text rightHighScore;
    public Text data;

     public GameObject achievements;
     public GameObject ds;

    [SerializeField]
    private bool debugMode = true;

    void Start()
    {
        PlayerPrefs.SetInt("maxScoreLeft", 0);
        //achievements = GetComponent<Achievements>();
        //ds = GetComponent<DisplayScore>();
        //Debug.Log(achievements.gameObject.name);
        leftHighScore = leftHighScore.GetComponent<Text>();
        rightHighScore = rightHighScore.GetComponent<Text>();
        data = data.GetComponent<Text>();
        Load();
    }
	
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && debugMode == true)
        {
            PlayerPrefs.SetInt("maxScoreLeft", 0);
            PlayerPrefs.SetInt("maxScoreRight", 0);
            PlayerPrefs.SetInt("CompletedOne", 0);
            PlayerPrefs.SetInt("CompletedTwo", 0);
            PlayerPrefs.SetInt("CompletedThree", 0);
            GameObject.Find("Achievements").GetComponent<Achievements>().loadResults();
            GameObject.Find("Achievements").GetComponent<Achievements>().checkCompleted();
            Load();
        }
    }
     public void Save()
    {
        if (!PlayerPrefs.HasKey("maxScoreLeft"))
        {
            PlayerPrefs.SetInt("maxScoreLeft", this.maxScoreLeft); 
        }
        else
        {
            int prevLeft = PlayerPrefs.GetInt("maxScoreLeft");
            Debug.Log(this.maxScoreLeft + " > " + prevLeft);
            if (this.maxScoreLeft > prevLeft)
            {
                PlayerPrefs.SetInt("maxScoreLeft", this.maxScoreLeft);
            }
        }

        if (!PlayerPrefs.HasKey("maxScoreRight"))
        {
            PlayerPrefs.SetInt("maxScoreRight", this.maxScoreRight);
        }
        else
        {
            int prevRight = PlayerPrefs.GetInt("maxScoreRight");
            if (this.maxScoreRight > prevRight)
            {
                PlayerPrefs.SetInt("maxScoreRight", this.maxScoreRight);
            }
        }

        PlayerPrefs.Save();
        Load();
    }

    public void Load()
    {
        
        if (PlayerPrefs.HasKey("maxScoreLeft"))
        {
            this.maxScoreLeft = PlayerPrefs.GetInt("maxScoreLeft");
        }
        else
        {
            this.maxScoreLeft = 0;
        }
        if (PlayerPrefs.HasKey("maxScoreRight"))
        {     
            this.maxScoreRight = PlayerPrefs.GetInt("maxScoreRight");
        }
        else
        {
            this.maxScoreRight = 0;
        }
        //Debug.Log(achievements.gameObject.name);
        Achievements a = achievements.GetComponent<Achievements>();
        DisplayScore d = ds.GetComponent<DisplayScore>();
        a.loadResults();
        d.updateText(this.maxScoreLeft, this.maxScoreRight);
    }
   
}
