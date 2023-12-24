using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BalloonsGame 
{
	public class PlayerManager : MonoBehaviour
    {
        public static PlayerManager Instance {get; private set;}

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

        public void ReduceLife()
        {
            --this.lives;
        }
    }
}

