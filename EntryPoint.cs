using Rage;
using RAGENativeUI;
using RAGENativeUI.Elements;
using RAGENativeUI.PauseMenu;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

[assembly: Rage.Attributes.Plugin("HAHTDisplay", Description = "A plugin displaying the player's life and armour with an icon and its value. Also add an hunger and thirst system", Author = "SSStuart", PrefersSingleInstance = true, SupportUrl = "https://ssstuart.net/discord")]

namespace HealthArmourDisplay
{
    public class EntryPoint
    {
        public static string pluginName = "HAHTDisplay";
        public static string pluginVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

        private static MenuPool myMenuPool;
        private static UIMenu storeMenu;
        private static UIMenu inventoryMenu;
        private static readonly List<List<float>> storesLocations = new List<List<float>>();
        private static bool storeHelpDisplayed = false;
        private static List<List<string>> foodsAndDrinks;

        //private static Rage.Graphics graphics;

        public static void Main()
        {
            Game.LogTrivial($"{pluginName} Plugin v{pluginVersion} has been loaded.");

            Settings.LoadSettings();
            Game.LogTrivial($"[{pluginName}] Plugin settings loaded.");

            UpdateChecker.CheckForUpdates();

            GameFiber.StartNew(delegate
            {
                Random random = new Random();
                
                string patternRGB = @"^\d{1,3}\s?,\s?\d{1,3}\s?,\s?\d{1,3}$";
                Color healthColor, armourColor, hungerColor, thirstColor;
                if (Regex.Match(Settings.HealthColor, patternRGB, RegexOptions.IgnorePatternWhitespace).Success)
                {
                    string[] configColor = Settings.HealthColor.Split(',');
                    int[] configColorInt = Array.ConvertAll(configColor, int.Parse);
                    healthColor = Color.FromArgb(configColorInt[0], configColorInt[1], configColorInt[2]);
                }
                else if (Enum.TryParse(Settings.HealthColor, out HudColor healthColorHUD))
                {
                    healthColor = healthColorHUD.GetColor();
                }
                else
                {
                    healthColor = HudColor.RadarHealth.GetColor();
                }
                if (Regex.Match(Settings.ArmourColor, patternRGB, RegexOptions.IgnorePatternWhitespace).Success) {
                    string[] configColor = Settings.ArmourColor.Split(',');
                    int[] configColorInt = Array.ConvertAll(configColor, int.Parse);
                    armourColor = Color.FromArgb(configColorInt[0], configColorInt[1], configColorInt[2]);
                }
                else if (Enum.TryParse(Settings.ArmourColor, true, out HudColor armourColorHUD))
                {
                    armourColor = armourColorHUD.GetColor();
                }
                else
                {
                    armourColor = HudColor.RadarArmour.GetColor();
                }
                if (Regex.Match(Settings.HungerColor, patternRGB, RegexOptions.IgnorePatternWhitespace).Success) {
                    string[] configColor = Settings.HungerColor.Split(',');
                    int[] configColorInt = Array.ConvertAll(configColor, int.Parse);
                    hungerColor = Color.FromArgb(configColorInt[0], configColorInt[1], configColorInt[2]);
                }
                else if (Enum.TryParse(Settings.HungerColor, true, out HudColor hungerColorHUD))
                {
                    hungerColor = hungerColorHUD.GetColor();
                }
                else
                {
                    hungerColor = HudColor.OrangeLight.GetColor();
                }
                if (Regex.Match(Settings.ThirstColor, patternRGB, RegexOptions.IgnorePatternWhitespace).Success) {
                    string[] configColor = Settings.ThirstColor.Split(',');
                    int[] configColorInt = Array.ConvertAll(configColor, int.Parse);
                    thirstColor = Color.FromArgb(configColorInt[0], configColorInt[1], configColorInt[2]);
                }
                else if (Enum.TryParse(Settings.ThirstColor, true, out HudColor thirstColorHUD))
                {
                    thirstColor = thirstColorHUD.GetColor();
                }
                else
                {
                    thirstColor = HudColor.BlueLight.GetColor();
                }

                Common.EFont fontFamily = Settings.FontFamily;
                float fontSize = Settings.FontSize;
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


                foodsAndDrinks = new List<List<string>>();
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

                int savedInventoryLenght = Settings.Inventory.Split('|').Length;
                if (savedInventoryLenght == foodsAndDrinks.Count) {
                    int itemindex = 0;
                    foreach (var inventoryItem in foodsAndDrinks) {
                        inventoryItem[3] = Settings.Inventory.Split('|')[itemindex].Trim();
                        itemindex++;
                    }
                }

                int playerMaxHealth = Game.LocalPlayer.Character.MaxHealth - 100;    // Player is killed by game when player health is under 100
                double playerHealthPercent;
                Point offset = new Point(Settings.BaseOffsetHorizontal, Game.Resolution.Height - Settings.BaseOffsetVertical);

                Game.LogTrivial("[" + pluginName + "] Stores Locations :");
                foreach (var loc in storesLocations)
                {
                    Game.LogTrivial(loc[0] + " " + loc[1] + " " + loc[2]);
                }
                Game.LogTrivial("[" + pluginName + "] ------------------");

                // Menu
                myMenuPool = new MenuPool();

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
                    UIMenuItem storeItem = new UIMenuItem(foodAndDrink[0].Trim(), "$" + foodAndDrink[1].Trim())
                    {
                        ForeColor = itemcolor
                    };
                    storeMenu.AddItem(storeItem);
                    storeItem.Activated += (menu, item) =>
                    {
                        foodsAndDrinks[storeMenu.CurrentSelection][3] = (int.Parse(foodsAndDrinks[storeMenu.CurrentSelection][3]) + 1).ToString();
                        SaveInventory();
                    };
                }

                inventoryMenu = new UIMenu("Inventory", "~b~Consumables");

                myMenuPool.Add(inventoryMenu);

                foreach (var foodAndDrink in foodsAndDrinks)
                {
                    Color itemcolor;
                    if (foodAndDrink[2] == "food")
                        itemcolor = hungerColor;
                    else
                        itemcolor = thirstColor;
                    UIMenuItem invItem = new UIMenuItem(foodAndDrink[0].Trim(), foodAndDrink[3] + " in your inventory")
                    {
                        ForeColor = itemcolor
                    };
                    if (int.Parse(foodAndDrink[3]) < 1)
                        invItem.Enabled = false;
                    inventoryMenu.AddItem(invItem);
                    invItem.Activated += (menu, item) =>
                    {
                        foodsAndDrinks[inventoryMenu.CurrentSelection][3] = (int.Parse(foodsAndDrinks[inventoryMenu.CurrentSelection][3]) - 1).ToString();
                        if (foodsAndDrinks[inventoryMenu.CurrentSelection][2] == "food")
                        {
                            if (Settings.HungerAnimationEnabled)
                            {
                                Rage.Object foodItem = new Rage.Object(Settings.HungerAnimProp, Game.LocalPlayer.Character.Position)
                                {
                                    IsPersistent = false
                                };
                                foodItem.AttachTo(Game.LocalPlayer.Character, Game.LocalPlayer.Character.GetBoneIndex(PedBoneId.RightHand), new Vector3(0.14f, 0.05f, -0.04f), new Rotator(0f, 0f, 0f));

                                Game.LocalPlayer.Character.Tasks.PlayAnimation(Settings.HungerAnimDictio, Settings.HungerAnimName, 8f, AnimationFlags.UpperBodyOnly).WaitForCompletion();
                                foodItem.Detach();
                            }
                            hunger = Math.Min(hunger + 50, 100);
                        }
                        else if (foodsAndDrinks[inventoryMenu.CurrentSelection][2] == "drink")
                        {
                            if (Settings.ThirstAnimationEnabled)
                            {
                                Rage.Object drinkBottle = new Rage.Object(Settings.ThirstAnimProp, Game.LocalPlayer.Character.Position)
                                {
                                    IsPersistent = false
                                };
                                drinkBottle.AttachTo(Game.LocalPlayer.Character, Game.LocalPlayer.Character.GetBoneIndex(PedBoneId.RightHand), new Vector3(0.14f, 0.03f, -0.03f), new Rotator(90f, 180f, 0f));

                                Game.LocalPlayer.Character.Tasks.PlayAnimation(Settings.ThirstAnimDictio, Settings.ThirstAnimName, 1f, AnimationFlags.UpperBodyOnly).WaitForCompletion();
                                drinkBottle.Detach();
                            }
                            thirst = Math.Min(thirst + 50, 100);
                        }
                        UpdateInventoryMenu();
                    };
                }

                while (true)
                {
                    GameFiber.Yield();

                    if (Game.LocalPlayer.Character.IsAlive && !Game.IsPaused)
                    {
                        // CIRCLE MINIMAP
                        // Health
                        playerHealthPercent = (Game.LocalPlayer.Character.Health - 100) * 100 / playerMaxHealth;
                        //   Border
                        Sprite.Draw("commonmenu", "shop_health_icon_a", new Point(offset.X + Settings.HealthIconHorizontal - 4, offset.Y - Settings.HealthIconVertical - 4), new Size(58, 58), 0f, Color.FromArgb(Math.Min(255 - (int)(playerHealthPercent * 2.5), 255), Color.Red));
                        //   Text
                        ResText.Draw(playerHealthPercent.ToString(), new Point(offset.X + Settings.HealthTextHorizontal, offset.Y - Settings.HealthTextVertical), fontSize, healthColor, Common.EFont.Pricedown, false);
                        //   Icon
                        Sprite.Draw("commonmenu", "shop_health_icon_b", new Point(offset.X + Settings.HealthIconHorizontal, offset.Y - Settings.HealthIconVertical), new Size(50, 50), 0f, healthColor);

                        // Armour
                        //   Border
                        Sprite.Draw("commonmenu", "shop_armour_icon_a", new Point(offset.X + Settings.ArmourIconHorizontal - 4, offset.Y - Settings.ArmourIconVertical - 4), new Size(58, 58), 0f, Color.FromArgb(Math.Min((int)(Game.LocalPlayer.Character.Armor * 2.5), 255), armourColor));
                        //   Icon
                        Sprite.Draw("commonmenu", "shop_armour_icon_b", new Point(offset.X + Settings.ArmourIconHorizontal, offset.Y - Settings.ArmourIconVertical), new Size(50, 50), 0f, Color.FromArgb(Game.LocalPlayer.Character.Armor == 0 ? 50 : 255, armourColor));
                        //   Text
                        ResText.Draw(Game.LocalPlayer.Character.Armor.ToString(), new Point(offset.X + Settings.ArmourTextHorizontal, offset.Y - Settings.ArmourTextVertical), fontSize, Color.FromArgb(Game.LocalPlayer.Character.Armor == 0 ? 50 : 255, armourColor), Common.EFont.Pricedown, false);

                        // Hunger
                        //   Icon
                        Sprite.Draw("commonmenu", "shop_health_icon_b", new Point(offset.X + Settings.HungerIconHorizontal, offset.Y - Settings.HungerIconVertical), new Size(50, 50), 0f, hungerColor);
                        //   Text
                        ResText.Draw(hunger.ToString(), new Point(offset.X + Settings.HungerTextHorizontal, offset.Y - Settings.HungerTextVertical), fontSize, hungerColor, Common.EFont.Pricedown, false);

                        // Thirst
                        //   Text
                        ResText.Draw(thirst.ToString(), new Point(offset.X + Settings.ThirstTextHorizontal, offset.Y - Settings.ThirstTextVertical), fontSize, thirstColor, Common.EFont.Pricedown, false);
                        //   Icon
                        Sprite.Draw("commonmenu", "shop_health_icon_b", new Point(offset.X + Settings.ThirstIconHorizontal, offset.Y - Settings.ThirstIconVertical), new Size(50, 50), 0f, thirstColor);


                        // SQUARE MINIMAP
                        /*Sprite.Draw("commonmenu", "bettingbox_centre", new Point(offset.X - 40, offset.Y + 4), new Size(270, 50), 0f, Color.FromArgb(100, Color.Black));

                        Sprite.Draw("commonmenu", "shop_health_icon_b", offset, new Size(50, 50), 0f, Color.LightCoral);
                        ResText.Draw((Game.LocalPlayer.Character.Health-100).ToString(), new Point(offset.X + 60, offset.Y + 12), 0.3f, Color.LightCoral, Common.EFont.ChaletLondon, true);

                        Sprite.Draw("commonmenu", "shop_armour_icon_b", new Point(offset.X + 100, offset.Y), new Size(50, 50), 0f, Color.LightBlue);
                        ResText.Draw(Game.LocalPlayer.Character.Armor.ToString(), new Point(offset.X + 160, offset.Y + 12), 0.3f, Color.LightBlue, Common.EFont.ChaletLondon, true);*/

                        switch (Game.LocalPlayer.Character)
                        {
                            case Ped player when player.IsSprinting:
                                hungerDepletionMult = 0.2f;
                                thirstDepletionMult = 0.3f;
                                break;
                            case Ped player when player.IsRunning:
                                hungerDepletionMult = 0.5f;
                                thirstDepletionMult = 0.7f;
                                break;
                            case Ped player when player.IsSwimming:
                                hungerDepletionMult = 0.8f;
                                thirstDepletionMult = 0.8f;
                                break;
                            case Ped player when player.IsDiving:
                                hungerDepletionMult = 0.3f;
                                thirstDepletionMult = 0.5f;
                                break;
                            default:
                                hungerDepletionMult = 1.0f;
                                thirstDepletionMult = 1.0f;
                                break;
                        }


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
                                    Game.DisplayHelp("Press ~b~"+Settings.StoreKey+"~w~ to buy food and drinks");
                                    storeHelpDisplayed = true;
                                }else { storeHelpDisplayed = false;}
                            }
                        });

