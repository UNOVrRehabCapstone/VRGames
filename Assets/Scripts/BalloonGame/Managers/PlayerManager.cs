using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BalloonsGame 
{
	public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager Instance {get; private set;}
        public event EventHandler OnAllLivesLost;

        private int lives;

        private void Awake()
        {
            if (Instance != null) {
                Destroy(this);
            } else {
                Instance = this;
            }

            this.lives = GameManager.Instance.GetGameSettings().maxLives;
        }

        public void DecrementLife()
        {
            --this.lives;
            if (this.lives < 1) {
                OnAllLivesLost?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}

