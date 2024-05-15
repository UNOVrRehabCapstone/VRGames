/**
 * \file PointsManager.cs
 * \brief Manages points and related functionality in the game.
 *
 * The PointsManager class is responsible for managing points and related functionality in the game, including updating scores, checking goals, and handling point triggers.
 */

using Classes.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * \class PointsManager
 * \brief Manages points and related functionality in the game.
 *
 * The PointsManager class is responsible for managing points and related functionality in the game, including updating scores, checking goals, and handling point triggers.
 */
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


  /**
   * \brief Initializes the PointsManager and resets points.
   */
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


  /// Retrieves the points accumulated by the left hand.
  public static int getLeftPoints() { return leftPoints; }

  /// Retrieves the points accumulated by the right hand.
  public static int getRightPoints() { return rightPoints; }

  /// Retrieves the total points accumulated.
  public static int getPoints() { return points; }

  /**
   * \brief Resets all points to zero.
   */
  public static void resetPoints() {
        points = 0;
        checkPoints();
        updateScoreboard();
      
    }

  /**
   * \brief Resets points for both left and right hands to zero.
   */
  public static void resetBothHandPoints()
    {
        leftPoints = 0;
        rightPoints = 0;
        checkPoints();
        updateLeftScore();
        updateRightScore();
        updateLivesDisplay();
    }

  /**
   * \brief Adds points to the total score.
   * 
   * \param p The points to add.
   */
  public static void addPoints(int p)
  {
    points += p;
    checkPoints();
    updateScoreboard();
  }

  /**
   * \brief Adds points to the left hand's score.
   * 
   * \param p The points to add.
   */
  public static void addLeftPoints(int p)
  {
    leftPoints += p;
    checkPoints();
    updateLeftScore();
  }

  /**
   * \brief Adds points to the right hand's score.
   * 
   * \param p The points to add.
   */
  public static void addRightPoints(int p)
  {
    rightPoints += p;
    checkPoints();
    updateRightScore();
  }

  /**
   * \brief Subtracts points from the total score.
   * 
   * \param p The points to subtract.
   */
  public static void subPoints(int p)
  {
    points -= p;
    checkPoints();
    updateScoreboard();
  }

  /**
   * \brief Multiplies the total score by a factor.
   * 
   * \param p The factor to multiply by.
   */
  public static void multPoints(int p)
  {
    points *= p;
    checkPoints();
    updateScoreboard();
  }

  /**
   * \brief Divides the total score by a factor.
   * 
   * \param p The factor to divide by.
   */
  public static void divPoints(int p)
  {
    points /= p;
    checkPoints();
    updateScoreboard();
  }

  /**
   * \brief Checks if a goal is reached based on the current hand setting.
   * 
   * \param handSetting The hand setting to check against.
   * \param goal The goal to compare against.
   * \return True if the goal is reached, otherwise false.
   */
  public static bool isGoalReached(GameSettingsSO.HandSetting handSetting, int goal)
  {
    switch (handSetting)
    {
      case GameSettingsSO.HandSetting.LEFT_HAND:
        return leftPoints >= goal;
      case GameSettingsSO.HandSetting.RIGHT_HAND:
        return rightPoints >= goal;
      case GameSettingsSO.HandSetting.BOTH_HANDS:
        return (leftPoints + rightPoints) >= goal;
      default:
        return false;
    }
  }

  /**
   * \brief Adds a point trigger to the list.
   * 
   * \param equality The condition to trigger the point trigger.
   * \param points The points required for the trigger.
   * \param function The function to execute when triggered.
   */
  public static void addPointTrigger(string equality, int points, string function)
  {
    pointTriggers.Add(new PointTrigger(equality, points, function));
  }

  /**
   * \brief Updates the message displayed on the scoreboard.
   * 
   * \param s The message to display.
   */
  public static void updateScoreboardMessage(string s)
  {
    GameObject scoreboardMessage = GameObject.FindGameObjectWithTag("MessageText");
    scoreboardMessage.GetComponentInChildren<TextMesh>().text = s;
  }

  /**
   * \brief Checks if any point triggers have their requirements met and calls their functions.
   */
  private static void checkPoints()
  {
    foreach (PointTrigger pt in pointTriggers)
    {
      if (pt.testEquality())
        pt.execute();
    }
  }

  /**
   * \brief Updates the scoreboard with current scores and display settings.
   */
  public static void updateScoreboard()
  {
    updateScore();
    updateLeftScore();
    updateRightScore();
    updateLivesDisplay();
  }

  /**
   * \brief Updates the left hand's score displayed on the scoreboard.
   */
  private static void updateLeftScore()
  {
    GameObject leftScoreboard = GameObject.FindGameObjectWithTag("LeftPoints");
    TextMesh leftTextMesh = leftScoreboard.GetComponentInChildren<TextMesh>();
    leftScoreboard.GetComponentInChildren<TextMesh>().text = "Oh no Left: " + leftPoints + " pts";

    // Updating the color based on the hand setting
    if (BalloonGameplayManager.Instance.GetGameSettings().handSetting == GameSettingsSO.HandSetting.LEFT_HAND)
    {
      leftTextMesh.color = Color.blue;
    }
    else
    {
      leftTextMesh.color = Color.black;
    }
  }

  /**
   * \brief Updates the right hand's score displayed on the scoreboard.
   */
  private static void updateRightScore()
  {
    GameObject rightScoreboard = GameObject.FindGameObjectWithTag("RightPoints");
    TextMesh rightTextMesh = rightScoreboard.GetComponentInChildren<TextMesh>();
    rightScoreboard.GetComponentInChildren<TextMesh>().text = "Right: " + rightPoints + " pts";

    // Updating the color based on the hand setting
    if (BalloonGameplayManager.Instance.GetGameSettings().handSetting == GameSettingsSO.HandSetting.RIGHT_HAND)
    {
      rightTextMesh.color = Color.blue;
    }
    else
    {
      rightTextMesh.color = Color.black;
    }
  }

  /**
   * \brief Updates the total score displayed on the scoreboard.
   */
  private static void updateScore()
  {
    GameObject scoreboard = GameObject.FindGameObjectWithTag("Points");
    TextMesh textMesh = scoreboard.GetComponentInChildren<TextMesh>();
    scoreboard.GetComponentInChildren<TextMesh>().text = points + " pts";
  }

  /**
   * \brief Updates the display of lives on the scoreboard.
   */
  private static void updateLivesDisplay()
  {
    GameObject lifeDisplay = GameObject.FindGameObjectWithTag("LivesDisplay");
    if (BalloonGameplayManager.Instance.GetGameSettings().maxLives > 50)
    {
      lifeDisplay.GetComponentInChildren<TextMesh>().text = "Lives: Unlimited";
    }
    else
    {
      lifeDisplay.GetComponentInChildren<TextMesh>().text = "Lives: " + BalloonGameplayManager.Instance.playerLives;
    }
  }
}
