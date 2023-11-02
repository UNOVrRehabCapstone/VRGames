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
    }
    public static class BalloonGameSettingsValues
    {
        public static string balloonGameMode = "0";
        public static string balloonGameGoal = "10";
        public static string balloonGameSpecialBalloonFrequency = "50";
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