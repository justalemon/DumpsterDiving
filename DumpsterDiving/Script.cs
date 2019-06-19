using Citron;
using DumpsterDiving.Properties;
using GTA;
using GTA.Math;
using GTA.Native;
using NAudio.Wave;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

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

    public class DumpsterDiving : Script
    {
        /// <summary>
        /// The audio output device.
        /// </summary>
        private WaveOutEvent Output = new WaveOutEvent();
        /// <summary>
        /// The audio file that we are going to hear.
        /// </summary>
        private AudioFileReader AudioFile = new AudioFileReader("scripts\\DumpsterDiving\\Search.mp3");
        /// <summary>
        /// Our random number generator.
        /// </summary>
        private Random Generator = new Random();
        /// <summary>
        /// If the game information should be updated after the playback.
        /// </summary>
        private bool UpdateRequired = false;
        /// <summary>
        /// A list that contains models of dumpsters.
        /// </summary>
        private readonly List<Model> Models = new List<Model>
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
        private Configuration Config = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText("scripts\\DumpsterDiving.json"));
        /// <summary>
        /// The dumpsters that exist arround the map.
        /// </summary>
        private List<Prop> Dumpsters = new List<Prop>();
        /// <summary>
        /// Next game time that we should update the lists of peds.
        /// </summary>
        private int NextFetch = 0;
        /// <summary>
        /// If the player has been notified about how to do the dumpster diving.
        /// </summary>
        private bool Notified = false;
        /// <summary>
        /// If a dumpster has been found.
        /// </summary>
        private bool Found = false;

        public DumpsterDiving()
        {
            // Add our events
            Tick += OnTick;
            Output.PlaybackStopped += OnPlaybackStopped;
        }

        private void OnTick(object sender, EventArgs e)
        {
            // Set found to false
            Found = false;

            // If the current time is higher or equal than the next fetch time
            if (Game.GameTime >= NextFetch)
            {
                // Reset the list of dumpsters
                Dumpsters = new List<Prop>();
                // Iterate over the dumpster models
                foreach (Model DumpsterModel in Models)
                {
                    // Fill the list with all of those props
                    Dumpsters.AddRange(World.GetAllProps(DumpsterModel));
                }
                // Finally, set the next fetch time to one second in the future
                NextFetch = Game.GameTime + 1000;
            }

            // If we need to update the playback
            if (UpdateRequired)
            {
                // Disable the update
                UpdateRequired = false;
                // Fade in
                Game.FadeScreenIn(Config.Fade);
                // And unfreeze the player
                Game.Player.Character.FreezePosition = false;
            }

            // Iterate over the stored dumpsters
            foreach (Prop DumpsterProp in Dumpsters)
            {
                // If the user wants blips and the dumpster doesn't have one
                if (Config.Blips && !DumpsterProp.CurrentBlip.Exists())
                {
                    // Create the blip
                    Blip Current = DumpsterProp.AddBlip();
                    // And set the properties of it
                    Current.Name = "Dumpster";
                    Current.Color = Config.BlipColor;
                }

                // Get the position of the front
                Vector3 Front = DumpsterProp.GetOffsetInWorldCoords(new Vector3(0, -1f, 0));

                // If the distance is lower or equal to 25 units
                if (World.GetDistance(Game.Player.Character.Position, DumpsterProp.Position) <= Config.MarkerDistance)
                {
                    // Draw a marker that will trigger the dumpster diving
                    World.DrawMarker(MarkerType.VerticalCylinder, Front, Vector3.Zero, Vector3.Zero, new Vector3(0.7f, 0.7f, 0.7f), Config.MarkerColor);
                }

                // If the player is on foot
                if (Game.Player.Character.CurrentVehicle == null)
                {
                    // If the distance between the front and the player is lower or equal to 1.5
                    if (World.GetDistance(Game.Player.Character.Position, Front) <= Config.LootDistance)
                    {
                        // If the user has not been notified
                        if (!Notified)
                        {
                            // Show the user
                            Screen.ShowHelp("Press ~INPUT_PICKUP~ to loot the dumpster.", 2000);
                            // And set the flag to true
                            Notified = true;
                        }

                        // Set the found variable to true
                        Found = true;

                        // If the player pressed the interact button
                        // DEV NOTE: Use GTA.Control.Whistle if Talk doesn't work
                        if (Game.IsControlJustPressed(0, Control.Talk))
                        {
                            // Fade the screen out and freeze the player
                            Game.FadeScreenOut(Config.Fade);
                            Game.Player.Character.FreezePosition = true;
                            // Wait for a seccond
                            Wait(1000);
                            // If the current time of the audio is the same as the total time
                            if (AudioFile.CurrentTime == AudioFile.TotalTime)
                            {
                                // Stop the playback and reset the playtime
                                Output.Stop();
                                AudioFile.CurrentTime = TimeSpan.Zero;
                            }
                            // Otherwise
                            else
                            {
                                // Initialize the audio
                                Output.Init(AudioFile);
                            }

                            // If the user wants the sound
                            if (Config.Sound)
                            {
                                // Play it
                                Output.Play();
                            }
                            // Otherwise
                            else
                            {
                                // Mark an update as required and wait for the next tick
                                UpdateRequired = true;
                            }

                            // Finally, search the dumpster
                            SearchDumpster();
                        }
                    }
                }
            }

            // If there was no dumpster found and the user was notified
            if (!Found && Notified)
            {
                // A notification is required in the next tick
                Notified = false;
            }
        }

        private void OnPlaybackStopped(object sender, StoppedEventArgs e)
        {
            // Make the tick update the playback
            UpdateRequired = true;
        }

        private void SearchDumpster()
        {
            // Get a random item from the enum at the top
            Items Item = (Items)Generator.Next(0, Enum.GetValues(typeof(Items)).Length);
            // Get the money that we should add
            int Money = Item == Items.Money ? Generator.Next(Config.MoneyMinimum, Config.MoneyMaximum + 1) : 0;

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
                UI.Notify(string.Format(Resources.ResourceManager.GetString($"Found{Item}"), Money));
            }
            // Otherwise
            else
            {
                // Notify the player with the string as-is
                UI.Notify(Resources.ResourceManager.GetString($"Found{Item}"));
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
    }
}