                        if (lastHungerUpdate + Settings.HungerDepletionSpeed * 100 * hungerDepletionMult < Game.GameTime)
                        {
                            lastHungerUpdate = Game.GameTime;
                            if (hunger < 20 && Settings.AutoConsume)
                            {
                                foreach (var foodAndDrink in foodsAndDrinks)
                                {
                                    if (foodAndDrink[2] == "food" && int.Parse(foodAndDrink[3]) > 0)
                                    {
                                        foodsAndDrinks[foodsAndDrinks.IndexOf(foodAndDrink)][3] = (int.Parse(foodAndDrink[3]) - 1).ToString();
                                        hunger = Math.Min(hunger + 50, 100);
                                        break;
                                    }
                                }
                            }
                            if (hunger > 0)
                                hunger--;
                            else if (hunger == 0)
                            {
                                int randomValue = random.Next(0, 100);
                                if (randomValue > (Game.LocalPlayer.Character.Health - 75))
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

                        if (lastThirstUpdate + Settings.ThirstDepletionSpeed * 100 * thirstDepletionMult < Game.GameTime)
                        {
                            lastThirstUpdate = Game.GameTime;
                            if (thirst < 20 && Settings.AutoConsume)
                            {
                                foreach (var foodAndDrink in foodsAndDrinks)
                                {
                                    if (foodAndDrink[2] == "drink" && int.Parse(foodAndDrink[3]) > 0)
                                    {
                                        foodsAndDrinks[foodsAndDrinks.IndexOf(foodAndDrink)][3] = (int.Parse(foodAndDrink[3]) - 1).ToString();
                                        thirst = Math.Min(thirst + 50, 100);
                                        break;
                                    }
                                }
                            }
                            if (thirst > 0)
                                thirst--;
                            else if (thirst == 0)
                            {
                                int randomValue = random.Next(0, 100);
                                if (randomValue > (Game.LocalPlayer.Character.Health - 80))
                                {
                                    Game.FadeScreenOut(1500, true);
                                    Game.FadeScreenIn(2000, true);
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

                if (Game.IsKeyDown(Settings.StoreKey) && !Game.IsControlKeyDownRightNow) // the open/close trigger
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

                if (Game.IsKeyDown(Settings.InventoryKey) && Game.IsKeyDownRightNow(Settings.InventoryModifier)) {
                    if (inventoryMenu.Visible) {
                        inventoryMenu.Visible = false;
                    } else if (!UIMenu.IsAnyMenuVisible && !TabView.IsAnyPauseMenuVisible) {
                        UpdateInventoryMenu();

                        inventoryMenu.Visible = true;
                    }
                }
            }
        }

        private static void UpdateInventoryMenu()
        {
            for (int i = 0; i < foodsAndDrinks.Count; i++)
            {
                if (int.Parse(foodsAndDrinks[i][3]) < 1)
                {
                    inventoryMenu.MenuItems[i].Enabled = false;
                    inventoryMenu.MenuItems[i].Description = "You don't have any " + foodsAndDrinks[i][0].Trim() + " in your inventory";
                } else
                {
                    inventoryMenu.MenuItems[i].Enabled = true;
                    inventoryMenu.MenuItems[i].Description = "You have " + foodsAndDrinks[i][3] + " " + foodsAndDrinks[i][0].Trim() + " in your inventory";
                }
            }

            SaveInventory();
        }

        private static void SaveInventory()
        {
            string inventoryString = "";
            foreach (List<string> inventoryItem in foodsAndDrinks)
            {
                inventoryString += inventoryItem[3] + " | ";
            }
            inventoryString = inventoryString.Remove(inventoryString.Length - 3);
            Settings.SaveSettings("Other", "Inventory", inventoryString);
        }

        /*static void drawSprites(object sender, GraphicsEventArgs e)
        {
            Sprite.DrawTexture(Game.CreateTextureFromFile(@"D:\Jeux\Rockstar Games\Grand Theft Auto V\plugins\thirst.png"), new Point(10, 10), new Size(100, 100), e.Graphics);
            Game.RawFrameRender -= drawSprites;
        }*/
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
