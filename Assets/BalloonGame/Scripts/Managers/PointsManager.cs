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
		public event EventHandler<OnUpdatePointsEventArgs> OnUpdatePoints;
		public class OnUpdatePointsEventArgs : EventArgs {
			public int leftPoints;
			public int rightPoints;
			public int totalPoints;

			public OnUpdatePointsEventArgs(int leftPoints, int rightPoints, int totalPoints)
			{
				this.leftPoints  = leftPoints;
				this.rightPoints = rightPoints;
				this.totalPoints = totalPoints;
			}
		}

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

		public void AddLeftPoints(int points)
		{
			this.leftPoints  += points;
			this.totalPoints += points;
			OnUpdatePoints?.Invoke(this, new OnUpdatePointsEventArgs(this.leftPoints, this.rightPoints, 
			                                                          this.totalPoints));
			this.CheckGoal();
		}

		public void AddRightPoints(int points)
		{
			this.rightPoints += points;
			this.totalPoints += points;
			OnUpdatePoints?.Invoke(this, new OnUpdatePointsEventArgs(this.leftPoints, this.rightPoints, 
			                                                          this.totalPoints));
			this.CheckGoal();
		}

		private void CheckGoal()
		{
			int goal = this.gameSettings.goal;

			switch(this.gameSettings.handSetting) {
				case GameSettingsSO.HandSetting.LEFT_HAND:
					if (this.leftPoints >= goal) {
						OnGoalReached?.Invoke(this, EventArgs.Empty);
					}
					break;
				case GameSettingsSO.HandSetting.RIGHT_HAND:
					if (this.rightPoints >= goal) {
						OnGoalReached?.Invoke(this, EventArgs.Empty);
					}
					break;
				case GameSettingsSO.HandSetting.BOTH_HANDS:
					if (this.totalPoints >= goal) {
						OnGoalReached?.Invoke(this, EventArgs.Empty);
					}
					break;
			}
		}
	}
}

