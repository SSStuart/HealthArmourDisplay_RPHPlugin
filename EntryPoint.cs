using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using System.Drawing;

[assembly: Rage.Attributes.Plugin("Health & Armour Display", Description = "A plugin displaying the player's life and armour with an icon and its value", Author = "SSStuart")]


namespace HealthArmourDisplay
{
    public class EntryPoint
    {
        public static string pluginName = "Health & Armour Display";
        public static string pluginVersion = "v 0.0.1";

        public static void Main()
        {
            Game.LogTrivial(pluginName + " loaded.");

            Settings.LoadSettings();
            Game.LogTrivial("[" + pluginName + "] Plugin settings loaded.");

            GameFiber.StartNew(delegate
            {
                Color healthColor = Color.LightCoral;
                Color armourColor = Color.DeepSkyBlue;
                float fontSize = 0.4f;

                // Waiting for the player to "exist"
                /*while (!Game.LocalPlayer.Character.IsOnScreen)
                {
                    GameFiber.Yield();
                }
                Ped player = Game.LocalPlayer.Character;*/

                int playerMaxHealth = Game.LocalPlayer.Character.MaxHealth - 100;
                double playerHealthPercent;
                Point offset = new Point(Settings.BaseOffsetHorizontal, Game.Resolution.Height - Settings.BaseOffsetVertical);

                while (true)
                {
                    GameFiber.Yield();


                    if (Game.LocalPlayer.Character.IsAlive)
                    {
                        // CIRCLE MINIMAP
                        // Health
                        playerHealthPercent = (Game.LocalPlayer.Character.Health - 100) * 100 / playerMaxHealth;
                        //   Border
                        Sprite.Draw("commonmenu", "shop_health_icon_a", new Point(offset.X + Settings.HealthIconHorizontal - 4, offset.Y - Settings.HealthIconVertical - 4), new Size(58, 58), 0f, Color.FromArgb(255 - (int)(playerHealthPercent * 2.5), Color.Red));
                        //   Text
                        ResText.Draw(playerHealthPercent.ToString(), new Point(offset.X + Settings.HealthTextHorizontal, offset.Y - Settings.HealthTextVertical), fontSize, healthColor, Common.EFont.Pricedown, false);
                        //   Icon
                        Sprite.Draw("commonmenu", "shop_health_icon_b", new Point(offset.X + Settings.HealthIconHorizontal, offset.Y - Settings.HealthIconVertical), new Size(50, 50), 0f, healthColor);

                        // Armour
                        //   Border
                        Sprite.Draw("commonmenu", "shop_armour_icon_a", new Point(offset.X + Settings.ArmourIconHorizontal - 4, offset.Y - Settings.ArmourIconVertical - 4), new Size(58, 58), 0f, Color.FromArgb((int)(Game.LocalPlayer.Character.Armor * 2.5), armourColor));
                        //   Icon
                        Sprite.Draw("commonmenu", "shop_armour_icon_b", new Point(offset.X + Settings.ArmourIconHorizontal, offset.Y - Settings.ArmourIconVertical), new Size(50, 50), 0f, Color.FromArgb(Game.LocalPlayer.Character.Armor == 0 ? 50 : 255, armourColor));
                        //   Text
                        ResText.Draw(Game.LocalPlayer.Character.Armor.ToString(), new Point(offset.X + Settings.ArmourTextHorizontal, offset.Y - Settings.ArmourTextVertical), fontSize, Color.FromArgb(Game.LocalPlayer.Character.Armor == 0 ? 50 : 255, armourColor), Common.EFont.Pricedown, false);


                        // SQUARE MINIMAP
                        /*Sprite.Draw("commonmenu", "bettingbox_centre", new Point(offset.X - 40, offset.Y + 4), new Size(270, 50), 0f, Color.FromArgb(100, Color.Black));

                        Sprite.Draw("commonmenu", "shop_health_icon_b", offset, new Size(50, 50), 0f, Color.LightCoral);
                        ResText.Draw((Game.LocalPlayer.Character.Health-100).ToString(), new Point(offset.X + 60, offset.Y + 12), 0.3f, Color.LightCoral, Common.EFont.ChaletLondon, true);

                        Sprite.Draw("commonmenu", "shop_armour_icon_b", new Point(offset.X + 100, offset.Y), new Size(50, 50), 0f, Color.LightBlue);
                        ResText.Draw(Game.LocalPlayer.Character.Armor.ToString(), new Point(offset.X + 160, offset.Y + 12), 0.3f, Color.LightBlue, Common.EFont.ChaletLondon, true);*/
                    }
                }
            });
        }
    }


    public static class ReloadHADOverlayCommand
    {
        [Rage.Attributes.ConsoleCommand]
        public static void Command_ReloadHADOverlay()
        {
            Game.ReloadActivePlugin();
        }
    }
}
