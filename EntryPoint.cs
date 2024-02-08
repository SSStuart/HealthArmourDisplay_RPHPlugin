using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using RAGENativeUI.PauseMenu;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

[assembly: Rage.Attributes.Plugin("Health & Armour Display", Description = "A plugin displaying the player's life and armour with an icon and its value", Author = "SSStuart")]


namespace HealthArmourDisplay
{
    public class EntryPoint
    {
        public static string pluginName = "Health & Armour Display";
        public static string pluginVersion = "v 0.0.1";

        static MenuPool myMenuPool = new MenuPool();
        static UIMenu storeMenu;
        static List<List<float>> storesLocations = new List<List<float>>();
        static bool storeHelpDisplayed = false;

        //private static Rage.Graphics graphics;

        public static void Main()
        {
            Game.LogTrivial(pluginName + " loaded.");

            Settings.LoadSettings();
            Game.LogTrivial("[" + pluginName + "] Plugin settings loaded.");

            GameFiber.StartNew(delegate
            {
                Random random = new Random();
                Color healthColor = HudColor.RadarHealth.GetColor();
                Color armourColor = HudColor.RadarArmour.GetColor();
                Color hungerColor = HudColor.OrangeLight.GetColor();
                Color thirstColor = HudColor.BlueLight.GetColor();
                float fontSize = 0.4f;
                int hunger = 100;
                int thirst = 100;
                float hungerDepletionMult = 1;
                float thirstDepletionMult = 1.5f;
                uint lastHungerUpdate = 0;
                uint lastThirstUpdate = 0;

               
                List<string> storesLocationsString = Settings.StoreLocations.Split('|').ToList();
                foreach (var coordinates in storesLocationsString)
                {
                    bool xOk = float.TryParse(coordinates.Split(',')[0], out float x);
                    bool yOk = float.TryParse(coordinates.Split(',')[1], out float y);
                    bool zOk = float.TryParse(coordinates.Split(',')[2], out float z);
                    if (xOk && yOk && zOk)
                        storesLocations.Add(new List<float> { x, y, z });
                    else
                        Game.LogTrivial("[" + pluginName + "] Error parsing store location : " + coordinates);
                }


                List<List<string>> foodsAndDrinks = new List<List<string>>();
                List<string> drinksString = Settings.Drinks.Split('|').ToList();
                foreach (var drink in drinksString)
                {
                    List<string> drinkList = new List<string>(drink.Split(':').ToList())
                    {
                        "drink",
                        "0"
                    };
                    foodsAndDrinks.Add(drinkList);
                }
                List<string> foodsString = Settings.Foods.Split('|').ToList();
                foreach (var food in foodsString)
                {
                    List<string> foodList = new List<string>(food.Split(':').ToList())
                    {
                        "food",
                        "0"
                    };
                    foodsAndDrinks.Add(foodList);
                }


                //Game.RawFrameRender += drawSprites;

                while (Game.IsScreenFadedOut || !Game.LocalPlayer.Character.Exists() || Game.IsLoading)
                {
                    GameFiber.Yield();
                }
                Game.LogTrivial("===========LOADED===========");
                //Ped player = Game.LocalPlayer.Character;
                int playerMaxHealth = Game.LocalPlayer.Character.MaxHealth - 100;
                double playerHealthPercent;
                Point offset = new Point(Settings.BaseOffsetHorizontal, Game.Resolution.Height - Settings.BaseOffsetVertical);

                Game.LogTrivial("[" + pluginName + "] Stores Locs : ");
                foreach (var loc in storesLocations)
                {
                    Game.LogTrivial(loc[0] + " " + loc[1] + " " + loc[2]);
                }
                Game.LogTrivial("[" + pluginName + "] --------------");

                // Menu
                storeMenu = new UIMenu("Store", "~b~Drinks and snacks");

                myMenuPool.Add(storeMenu);

                // start the fiber which will handle drawing and processing the menus
                GameFiber.StartNew(ProcessMenus);

                foreach (var foodAndDrink in foodsAndDrinks)
                {
                    Color itemcolor;
                    if (foodAndDrink[2] == "food")
                        itemcolor = hungerColor;
                    else
                        itemcolor = thirstColor;
                    UIMenuItem item = new UIMenuItem(foodAndDrink[0], "$"+foodAndDrink[1]);
                    item.ForeColor = itemcolor;
                    storeMenu.AddItem(item);
                    storeMenu.OnItemSelect += (sender, selectedItem, index) =>
                    {
                        foodsAndDrinks[index][3] = (int.Parse(foodsAndDrinks[index][3]) + 1).ToString();
                        Game.LocalPlayer.Character.Money -= int.Parse(foodsAndDrinks[index][1]);
                    };
                }


                Game.LogTrivial(foodsAndDrinks.Count.ToString());


                while (true)
                {
                    GameFiber.Yield();

                    if (Game.LocalPlayer.Character.IsAlive)
                    {
                        // CIRCLE MINIMAP
                        // Health
                        playerHealthPercent = (Game.LocalPlayer.Character.Health - 100) * 100 / playerMaxHealth;
                        Sprite.Draw("commonmenu", "shop_health_icon_a", new Point(offset.X + Settings.HealthIconHorizontal - 4, offset.Y - Settings.HealthIconVertical - 4), new Size(58, 58), 0f, Color.FromArgb(255-(int)(playerHealthPercent*2.5), Color.Red));
                        ResText.Draw(playerHealthPercent.ToString(), new Point(offset.X + Settings.HealthTextHorizontal, offset.Y - Settings.HealthTextVertical), fontSize, healthColor, Common.EFont.Pricedown, false);
                        Sprite.Draw("commonmenu", "shop_health_icon_b", new Point(offset.X + Settings.HealthIconHorizontal, offset.Y - Settings.HealthIconVertical), new Size(50, 50), 0f, healthColor);

                        // Armour
                        Sprite.Draw("commonmenu", "shop_armour_icon_a", new Point(offset.X + Settings.ArmourIconHorizontal - 4, offset.Y - Settings.ArmourIconVertical - 4), new Size(58, 58), 0f, Color.FromArgb((int)(Game.LocalPlayer.Character.Armor * 2.5), armourColor));
                        Sprite.Draw("commonmenu", "shop_armour_icon_b", new Point(offset.X + Settings.ArmourIconHorizontal, offset.Y - Settings.ArmourIconVertical), new Size(50, 50), 0f, Color.FromArgb(Game.LocalPlayer.Character.Armor == 0 ? 50 : 255, armourColor));
                        ResText.Draw(Game.LocalPlayer.Character.Armor.ToString(), new Point(offset.X + Settings.ArmourTextHorizontal, offset.Y - Settings.ArmourTextVertical), fontSize, Color.FromArgb(Game.LocalPlayer.Character.Armor == 0 ? 50 : 255, armourColor), Common.EFont.Pricedown, false);

                        // Hunger
                        Sprite.Draw("commonmenu", "shop_health_icon_b", new Point(offset.X + Settings.HungerIconHorizontal, offset.Y - Settings.HungerIconVertical), new Size(50, 50), 0f, hungerColor);
                        ResText.Draw(hunger.ToString(), new Point(offset.X + Settings.HungerTextHorizontal, offset.Y - Settings.HungerTextVertical), fontSize, hungerColor, Common.EFont.Pricedown, false);

                        // Thirst
                        Sprite.Draw("commonmenu", "shop_health_icon_b", new Point(offset.X + Settings.ThirstIconHorizontal, offset.Y - Settings.ThirstIconVertical), new Size(50, 50), 0f, thirstColor);
                        ResText.Draw(thirst.ToString(), new Point(offset.X + Settings.ThirstTextHorizontal, offset.Y - Settings.ThirstTextVertical), fontSize, thirstColor, Common.EFont.Pricedown, false);



                        // SQUARE MINIMAP
                        /*Sprite.Draw("commonmenu", "bettingbox_centre", new Point(offset.X - 40, offset.Y + 4), new Size(270, 50), 0f, Color.FromArgb(100, Color.Black));

                        Sprite.Draw("commonmenu", "shop_health_icon_b", offset, new Size(50, 50), 0f, Color.LightCoral);
                        ResText.Draw((player.Health-100).ToString(), new Point(offset.X + 60, offset.Y + 12), 0.3f, Color.LightCoral, Common.EFont.ChaletLondon, true);

                        Sprite.Draw("commonmenu", "shop_armour_icon_b", new Point(offset.X + 100, offset.Y), new Size(50, 50), 0f, Color.LightBlue);
                        ResText.Draw(player.Armor.ToString(), new Point(offset.X + 160, offset.Y + 12), 0.3f, Color.LightBlue, Common.EFont.ChaletLondon, true);*/

                        hungerDepletionMult = Game.LocalPlayer.Character.IsSprinting ? 0.2f : Game.LocalPlayer.Character.IsRunning ? 0.5f : 1f;
                        thirstDepletionMult = Game.LocalPlayer.Character.IsSprinting ? 0.1f : Game.LocalPlayer.Character.IsRunning ? 0.8f : 1.2f;


                        // Create a new game fiber to show the stores locations
                        GameFiber.StartNew(delegate
                        {
                            foreach (var loc in storesLocations)
                            {
                                Vector3 storeLocation = new Vector3(loc[0], loc[1], loc[2]);
                                float distance = Game.LocalPlayer.Character.Position.DistanceTo(storeLocation);
                                if (distance < 50)
                                {
                                    Rage.Native.NativeFunction.Natives.DRAW_MARKER(1, storeLocation.X, storeLocation.Y, storeLocation.Z, 0f, 0f, 0f, 0f, 0f, 0f, 2f, 2f, 1f, 255, 194, 170, 100, false, false, 2, true, 0, 0, false);
                                }
                                if (distance < 2 && !storeHelpDisplayed)
                                {
                                    Game.DisplayHelp("Press ~b~E~w~ to buy food and drinks");
                                    storeHelpDisplayed = true;
                                }else { storeHelpDisplayed = false;}
                            }
                        });

                        if (lastHungerUpdate + Settings.HungerDepletionSpeed * 100 * hungerDepletionMult < Game.GameTime)
                        {
                            lastHungerUpdate = Game.GameTime;
                            if (hunger > 0)
                                hunger--;
                            if (hunger == 0)

                            {
                                int randomValue = random.Next(0, 100);
                                if (randomValue > (Game.LocalPlayer.Character.Health - 50))
                                {
                                    Game.LocalPlayer.Character.IsRagdoll = true;
                                }
                                if (randomValue > 10 && randomValue < 50)
                                {
                                    Game.LocalPlayer.Character.Health -= 1;
                                }
                                else if (randomValue > 50 && Game.LocalPlayer.Character.IsRagdoll)
                                {
                                    Game.LocalPlayer.Character.IsRagdoll = false;
                                }
                            }
                        }

                        if (lastThirstUpdate + Settings.ThristDepletionSpeed * 100 * thirstDepletionMult < Game.GameTime)
                        {
                            lastThirstUpdate = Game.GameTime;
                            if (thirst > 0)
                                thirst--;
                            if (thirst == 0)

                            {
                                int randomValue = random.Next(0, 100);
                                if (randomValue > (Game.LocalPlayer.Character.Health - 50))
                                {
                                    Game.FadeScreenOut(1500, true);
                                    Game.FadeScreenIn(1000, true);
                                }
                                if (randomValue > 10 && randomValue < 50)
                                {
                                    Game.LocalPlayer.Character.Health -= 1;
                                }
                            }
                        }
                    }
                    else if (Game.LocalPlayer.Character.IsDead)
                    {
                        hunger = 100;
                        thirst = 100;
                    }
                }
            });
        }

