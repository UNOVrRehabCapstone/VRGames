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
        public string environment;
    }

    public class BalloonGameSettings
    {
        public string mode;
        public string target;
        public string freq;
        public string ratio;
        public string pattern;
        public string lives;
        public string hand;
        public string careerModeLevelToPlay;
        public string modifier;
        public string numBalloonsSpawnedAtOnce;
        public string timeBetweenSpawns;
    }

    public static class BalloonGameSettingsValues
    {
        //these are the default values for the balloon game that will be loaded if no clinician is controlling the game
        //NOTE - ballonGameSettingsMaxLives must be set to 3 for careerMode to function correctly.
        
        public static string balloonGameMode = "1"; /* 0 = Career, 1 = Custom, 2 = Manual */
        public static string environment = "0";     /* 0 = Balloon Enclosure,  1 = Meadow */

        public static bool   balloonStart = true;
        public static bool   clinicianIsControlling = false;
        public static bool   gameIsRunning = false;


        /* SETTINGS SPECIFIC TO CUSTOM GAME MODE */
        public static string balloonGamePattern  = "1";    /* 0 = Concurrent, 1 = Alternating, 2 = Random*/
        public static string balloonGameHandSetting = "2"; /* 0 = Left hand, 1 = Right hand , 2 = Both hands */
        public static string balloonGameGoal = "10";
        public static string balloonGameSpecialBalloonFrequency = "99";
        public static string balloonGameMaxLives = "3";
        public static string balloonGameLeftRightRatio = "0.5";
        public static string spawnTime = "2.5";
        public static string numBalloonsSpawnedAtOnce = "2";
        public static string speedModifier = "1";

        /* SETTINGS SPECIFIC TO CAREER MODE */
        public static string careerModeLevelToPlay = "0";
    }

    public class BalloonGameData
    {
        public string userName;
        public string levelOneScore;
        public string levelTwoScore;
        public string levelThreeScore;
        public string levelFourScore;
        public string levelFiveScore;
        public bool ach0;
        public bool ach1;
        public bool ach2;
        public bool ach3;
        public bool ach4;
        public bool ach5;
        public bool ach6;
        public bool ach7;
        public bool ach8;
        public bool ach9;
    }

    public static class BalloonGameDataValues
    {
        public static string userName = "Default User";
        public static string levelOneScore = "0";
        public static string levelTwoScore = "0";
        public static string levelThreeScore = "0";
        public static string levelFourScore = "0";
        public static string levelFiveScore = "0";
        public static bool ach0 = false;
        public static bool ach1 = false;
        public static bool ach2 = false;
        public static bool ach3 = false;
        public static bool ach4 = false;
        public static bool ach5 = false;
        public static bool ach6 = false;
        public static bool ach7 = false;
        public static bool ach8 = false;
        public static bool ach9 = false;


        public static string[] levelScores = {levelOneScore, levelTwoScore,
            levelThreeScore, levelFourScore, levelFiveScore};
    }

    public static class Achievements
    {
        public static bool[] SpecialsPopped = { false, false, false, false};
        public static bool[] LevelsPlayed = { false, true };
        // Here is the list of available achievements!
        public static Achievement PopOneBalloon = new Achievement(false,0, "Welcome to the Game!", "Pop your first balloon.");
        public static Achievement PopEntireBalloonStream = new Achievement(false, 1, "Go With the Flow", "Pop an entire balloon stream.");
        public static Achievement PopEntireTargetBalloon = new Achievement(false, 2, "Peak efficiency", "Fully pop a target balloon.");
        public static Achievement PopOnionCore = new Achievement(false, 3, "Inner Core", "Pop the core of a layered balloon.");
        public static Achievement PopTwoAtOnce = new Achievement(false, 4, "Double Barreled", "Pop two normal balloons at once.");
        public static Achievement PopAllSpecials = new Achievement(false, 5, "Well Versed", "Pop every type of special balloon.");
        public static Achievement EndWithMoreLives = new Achievement(false, 6, "Overachiever", "Finish a game with more lives than you started with.");
        public static Achievement FinishedCustomGame = new Achievement(false, 7, "Full Control", "Finish a custom game.");
        public static Achievement PlayBothEnvironments = new Achievement(false, 8, "World Traveler", "Play a game in all environments.");
        public static Achievement FinishCareerMode = new Achievement(false, 9, "Game Over!", "Finish career mode. Well done!");




        public static Achievement[] AllAchievements = {PopOneBalloon, PopEntireBalloonStream, PopEntireTargetBalloon,
            PopOnionCore, PopTwoAtOnce, PopAllSpecials, EndWithMoreLives, FinishedCustomGame, PlayBothEnvironments, FinishCareerMode };

    }

    public static class CareerModeLevels
        //Balloon tags: 
        //Default balloon = Balloon
        //Onion   balloon = OnionBalloon
        //Stream  balloon = Balloon_Stream_Powerup
        //Target  balloon = Balloon_Target
        //Life    balloon = RestoreLife
    {
        //A spawn schedule is a list of strings. Numerical values are interpreted as spawn timings. Alphabetical gets interpreted as 
        // the tag of the type of balloon to spawn. Each schedule must be terminated with an "END" string.
        public static string[] levelOneSchedule = 
            {
            "Balloon",
            "3.0",
            "Balloon",
            "3.0",
            "Balloon",
            "3.0",
            "Balloon",
            "3.0",
            "Balloon",
            "END" 
        };
        public static string[] levelTwoSchedule = 
            {
            "Balloon",
            "2.5",
            "RestoreLife",
            "2.0",
            "Balloon",
            "2.0",
            "BalloonOnion",
            "4",
            "BalloonOnion",
            "END"
        };
        public static string[] levelThreeSchedule = {
            "Balloon_Stream_Powerup",
            "2.0",
            "Balloon_Stream_Powerup",
            "5",
            "BalloonOnion",
            "2.0",
            "BalloonOnion",
            "2.0",
            "RestoreLife",
            "END"
        };
        public static string[] levelFourSchedule = 
            {
            "Balloon_Target",
            "7.0",
            "Balloon_Stream_Powerup",
            "1", "Balloon_Stream_Powerup",
            "3.0",
            "Balloon_Target",
            "END"
        };
        public static string[] levelFiveSchedule = 
            {
            "RestoreLife",
            "0.0",
            "RestoreLife",
            "1.0",
            "Balloon_Stream_Powerup",
            "2.0",
            "BalloonOnion",
            "2.5",
            "Balloon_Target",
            "0",
            "Balloon_Target",
            "END"
        };

        
        public static CareerModeLevel levelOne = new CareerModeLevel(levelOneSchedule);
        public static CareerModeLevel levelTwo = new CareerModeLevel(levelTwoSchedule);
        public static CareerModeLevel levelThree = new CareerModeLevel(levelThreeSchedule);
        public static CareerModeLevel levelFour = new CareerModeLevel(levelFourSchedule);
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