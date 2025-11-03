using Rage;
using RAGENativeUI;
using System.Windows.Forms;

namespace HealthArmourDisplay
{
    internal static class Settings
    {
        internal static int BaseOffsetVertical = 40;
        internal static int BaseOffsetHorizontal = 70;

        internal static int HealthTextVertical = 0;
        internal static int HealthTextHorizontal = 0;
        internal static int HealthIconVertical = 12;
        internal static int HealthIconHorizontal = 35;
        internal static string HealthColor = "HudColor.RadarHealth";
        
        internal static int ArmourTextVertical = 0;
        internal static int ArmourTextHorizontal = 250;
        internal static int ArmourIconVertical = 12;
        internal static int ArmourIconHorizontal = 200;
        internal static string ArmourColor = "HudColor.RadarArmour";
        
        internal static int HungerDepletionSpeed = 600;
        internal static int HungerTextVertical = 200;
        internal static int HungerTextHorizontal = 0;
        internal static int HungerIconVertical = 215;
        internal static int HungerIconHorizontal = 30;
        internal static string HungerColor = "HudColor.OrangeLight";
        internal static bool HungerAnimationEnabled = true;
        internal static string HungerAnimDictio = "amb@world_human_seat_wall_eating@male@both_hands@base";
        internal static string HungerAnimName = "base";
        internal static string HungerAnimProp = "prop_cs_hotdog_01";
        
        internal static int ThirstDepletionSpeed = 800;
        internal static int ThirstTextVertical = 200;
        internal static int ThirstTextHorizontal = 250;
        internal static int ThirstIconVertical = 215;
        internal static int ThirstIconHorizontal = 200;
        internal static string ThirstColor = "HudColor.BlueLight";
        internal static bool ThirstAnimationEnabled = true;
        internal static string ThirstAnimDictio = "amb@world_human_drinking@coffee@male@idle_a";
        internal static string ThirstAnimName = "idle_a";
        internal static string ThirstAnimProp = "prop_cs_beer_bot_01";
        
        internal static string StoreLocations = "29.537, -1345.474, 28.497 | -2969.773, 393.134, 14.043 | -1490.112, -378.83, 39.163 | 1137.569, -983.894, 45.416 | 1704.031, 4929.222, 41.064 | 1156.47, -322.609, 68.205 | -1827.282, 789.701, 137.254 | -1222.331, -904.582, 11.326 | -714.432, -912.26, 18.216 | -52.042, -1751.564, 28.421 | 544.2, 2668.531, 41.156 | 1963.312, 3744.618, 31.344 | 2555.542, 386.205, 107.623 | 377.961, 327.06, 102.566 | 2678.844, 3284.787, 54.241 | 1733.24, 6414.755, 34.037 | -3042.428, 588.687, 6.909 | -3243.83, 1005.144, 11.831 | 1164.038, 2707.052, 37.158";
        internal static string Drinks = "eCola: 2 | Pisswasser: 2 | Sprunk: 2";
        internal static string Foods = "EgoChaser: 3 | Meteorite Bar: 5 | P's & Q's: 2";
        
        internal static bool AutoConsume = false;
        internal static Keys StoreKey = Keys.E;
        internal static Keys InventoryKey = Keys.I;
        internal static Keys InventoryModifier = Keys.LControlKey;
        internal static string Inventory = "2 | 0 | 0 | 0 | 4 | 0";

        internal static Common.EFont FontFamily = Common.EFont.Pricedown;
        internal static float FontSize = 0.4f;



        internal static string path = "Plugins/HealthArmourDisplay.ini";
        internal static InitializationFile ini = new InitializationFile(path);

