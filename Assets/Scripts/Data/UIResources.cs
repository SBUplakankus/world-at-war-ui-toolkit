using System.Collections.Generic;
using UI.Enums;
using UnityEngine;
using UnityEngine.UIElements;

namespace Data
{
    public static class UIResources
    {
        public static readonly Dictionary<Difficulty, VectorImage> DifficultyIcons = new()
        {
            [Difficulty.Recruit] = Resources.Load< VectorImage>("Icons/Recruit"),
            [Difficulty.Regular] = Resources.Load< VectorImage>("Icons/Regular"),
            [Difficulty.Hardened] = Resources.Load< VectorImage>("Icons/Hardened"),
            [Difficulty.Veteran] = Resources.Load< VectorImage>("Icons/Veteran")
        };

        public static readonly Dictionary<Missions, string> MissionTitles = new()
        {
            [Missions.SemperFi] = "SEMPER FI",
            [Missions.LittleResistance] = "LITTLE RESISTANCE",
            [Missions.HardLanding] = "HARD LANDING",
            [Missions.Vendetta] = "VENDETTA",
            [Missions.TheirLandTheirBlood] = "THEIR LAND, THEIR BLOOD",
            [Missions.BurnEmOut] = "BURN 'EM OUT",
            [Missions.Relentless] = "RELENTLESS",
            [Missions.BloodAndIron] = "BLOOD & IRON",
            [Missions.RingOfSteel] = "RING OF STEEL",
            [Missions.Eviction] = "EVICTION",
            [Missions.BlackCats] = "BLACK CATS",
            [Missions.BlowtorchAndCorkscrew] = "BLOWTORCH & CORKSCREW",
            [Missions.BreakingPoint] = "BREAKING POINT",
            [Missions.HeartOfTheReich] = "HEART OF THE REICH",
            [Missions.Downfall] = "DOWNFALL"
        };

        public static readonly Dictionary<Missions, string> MissionDescriptions = new()
        {
            [Missions.SemperFi] = "The Marines assault the beaches of Betio Island. Heavy resistance expected on all sectors.",
            [Missions.LittleResistance] = "Advance through the jungles of Peleliu and secure the enemy stronghold.",
            [Missions.HardLanding] = "Secure the airfield on Peleliu under heavy mortar fire. Extraction is not an option.",
            [Missions.Vendetta] = "Fight alongside Reznov in the ruins of Stalingrad. One shot. One kill.",
            [Missions.TheirLandTheirBlood] = "Push through the Russian countryside and drive the enemy back.",
            [Missions.BurnEmOut] = "Use flamethrowers to clear fortified positions in the jungle.",
            [Missions.Relentless] = "Fight through the industrial district of Stalingrad. No surrender.",
            [Missions.BloodAndIron] = "Command a tank through enemy lines and break the siege.",
            [Missions.RingOfSteel] = "Defend the outskirts of Berlin against overwhelming forces.",
            [Missions.Eviction] = "Clear buildings in Berlin block by block. Room by room.",
            [Missions.BlackCats] = "Fly a PBY Catalina on a rescue mission through enemy waters.",
            [Missions.BlowtorchAndCorkscrew] = "Assault Okinawa's beaches and clear the cave networks.",
            [Missions.BreakingPoint] = "Fight through the caves of Okinawa. The enemy is cornered.",
            [Missions.HeartOfTheReich] = "Storm the Reichstag. This is the final push.",
            [Missions.Downfall] = "The final assault on Berlin. End it."
        };

        public static readonly Dictionary<Difficulty, string> DifficultyDescriptions = new()
        {
            [Difficulty.Recruit] = "We've got you covered. Enemies are less aggressive and deal reduced damage.",
            [Difficulty.Regular] = "A balanced challenge. Enemies are tactical and deadly.",
            [Difficulty.Hardened] = "Enemies are aggressive and accurate. Only the strong survive.",
            [Difficulty.Veteran] = "The ultimate test. Enemies are relentless and lethal."
        };

        public static readonly string[] MessagesOfTheDay =
        {
            "All campaign missions are now available for deployment. Good luck out there, soldier.",
            "Addressed stability issues and improved menu navigation. Save system now operational across all modes.",
            "Team Deathmatch and Domination now available in core matchmaking rotation. Get your squad together.",
            "Thank you to our community for 10 million players worldwide. See you on the front lines.",
            "Weapon balance adjustments and performance improvements deployed across all modes.",
            "Double XP weekend is active now through Monday. Rank up faster in all game modes.",
            "Party connection stability has been upgraded. Matchmaking improvements are now live on all servers.",
            "Check the barracks for your service record and recent accolades. New missions are waiting.",
            "Resolved a rare crash when navigating the multiplayer lobby. Servers are back online.",
            "All servers are operational. Squad up and dominate the leaderboards."
        };

        public static readonly string[] OptionInversion = { "Off", "On" };
        public static readonly string[] OptionStickLayout = { "Default", "Southpaw", "Legacy", "Legacy Southpaw" };
        public static readonly string[] OptionButtonLayout = { "Default", "Southpaw", "Legacy", "Legacy Southpaw" };
        public static readonly string[] OptionSensitivity = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" };
        public static readonly string[] OptionTargetAssist = { "Off", "On" };
    }
}
