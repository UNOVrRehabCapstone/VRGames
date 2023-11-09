using Classes.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Responsible for calling functions related to points.
//Useful for games where points are used for scoring instead of time.
public class PointsManager : MonoBehaviour
{
    public bool useSeparateScores;
    private static int points;
    private static int leftPoints;
    private static int rightPoints;
    private static List<PointTrigger> pointTriggers = new List<PointTrigger>();
    private static GameObject scoreboard;

 

    void Start(){
        scoreboard = GameObject.FindGameObjectWithTag("Scoreboard");
        
        if (!useSeparateScores)
        {
            resetPoints();
        }
        else
        {
            resetBothHandPoints();
        }
        
    }

    public static int getLeftPoints() { return leftPoints; }
    public static int getRightPoints() { return rightPoints; }

    public static int getPoints() { return points; }
    public static void resetPoints() {
        points = 0;
        checkPoints();
        updateScoreboard();
      
    }

    public static void resetBothHandPoints()
    {
        leftPoints = 0;
        rightPoints = 0;
        checkPoints();
        updateLeftScore();
        updateRightScore();
        updateLivesDisplay();
    }
    public static void addPoints(int p) {
        points += p;
        checkPoints();
        updateScoreboard();
    } 
    public static void addLeftPoints(int p)
    { 
        leftPoints += p;
        checkPoints();
        updateLeftScore();
    } 
    public static void addRightPoints(int p) {
        rightPoints += p;
        checkPoints();
        updateRightScore();
    }
    
    public static void subPoints(int p) {
        points -= p;
        checkPoints();
        updateScoreboard();
    }
    public static void multPoints(int p) {
        points *= p;
        checkPoints();
        updateScoreboard();
    }
    public static void divPoints(int p) {
        points /= p;
        checkPoints();
        updateScoreboard();
    }
    
    public static void addPointTrigger( string equality, int points, string function ){
        pointTriggers.Add( new PointTrigger( equality, points, function ) );
    }
    
    public static void updateScoreboardMessage(string s)
    {
        GameObject scoreboardMessage = GameObject.FindGameObjectWithTag("MessageText");
        scoreboardMessage.GetComponentInChildren<TextMesh>().text = s;
    }
    //See if any PointTriggers have their requirements met; if so, call their functions.
    private static void checkPoints(){
        foreach ( PointTrigger pt in pointTriggers ){
            if(pt.testEquality())
                pt.execute();
        }
    }

    public static void updateScoreboard(){
        updateLeftScore();
        updateRightScore();
        updateLivesDisplay();
    }

    private static void updateLeftScore()
    {
        GameObject leftScoreboard = GameObject.FindGameObjectWithTag("LeftPoints");
        leftScoreboard.GetComponentInChildren<TextMesh>().text = "Left: " + leftPoints + " pts";
    }

    private static void updateRightScore()
    {
        GameObject rightScoreboard = GameObject.FindGameObjectWithTag("RightPoints");
        rightScoreboard.GetComponentInChildren<TextMesh>().text = "Right: " + rightPoints + " pts";
    }

    private static void updateLivesDisplay()
    {
        GameObject lifeDisplay = GameObject.FindGameObjectWithTag("LivesDisplay");
        if (BalloonGameplayManager.Instance.GetGameSettings().gameMode == GameSettingsSO.GameMode.RELAXED)
        {
            lifeDisplay.GetComponentInChildren<TextMesh>().text = "Lives: Unlimited";
        }
        else
        {
            lifeDisplay.GetComponentInChildren<TextMesh>().text = "Lives: " + BalloonGameplayManager.Instance.playerLives;
        }
    }

   
}
