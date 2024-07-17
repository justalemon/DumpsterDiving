using GTA;
using GTA.Math;
using GTA.UI;
using NAudio.Wave;
using PlayerCompanion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using GTA.Native;

namespace DumpsterDiving;

/// <summary>
/// Script that allows you to perform dumpster diving
/// </summary>
public class DumpsterDiving : Script
{
    #region Fields

    private static readonly string location = new Uri(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase)).LocalPath;
    private static readonly Random generator = new Random();

    private static Configuration config = Configuration.Load();

    private readonly WaveOutEvent output = new WaveOutEvent();
    private readonly AudioFileReader audioFile = new AudioFileReader(Path.Combine(location, "DumpsterDiving", "Search.mp3"));
    private readonly List<WeaponHash> hashes = ((WeaponHash[])Enum.GetValues(typeof(WeaponHash))).ToList();

    private bool updateRequired = false;
    private List<Prop> nearbyDumpsters = [];
    private int nextFetch = 0;
    private bool notified = false;
    private bool found = false;

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new Dumpster Diving script.
    /// </summary>
    public DumpsterDiving()
    {
        Screen.FadeIn(0);

        Tick += OnTick;
        output.PlaybackStopped += OnPlaybackStopped;
    }

    #endregion

    #region Tools

    private void SearchDumpster()
    {
        int chance = generator.Next(100);

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

    #region Event Functions

    private void OnTick(object sender, EventArgs e)
    {
        found = false;

        if (Game.GameTime >= nextFetch)
        {
            nearbyDumpsters.Clear();

            foreach (Model model in config.Models)
            {
                nearbyDumpsters.AddRange(World.GetAllProps(model));
            }

            nextFetch = Game.GameTime + 1000;
        }

        if (updateRequired)
        {
            updateRequired = false;
            Screen.FadeIn(config.Fade);
            Game.Player.Character.IsPositionFrozen = false;
        }

        foreach (Prop prop in nearbyDumpsters)
        {
            if (config.Blips && (prop.AttachedBlip == null || !prop.AttachedBlip.Exists()))
            {
                Blip current = prop.AddBlip();
                current.Name = "Dumpster";
                current.Color = config.BlipColor;
            }

            Vector3 front = prop.GetOffsetPosition(new Vector3(0, -1f, 0));

            if (World.GetDistance(Game.Player.Character.Position, prop.Position) <= config.MarkerDistance)
            {
                World.DrawMarker(MarkerType.VerticalCylinder, front, Vector3.Zero, Vector3.Zero, new Vector3(0.7f, 0.7f, 0.7f), config.MarkerColor);
            }

            if (Game.Player.Character.CurrentVehicle == null)
            {
                if (World.GetDistance(Game.Player.Character.Position, front) <= config.LootDistance)
                {
                    if (!notified)
                    {
                        Screen.ShowHelpText("Press ~INPUT_PICKUP~ to loot the dumpster.");
                        notified = true;
                    }

                    found = true;

                    // DEV NOTE: Use GTA.Control.Whistle if Talk doesn't work
                    if (Game.IsControlJustPressed(Control.Talk))
                    {
                        Screen.FadeOut(config.Fade);
                        Game.Player.Character.IsPositionFrozen = true;

                        Wait(1000);

                        if (audioFile.CurrentTime == audioFile.TotalTime)
                        {
                            output.Stop();
                            audioFile.CurrentTime = TimeSpan.Zero;
                        }
                        else
                        {
                            output.Init(audioFile);
                        }

                        if (config.Sound)
                        {
                            output.Play();
                        }
                        else
                        {
                            updateRequired = true;
                        }

                        SearchDumpster();
                    }
                }
            }
        }

        if (!found && notified)
        {
            Function.Call(Hash.CLEAR_ALL_HELP_MESSAGES);
            Function.Call(Hash.CLEAR_HELP, true);
            notified = false;
        }
    }
    private void OnPlaybackStopped(object sender, StoppedEventArgs e)
    {
        updateRequired = true;
    }

    #endregion
}
