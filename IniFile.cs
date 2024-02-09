using Rage;
using RAGENativeUI;

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
        internal static int ThristDepletionSpeed = 800;
        internal static int ThirstTextVertical = 200;
        internal static int ThirstTextHorizontal = 250;
        internal static int ThirstIconVertical = 215;
        internal static int ThirstIconHorizontal = 200;
        internal static string ThirstColor = "HudColor.BlueLight";
        internal static string StoreLocations = "";
        internal static string Drinks = "";
        internal static string Foods = "";
        internal static Common.EFont FontFamily = Common.EFont.Pricedown;
        internal static float FontSize = 0.4f;



        internal static string path = "Plugins/HealthArmourDisplay.ini";
        internal static InitializationFile ini = new InitializationFile(path);

        internal static void LoadSettings()
        {
            Game.LogTrivial("[LOG]: Loading config file for " + EntryPoint.pluginName + ".");

            ini.Create();
            BaseOffsetVertical = ini.ReadInt16("BasePosition", "BaseOffsetVertical", 45);
            BaseOffsetHorizontal = ini.ReadInt16("BasePosition", "BaseOffsetHorizontal", 65);

            HealthTextVertical = ini.ReadInt16("Health", "HealthTextVertical", -15);
            HealthTextHorizontal = ini.ReadInt16("Health", "HealthTextHorizontal", 0);
            HealthIconVertical = ini.ReadInt16("Health", "HealthIconVertical", 0);
            HealthIconHorizontal = ini.ReadInt16("Health", "HealthIconHorizontal", 30);
            HealthColor = ini.ReadString("Health", "Color", "HudColor.RadarHealth");
            
            ArmourTextVertical = ini.ReadInt16("Armour", "ArmourTextVertical", -15);
            ArmourTextHorizontal = ini.ReadInt16("Armour", "ArmourTextHorizontal", 250);
            ArmourIconVertical = ini.ReadInt16("Armour", "ArmourIconVertical", 0);
            ArmourIconHorizontal = ini.ReadInt16("Armour", "ArmourIconHorizontal", 200);
            ArmourColor = ini.ReadString("Armour", "Color", "HudColor.RadarArmour");
            
            HungerDepletionSpeed = ini.ReadInt16("Hunger", "HungerDepletionSpeed", 600);
            HungerTextVertical = ini.ReadInt16("Hunger", "HungerTextVertical", 200);
            HungerTextHorizontal = ini.ReadInt16("Hunger", "HungerTextHorizontal", 0);
            HungerIconVertical = ini.ReadInt16("Hunger", "HungerIconVertical", 215);
            HungerIconHorizontal = ini.ReadInt16("Hunger", "HungerIconHorizontal", 30);
            HungerColor = ini.ReadString("Hunger", "Color", "HudColor.OrangeLight");

            ThristDepletionSpeed = ini.ReadInt16("Thirst", "ThirstDepletionSpeed", 800);
            ThirstTextVertical = ini.ReadInt16("Thirst", "ThirstTextVertical", 200);
            ThirstTextHorizontal = ini.ReadInt16("Thirst", "ThirstTextHorizontal", 250);
            ThirstIconVertical = ini.ReadInt16("Thirst", "ThirstIconVertical", 215);
            ThirstIconHorizontal = ini.ReadInt16("Thirst", "ThirstIconHorizontal", 200);
            ThirstColor = ini.ReadString("Thirst", "Color", "HudColor.BlueLight");

            StoreLocations = ini.ReadString("Stores", "StoreLocations", "");
            Drinks = ini.ReadString("Stores", "Drinks", "");
            Foods = ini.ReadString("Stores", "Foods", "");

            FontFamily = ini.ReadEnum("Font", "FontFamily", Common.EFont.Pricedown);
            FontSize = ini.ReadSingle("Font", "FontSize", 0.4f);
        }
    }
}