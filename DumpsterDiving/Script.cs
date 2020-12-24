using GTA;
using GTA.Math;
using GTA.UI;
using NAudio.Wave;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace DumpsterDiving
{
    /// <summary>
    /// The items that the player can get in the dumpsters.
    /// </summary>
    public enum Items
    {
        HotDog = 0,
        Hamburger = 1,
        MoldyHotDog = 2,
        MoldyHamburger = 3,
        Money = 4,
        Dildo = 5,
        Boot = 6,
        Fish = 7,
        Condom = 8,
        Pistol = 9,
        MicroSMG = 10,
        AssaultRifle = 11,
        Shotgun = 12,
        SawnOffShotgun = 13,
        Grenades = 14,
        BZGas = 15,
        TearGas = 16
    }

    /// <summary>
    /// Script that allows you to perform dumpster diving
    /// </summary>
    public class DumpsterDiving : Script
    {
        #region Fields

        /// <summary>
        /// The location of the game script.
        /// </summary>
        private static readonly string location = new Uri(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase)).LocalPath;
        /// <summary>
        /// The audio output device.
        /// </summary>
        private readonly WaveOutEvent output = new WaveOutEvent();
        /// <summary>
        /// The audio file that we are going to hear.
        /// </summary>
        private readonly AudioFileReader audioFile = new AudioFileReader(Path.Combine(location, "DumpsterDiving", "Search.mp3"));
        /// <summary>
        /// Our random number generator.
        /// </summary>
        private readonly Random generator = new Random();
        /// <summary>
        /// If the game information should be updated after the playback.
        /// </summary>
        private bool updateRequired = false;
        /// <summary>
        /// A list that contains models of dumpsters.
        /// </summary>
        private readonly List<Model> dumpsterModels = new List<Model>
        {
            new Model("prop_dumpster_01a"),
            new Model("prop_dumpster_02a"),
            new Model("prop_dumpster_02b"),
            new Model("prop_dumpster_04a"),
            new Model("prop_dumpster_4b"),
            new Model("prop_dumpster_3a")
        };
        /// <summary>
        /// The configuration for our current script.
        /// </summary>
        private Configuration config = new Configuration();
        /// <summary>
        /// The dumpsters that exist arround the map.
        /// </summary>
        private List<Prop> nearbyDumpsters = new List<Prop>();
        /// <summary>
        /// Next game time that we should update the lists of peds.
        /// </summary>
        private int nextFetch = 0;
        /// <summary>
        /// If the player has been notified about how to do the dumpster diving.
        /// </summary>
        private bool notified = false;
        /// <summary>
        /// If a dumpster has been found.
        /// </summary>
        private bool found = false;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new Dumpster Diving script.
        /// </summary>
        public DumpsterDiving()
        {
            // If the configuration file exists, load it
            // If not, create a new one
            string path = Path.Combine(location, "DumpsterDiving", "Config.json");
            if (File.Exists(path))
            {
                string contents = File.ReadAllText(path);
                config = JsonConvert.DeserializeObject<Configuration>(contents);
            }
            else
            {
                File.WriteAllText(path, JsonConvert.SerializeObject(config));
            }

            // Then just add the events
            Tick += OnTick;
            output.PlaybackStopped += OnPlaybackStopped;
        }

        #endregion

        #region Events

        private void OnTick(object sender, EventArgs e)
        {
            // Set found to false
            found = false;

            // If the current time is higher or equal than the next fetch time
            if (Game.GameTime >= nextFetch)
            {
                // Reset the list of dumpsters
                nearbyDumpsters = new List<Prop>();
                // Iterate over the dumpster models
                foreach (Model DumpsterModel in dumpsterModels)
                {
                    // Fill the list with all of those props
                    nearbyDumpsters.AddRange(World.GetAllProps(DumpsterModel));
                }
                // Finally, set the next fetch time to one second in the future
                nextFetch = Game.GameTime + 1000;
            }

            // If we need to update the playback
            if (updateRequired)
            {
                // Disable the update
                updateRequired = false;
                // Fade in
                Screen.FadeIn(config.Fade);
                // And unfreeze the player
                Game.Player.Character.IsPositionFrozen = false;
            }

            // Iterate over the stored dumpsters
            foreach (Prop DumpsterProp in nearbyDumpsters)
            {
                // If the user wants blips and the dumpster doesn't have one
                if (config.Blips && !DumpsterProp.AttachedBlip.Exists())
                {
                    // Create the blip
                    Blip Current = DumpsterProp.AddBlip();
                    // And set the properties of it
                    Current.Name = "Dumpster";
                    Current.Color = config.BlipColor;
                }

                // Get the position of the front
                Vector3 Front = DumpsterProp.GetOffsetPosition(new Vector3(0, -1f, 0));

                // If the distance is lower or equal to 25 units
                if (World.GetDistance(Game.Player.Character.Position, DumpsterProp.Position) <= config.MarkerDistance)
                {
                    // Draw a marker that will trigger the dumpster diving
                    World.DrawMarker(MarkerType.VerticalCylinder, Front, Vector3.Zero, Vector3.Zero, new Vector3(0.7f, 0.7f, 0.7f), config.MarkerColor);
                }

                // If the player is on foot
                if (Game.Player.Character.CurrentVehicle == null)
                {
                    // If the distance between the front and the player is lower or equal to 1.5
                    if (World.GetDistance(Game.Player.Character.Position, Front) <= config.LootDistance)
                    {
                        // If the user has not been notified
                        if (!notified)
                        {
                            // Show the user
                            Screen.ShowHelpTextThisFrame("Press ~INPUT_PICKUP~ to loot the dumpster.");
                            // And set the flag to true
                            notified = true;
                        }

                        // Set the found variable to true
                        found = true;

                        // If the player pressed the interact button
                        // DEV NOTE: Use GTA.Control.Whistle if Talk doesn't work
                        if (Game.IsControlJustPressed(Control.Talk))
                        {
                            // Fade the screen out and freeze the player
                            Screen.FadeOut(config.Fade);
                            Game.Player.Character.IsPositionFrozen = true;
                            // Wait for a seccond
                            Wait(1000);
                            // If the current time of the audio is the same as the total time
                            if (audioFile.CurrentTime == audioFile.TotalTime)
                            {
                                // Stop the playback and reset the playtime
                                output.Stop();
                                audioFile.CurrentTime = TimeSpan.Zero;
                            }
                            // Otherwise
                            else
                            {
                                // Initialize the audio
                                output.Init(audioFile);
                            }

                            // If the user wants the sound
                            if (config.Sound)
                            {
                                // Play it
                                output.Play();
                            }
                            // Otherwise
                            else
                            {
                                // Mark an update as required and wait for the next tick
                                updateRequired = true;
                            }

                            // Finally, search the dumpster
                            SearchDumpster();
                        }
                    }
                }
            }

            // If there was no dumpster found and the user was notified
            if (!found && notified)
            {
                // A notification is required in the next tick
                notified = false;
            }
        }

        private void OnPlaybackStopped(object sender, StoppedEventArgs e)
        {
            // Make the tick update the playback
            updateRequired = true;
        }

        #endregion

        #region Functions

        private void SearchDumpster()
        {
            // Get a random item from the enum at the top
            Items Item = (Items)generator.Next(0, Enum.GetValues(typeof(Items)).Length);
            // Get the money that we should add
            int Money = Item == Items.Money ? generator.Next(config.MoneyMinimum, config.MoneyMaximum + 1) : 0;

            // See what the user got
            switch (Item)
            {
                case Items.HotDog:
                case Items.Hamburger:
                    Game.Player.Character.Health = Game.Player.Character.MaxHealth;
                    break;
                case Items.Money:
                    Game.Player.Money += Money;
                    break;
                case Items.Pistol:
                    Weapon(WeaponHash.Pistol);
                    break;
                case Items.MicroSMG:
                    Weapon(WeaponHash.MicroSMG);
                    break;
                case Items.AssaultRifle:
                    Weapon(WeaponHash.AssaultRifle);
                    break;
                case Items.Shotgun:
                    Weapon(WeaponHash.PumpShotgun);
                    break;
                case Items.SawnOffShotgun:
                    Weapon(WeaponHash.SawnOffShotgun);
                    break;
                case Items.Grenades:
                    Weapon(WeaponHash.Grenade);
                    break;
                case Items.BZGas:
                    Weapon(WeaponHash.BZGas);
                    break;
                case Items.TearGas:
                    Weapon(WeaponHash.SmokeGrenade);
                    break;
            }

            // If the player picked up money
            if (Item == Items.Money)
            {
                // Format the item
                //Notification.Show(string.Format(Resources.ResourceManager.GetString($"Found{Item}"), Money));
            }
            // Otherwise
            else
            {
                // Notify the player with the string as-is
                //Notification.Show(Resources.ResourceManager.GetString($"Found{Item}"));
            }
        }

        private static void Weapon(WeaponHash Weapon)
        {
            // If the player does not have the weapon, give one to him
            if (!Game.Player.Character.Weapons.HasWeapon(Weapon))
            {
                Game.Player.Character.Weapons.Give(Weapon, 0, true, true);
            }

            // Then, select the weapon and give 2 magazines
            Game.Player.Character.Weapons.Select(Weapon);
            Game.Player.Character.Weapons.Current.Ammo += (Game.Player.Character.Weapons.Current.MaxAmmoInClip * 2);
        }

        #endregion
    }
}
