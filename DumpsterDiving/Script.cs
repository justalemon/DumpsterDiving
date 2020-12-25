using GTA;
using GTA.Math;
using GTA.UI;
using NAudio.Wave;
using Newtonsoft.Json;
using PlayerCompanion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DumpsterDiving
{
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
        /// The weapons to give randomly.
        /// </summary>
        private readonly List<WeaponHash> hashes = ((WeaponHash[])Enum.GetValues(typeof(WeaponHash))).ToList();
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
            // Clear a fade out just in case
            Screen.FadeIn(0);

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
                if (config.Blips && (DumpsterProp.AttachedBlip == null || !DumpsterProp.AttachedBlip.Exists()))
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
            // Get a number from 0 to 100 to calculate the channce
            int chance = generator.Next(100);

            // See what the user got and do what is necesary
            // 0 to 45 - Item
            if (chance <= 45f)
            {
                Item item = Companion.Inventories.GetRandomItem();
                if (item == null)
                {
                    Notification.Show($"~r~Error~s~: Unable to give a random item to the user!");
                    return;
                }
                Companion.Inventories.Current.Add(item);
                Notification.Show($"You found ~g~{item.Name}~s~!");
            }
            // 45 to 90 - Weapon
            else if (chance > 45 && chance <= 90)
            {
                WeaponHash hash = hashes[generator.Next(hashes.Count)];
                if (!Game.Player.Character.Weapons.HasWeapon(hash))
                {
                    Game.Player.Character.Weapons.Give(hash, 0, true, true);
                }
                Game.Player.Character.Weapons.Select(hash);
                Game.Player.Character.Weapons.Current.Ammo += Game.Player.Character.Weapons.Current.MaxAmmoInClip * 2;
                Notification.Show($"You found ~g~{Game.Player.Character.Weapons.Current.LocalizedName}~s~!");
            }
            // 90 to 100 - Money
            else if (chance > 90)
            {
                int money = generator.Next(config.MoneyMinimum, config.MoneyMaximum + 1);
                Companion.Wallet.Money += money;
                Notification.Show($"You found ~g~${money}~s~!");
            }
        }

        #endregion
    }
}