        private static void ProcessMenus()
        {
            while (true)
            {
                GameFiber.Yield();

                myMenuPool.ProcessMenus();

                bool playerIsInStore = false;
                foreach(var storePos in storesLocations)
                {
                    if (Game.LocalPlayer.Character.Position.DistanceTo(new Vector3(storePos[0], storePos[1], storePos[2])) < 2)
                    {
                        playerIsInStore = true;
                    }
                }

                if (Game.IsKeyDown(Keys.E)) // the open/close trigger
                {
                    if (storeMenu.Visible)
                    {
                        // close the menu
                        storeMenu.Visible = false;
                    }
                    else if (!UIMenu.IsAnyMenuVisible && !TabView.IsAnyPauseMenuVisible && playerIsInStore) // check that no menus are visible
                    {
                        // open the menu
                        storeMenu.Visible = true;
                    }
                }
            }
        }

        /*static void drawSprites(object sender, GraphicsEventArgs e)
        {
            Sprite.DrawTexture(Game.CreateTextureFromFile(@"D:\Jeux\Rockstar Games\Grand Theft Auto V\plugins\hunger.png"), new Point(10, 10), new Size(100, 100), e.Graphics);
        }*/
    }

    public static class ReloadOverlayCommand
    {
        [Rage.Attributes.ConsoleCommand]
        public static void Command_ReloadOverlay()
        {
            // Reload the plugin
            EntryPoint.Main();
        }
    }
}