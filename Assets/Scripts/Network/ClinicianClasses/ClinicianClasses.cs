using System;

namespace SocketClasses
{
    public class JoinRoom
    {
        public string clinician;
    }
    public class StartGame
    {
        public string game;
    }
    public class BalloonGameSettings
    {
        public string mode;
        public string target;
        public string freq;
        public string handSetting;
        public string pattern;
        public string lives;
        public string hand;
        public string ratio;
    }
    public static class BalloonGameSettingsValues
    {
        public static string balloonGameMode = "1";
        public static string balloonGameGoal = "30";
        public static string balloonGameSpecialBalloonFrequency = "44";
        public static string balloonGameHandSetting = "1";
        public static string balloonGamePattern = "2";
        public static string balloonGameMaxLives = "5";
        public static string balloonGameLeftRightRatio = "0.7";
    }

    public static class Pausing
    {
        public static Boolean isPaused = false;
    }

    public class Position
    {
        public float playerXPosition;
        public float playerYPosition;
        public float playerZPosition;
    }

    public class IKRig
    {
        public float shoulderWidth;
        public float headHeight;
        public float armLength;
        public float extendedArmThreshold;
        public float retractedArmThreshold;
    }

    public class ShowIKSkeleton
    {
        public bool showSkeleton;
    }

    public class HandScaling
    {
        public string handToScale;
        public float scaleAmount;
    }

    public class PatientSetting
    {
        public string userName;
    }
}