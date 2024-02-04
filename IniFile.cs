using Rage;

namespace HealthArmourDisplay
{
    internal static class Settings
    {
        internal static int BaseOffsetVertical = 45;
        internal static int BaseOffsetHorizontal = 65;
        internal static int HealthTextVertical = 15;
        internal static int HealthTextHorizontal = 0;
        internal static int HealthIconVertical = 0;
        internal static int HealthIconHorizontal = 30;
        internal static int ArmourTextVertical = 15;
        internal static int ArmourTextHorizontal = 260;
        internal static int ArmourIconVertical = 0;
        internal static int ArmourIconHorizontal = 200;



        internal static string path = "Plugins/HealthArmourDisplay.ini";
        internal static InitializationFile ini = new InitializationFile(path);

        internal static void LoadSettings()
        {
            Game.LogTrivial("[LOG]: Loading config file for " + EntryPoint.pluginName + ".");
            
            ini.Create();
            BaseOffsetVertical = ini.ReadInt16("BasePosition", "BaseOffsetVertical", 45);
            BaseOffsetHorizontal = ini.ReadInt16("BasePosition", "BaseOffsetHorizontal", 65);
            HealthTextVertical = ini.ReadInt16("HealthTextPosition", "HealthTextVertical", 15);
            HealthTextHorizontal = ini.ReadInt16("HealthTextPosition", "HealthTextHorizontal", 0);
            HealthIconVertical = ini.ReadInt16("HealthIconPosition", "HealthIconVertical", 0);
            HealthIconHorizontal = ini.ReadInt16("HealthIconPosition", "HealthIconHorizontal", 30);
            ArmourTextVertical = ini.ReadInt16("ArmourTextPosition", "ArmourTextVertical", 15);
            ArmourTextHorizontal = ini.ReadInt16("ArmourTextPosition", "ArmourTextHorizontal", 260);
            ArmourIconVertical = ini.ReadInt16("ArmourIconPosition", "ArmourIconVertical", 0);
            ArmourIconHorizontal = ini.ReadInt16("ArmourIconPosition", "ArmourIconHorizontal", 200);
        }
    }
}