        internal static void LoadSettings()
        {
            Game.LogTrivial($"Loading plugin settings...");

            ini.Create();
            BaseOffsetVertical = ini.ReadInt16("BasePosition", "BaseOffsetVertical", 45);
            BaseOffsetHorizontal = ini.ReadInt16("BasePosition", "BaseOffsetHorizontal", 65);

            HealthTextVertical = ini.ReadInt16("Health", "TextVertical", -15);
            HealthTextHorizontal = ini.ReadInt16("Health", "TextHorizontal", 0);
            HealthIconVertical = ini.ReadInt16("Health", "IconVertical", 0);
            HealthIconHorizontal = ini.ReadInt16("Health", "IconHorizontal", 30);
            HealthColor = ini.ReadString("Health", "Color", "HudColor.RadarHealth");

            ArmourTextVertical = ini.ReadInt16("Armour", "TextVertical", -15);
            ArmourTextHorizontal = ini.ReadInt16("Armour", "TextHorizontal", 250);
            ArmourIconVertical = ini.ReadInt16("Armour", "IconVertical", 0);
            ArmourIconHorizontal = ini.ReadInt16("Armour", "IconHorizontal", 200);
            ArmourColor = ini.ReadString("Armour", "Color", "HudColor.RadarArmour");

            HungerDepletionSpeed = ini.ReadInt16("Hunger", "HungerDepletionSpeed", 600);
            HungerTextVertical = ini.ReadInt16("Hunger", "TextVertical", 200);
            HungerTextHorizontal = ini.ReadInt16("Hunger", "TextHorizontal", 0);
            HungerIconVertical = ini.ReadInt16("Hunger", "IconVertical", 215);
            HungerIconHorizontal = ini.ReadInt16("Hunger", "IconHorizontal", 30);
            HungerColor = ini.ReadString("Hunger", "Color", "HudColor.OrangeLight");
            HungerAnimationEnabled = ini.ReadBoolean("Hunger", "AnimationEnabled", true);
            HungerAnimDictio = ini.ReadString("Hunger", "AnimationDictionnary", "amb@world_human_seat_wall_eating@male@both_hands@base");
            HungerAnimName = ini.ReadString("Hunger", "AnimationName", "base");
            HungerAnimProp = ini.ReadString("Hunger", "AnimationProp", "prop_cs_hotdog_01");

            ThirstDepletionSpeed = ini.ReadInt16("Thirst", "ThirstDepletionSpeed", 800);
            ThirstTextVertical = ini.ReadInt16("Thirst", "TextVertical", 200);
            ThirstTextHorizontal = ini.ReadInt16("Thirst", "TextHorizontal", 250);
            ThirstIconVertical = ini.ReadInt16("Thirst", "IconVertical", 215);
            ThirstIconHorizontal = ini.ReadInt16("Thirst", "IconHorizontal", 200);
            ThirstColor = ini.ReadString("Thirst", "Color", "HudColor.BlueLight");
            ThirstAnimationEnabled = ini.ReadBoolean("Thirst", "AnimationEnabled", true);
            ThirstAnimDictio = ini.ReadString("Thirst", "AnimationDictionnary", "amb@world_human_drinking@coffee@male@idle_a");
            ThirstAnimName = ini.ReadString("Thirst", "AnimationName", "idle_a");
            ThirstAnimProp = ini.ReadString("Thirst", "AnimationProp", "prop_cs_beer_bot_01");

            StoreLocations = ini.ReadString("Stores", "StoreLocations", "29.537, -1345.474, 28.497 | -2969.773, 393.134, 14.043 | -1490.112, -378.83, 39.163 | 1137.569, -983.894, 45.416 | 1704.031, 4929.222, 41.064 | 1156.47, -322.609, 68.205 | -1827.282, 789.701, 137.254 | -1222.331, -904.582, 11.326 | -714.432, -912.26, 18.216 | -52.042, -1751.564, 28.421 | 544.2, 2668.531, 41.156 | 1963.312, 3744.618, 31.344 | 2555.542, 386.205, 107.623 | 377.961, 327.06, 102.566 | 2678.844, 3284.787, 54.241 | 1733.24, 6414.755, 34.037 | -3042.428, 588.687, 6.909 | -3243.83, 1005.144, 11.831 | 1164.038, 2707.052, 37.158");
            Drinks = ini.ReadString("Stores", "Drinks", "eCola: 2 | Pisswasser: 2 | Sprunk: 2");
            Foods = ini.ReadString("Stores", "Foods", "EgoChaser: 3 | Meteorite Bar: 5 | P's & Q's: 2");
            
            AutoConsume = ini.ReadBoolean("Other", "AutoConsume", false);
            StoreKey = ini.ReadEnum("Other", "StoreKey", Keys.E);
            InventoryKey = ini.ReadEnum("Other", "InventoryKey", Keys.I);
            InventoryModifier = ini.ReadEnum("Other", "InventoryModifier", Keys.LControlKey);
            Inventory = ini.ReadString("Other", "Inventory", "2 | 0 | 0 | 0 | 4 | 0");

            FontFamily = ini.ReadEnum("Font", "FontFamily", Common.EFont.Pricedown);
            FontSize = ini.ReadSingle("Font", "FontSize", 0.4f);

            Game.LogTrivial($"Plugin settings loaded.");
        }

        internal static void SaveSettings(string section, string key, string value) {
            ini.Write(section, key, value);
        }
    }
}