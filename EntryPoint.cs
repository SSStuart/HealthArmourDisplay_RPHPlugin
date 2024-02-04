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

                // Waiting for the player to "exist"
                while (!Game.LocalPlayer.Character.IsOnScreen)
                {
                    GameFiber.Yield();
                }
                Ped player = Game.LocalPlayer.Character;
                int playerMaxHealth = player.MaxHealth - 100;
                int screenHeight = Game.Resolution.Height;
                int screenWidth = Game.Resolution.Width;
                Point offset = new Point(Settings.BaseOffsetHorizontal, screenHeight - Settings.BaseOffsetVertical);

                while (true)
                {
                    GameFiber.Yield();


                    if (player.IsAlive)
                    {
                        // CIRCLE MINIMAP
                        // Health
                        ResText.Draw(((player.Health-100)*100/playerMaxHealth).ToString(), new Point(offset.X + Settings.HealthTextHorizontal, offset.Y + Settings.HealthTextVertical), 0.3f, healthColor, Common.EFont.Pricedown, false);
                        Sprite.Draw("commonmenu", "shop_health_icon_b", new Point(offset.X + Settings.HealthIconHorizontal, offset.Y + Settings.HealthIconVertical), new Size(50, 50), 0f, healthColor);

                        //Armour
                        Sprite.Draw("commonmenu", "shop_armour_icon_b", new Point(offset.X + Settings.ArmourIconHorizontal, offset.Y + Settings.ArmourIconVertical), new Size(50, 50), 0f, armourColor);
                        ResText.Draw(player.Armor.ToString(), new Point(offset.X + Settings.ArmourTextHorizontal, offset.Y + Settings.ArmourTextVertical), 0.3f, armourColor, Common.EFont.Pricedown, true);


                        // SQUARE MINIMAP
                        /*Sprite.Draw("commonmenu", "bettingbox_centre", new Point(offset.X - 40, offset.Y + 4), new Size(270, 50), 0f, Color.FromArgb(100, Color.Black));

                        Sprite.Draw("commonmenu", "shop_health_icon_b", offset, new Size(50, 50), 0f, Color.LightCoral);
                        ResText.Draw((player.Health-100).ToString(), new Point(offset.X + 60, offset.Y + 12), 0.3f, Color.LightCoral, Common.EFont.ChaletLondon, true);

                        Sprite.Draw("commonmenu", "shop_armour_icon_b", new Point(offset.X + 100, offset.Y), new Size(50, 50), 0f, Color.LightBlue);
                        ResText.Draw(player.Armor.ToString(), new Point(offset.X + 160, offset.Y + 12), 0.3f, Color.LightBlue, Common.EFont.ChaletLondon, true);*/
                    }
                }
            });
        }
    }
}
