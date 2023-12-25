using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BalloonsGame
{
	public class PointsManager : MonoBehaviour
	{
		public static PointsManager Instance {get; private set;}
		public event EventHandler OnGoalReached;

		private int leftPoints;
		private int rightPoints;
		private int totalPoints;
		private GameSettingsSO gameSettings;

		private void Awake()
		{
			if (Instance != null) {
				Destroy(this);
			} else {
				Instance = this;
			}
		}

		private void Start()
		{
			this.gameSettings = GameManager.Instance.GetGameSettings();
		}

		public void IncrementLeftPoints()
		{
			++this.leftPoints;
			++this.totalPoints;
			this.CheckGoal();
		}

		public void IncrementRightPoints()
		{
			++this.rightPoints;
			++this.totalPoints;
			this.CheckGoal();
		}

		private void CheckGoal()
		{
			int goal = this.gameSettings.goal;

			switch(this.gameSettings.handSetting) {
				case GameSettingsSO.HandSetting.LEFT_HAND:
					if (this.leftPoints == goal) {
						OnGoalReached?.Invoke(this, EventArgs.Empty);
					}
					break;
				case GameSettingsSO.HandSetting.RIGHT_HAND:
					if (this.rightPoints == goal) {
						OnGoalReached?.Invoke(this, EventArgs.Empty);
					}
					break;
				case GameSettingsSO.HandSetting.BOTH_HANDS:
					if (this.totalPoints == goal) {
						OnGoalReached?.Invoke(this, EventArgs.Empty);
					}
					break;
			}
		}
	}
}

