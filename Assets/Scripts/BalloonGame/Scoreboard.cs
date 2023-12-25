using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BalloonsGame
{
	public class Scoreboard : MonoBehaviour
    {
        private void Start()
        {
            PointsManager.Instance.OnUpdatePoints += this.OnUpdatePointsHandler;
        }

        private void OnUpdatePointsHandler(object sender, PointsManager.OnUpdatePointsEventArgs e)
        {
            UpdateLeftPoints(e.leftPoints);
            UpdateRightPoints(e.rightPoints);
        }

        private void UpdateLeftPoints(int leftPoints)
        {
            GameObject leftScoreboard = GameObject.FindGameObjectWithTag("LeftPoints");
            TextMesh leftTextMesh = leftScoreboard.GetComponentInChildren<TextMesh>();
            leftScoreboard.GetComponentInChildren<TextMesh>().text = "Left: " + leftPoints + " pts";

            // Updating the color based on the hand setting
            if (GameManager.Instance.GetGameSettings().handSetting == GameSettingsSO.HandSetting.LEFT_HAND){
                leftTextMesh.color = Color.blue;
            } else{
                leftTextMesh.color = Color.black;
            }
        }

        private void UpdateRightPoints(int rightPoints)
        {
            GameObject rightScoreboard = GameObject.FindGameObjectWithTag("RightPoints");
            TextMesh rightTextMesh = rightScoreboard.GetComponentInChildren<TextMesh>();
            rightScoreboard.GetComponentInChildren<TextMesh>().text = "Right: " + rightPoints + " pts";

            // Updating the color based on the hand setting
            if (GameManager.Instance.GetGameSettings().handSetting == GameSettingsSO.HandSetting.RIGHT_HAND){
                rightTextMesh.color = Color.blue;
            } else{
                rightTextMesh.color = Color.black;
            }
        }
    }
}

