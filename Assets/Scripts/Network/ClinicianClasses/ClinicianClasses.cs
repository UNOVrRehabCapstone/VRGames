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
        //these are the default values for the balloon game that will be loaded if no clinician is controlling the game
        //NOTE - ballonGameSettingsMaxLives must be set to 3 for careerMode to function correctly.
        public static string balloonGameMode = "0";
        public static string balloonGameGoal = "55";
        public static string balloonGameSpecialBalloonFrequency = "44";
        public static string balloonGameHandSetting = "2";
        public static string balloonGamePattern = "1";
        public static string balloonGameMaxLives = "3";
        public static string balloonGameLeftRightRatio = "0.7";
        public static int    careerModeLevelToPlay = 4;
        public static bool   balloonStart = false;
        public static bool   clinicianIsControlling = false;
    }

    public class BalloonGameData
    {
        public string userName;
        public string achievementProgress;
        public string careerProgress;
        public string levelOneScore;
        public string levelTwoScore;
        public string levelThreeScore;
        public string levelFourScore;
        public string levelFiveScore;
    }

    public static class BalloonGameDataValues
    {
        public static string achievementProgress = "0000000000";
    }

    public static class Achievements
    {
        public static bool[] SpecialsPopped = { false, false, false, false};
        // Here is the list of available achievements!
        public static Achievement PopOneBalloon = new Achievement(0, "Welcome to the Game!", "Pop your first balloon.");
        public static Achievement PopEntireBalloonStream = new Achievement(1, "Go With the Flow", "Pop an entire balloon stream.");
        public static Achievement PopEntireTargetBalloon = new Achievement(2, "Peak efficiency", "Fully pop a target balloon.");
        public static Achievement PopOnionCore = new Achievement(3, "Inner Core", "Pop the core of a layered balloon.");
        public static Achievement PopTwoAtOnce = new Achievement(4, "Double Barreled", "Pop two normal balloons at once.");
        public static Achievement PopAllSpecials = new Achievement(5, "Well Versed", "Pop every type of special balloon.");
        public static Achievement EndWithMoreLives = new Achievement(6, "Overachiever", "Finish a game with more lives than you started with.");
        public static Achievement FinishedCustomGame = new Achievement(7, "Full Control", "Finish a custom game.");

    }

    public static class CareerModeLevels
        //Balloon tags: 
        //Default balloon = Balloon
        //Onion   balloon = OnionBalloon
        //Stream  balloon = Balloon_Stream_Powerup
        //Target  balloon = Balloon_Target
        //Life    balloon = RestoreLife
    {//                                         3sec -> normal spawn -> repeat 5 times
        public static string[] levelOneSchedule = {"Balloon", "3.0", "Balloon","3.0", "Balloon", "3.0", "Balloon", "3.0", "Balloon", "END" };
        public static CareerModeLevel levelOne = new CareerModeLevel(levelOneSchedule);

        public static string[] levelTwoSchedule = {"Balloon", "2.5", "RestoreLife", "2.0", "Balloon", "2.0", "BalloonOnion", "4", "BalloonOnion", "END" };
        public static CareerModeLevel levelTwo = new CareerModeLevel(levelTwoSchedule);

        public static string[] levelThreeSchedule = {"Balloon_Stream_Powerup", "2.0", "Balloon_Stream_Powerup", "5", "BalloonOnion", "2.0", "BalloonOnion", "2.0", "RestoreLife", "END" };
        public static CareerModeLevel levelThree = new CareerModeLevel(levelThreeSchedule);

        public static string[] levelFourSchedule = {"Balloon_Target", "7.0", "Balloon_Stream_Powerup", "1", "Balloon_Stream_Powerup", "3.0", "Balloon_Target", "END" };
        public static CareerModeLevel levelFour = new CareerModeLevel(levelFourSchedule);

        public static string[] levelFiveSchedule = {"RestoreLife", "0.0", "RestoreLife", "1.0", "Balloon_Stream_Powerup", "2.0", "BalloonOnion","2.5","Balloon_Target","0","Balloon_Target", "END" };
        public static CareerModeLevel levelFive = new CareerModeLevel(levelFiveSchedule);

        public static CareerModeLevel[] levels = { levelOne, levelTwo, levelThree, levelFour, levelFive };
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