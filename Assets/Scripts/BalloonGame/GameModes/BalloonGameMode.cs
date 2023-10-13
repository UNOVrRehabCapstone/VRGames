using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BalloonGameMode
{
    public enum GameStatus {
        PLAYING,
        WIN,
        LOSS,
        COMPLETE
    }

    public enum GameMode {
        RELAXED,
        NORMAL,
        ENDLESS
    }

    public abstract GameStatus UpdateGame();
}
