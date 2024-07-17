using GTA;
using GTA.Math;
using GTA.UI;
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
    private static readonly Dictionary<Prop, List<Item>> nextItem = [];

    private static Configuration config = Configuration.Load();

    private readonly List<Prop> nearbyDumpsters = [];
    private readonly Dictionary<Prop, int> dumpsterTimeout = [];

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
    }

    #endregion

    #region Tools

    private void SearchDumpster(Prop prop)
    {
        int chance = generator.Next(100);

        if (nextItem.TryGetValue(prop, out List<Item> items) && items.Count > 0)
        {
            Item item = items[generator.Next(items.Count)];
            Companion.Inventories.Current.Add(item);
            items.Remove(item);
            Notification.Show($"You found ~g~{item.Name}~s~!");
        }
        // 0 to 45 - Item
        else if (chance <= 45f)
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
        else if (chance > 45 && chance <= 90 && config.Weapons.Count >= 1)
        {
            WeaponHash hash = config.Weapons[generator.Next(config.Weapons.Count)];
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

        foreach (Prop prop in nearbyDumpsters)
        {
            if (dumpsterTimeout.ContainsKey(prop))
            {
                continue;
            }

            if (config.Blips && (prop.AttachedBlip == null || !prop.AttachedBlip.Exists()))
            {
                Blip current = prop.AddBlip();
                current.Name = "Dumpster";
                Function.Call(Hash.SET_BLIP_COLOUR, current.Handle, (config.Color.R << 24) + (config.Color.G << 16) + (config.Color.B << 8) + config.Color.A);
            }

            Vector3 front = prop.GetOffsetPosition(new Vector3(0, -1f, 0));

            if (config.Markers && World.GetDistance(Game.Player.Character.Position, prop.Position) <= config.MarkerDistance)
            {
                World.DrawMarker(MarkerType.VerticalCylinder, front, Vector3.Zero, Vector3.Zero, new Vector3(0.7f, 0.7f, 0.7f), config.Color);
            }

            if (Game.Player.Character.CurrentVehicle == null)
            {
                if (World.GetDistance(Game.Player.Character.Position, front) <= config.LootDistance)
                {
                    if (!notified)
                    {
                        Screen.ShowHelpText("Press ~INPUT_CONTEXT~ to loot the dumpster.");
                        notified = true;
                    }

                    found = true;

                    if (Game.IsControlJustPressed(Control.Context))
                    {
                        if (config.Fade >= 1)
                        {
                            Screen.FadeOut(config.Fade);
                            Game.Player.Character.IsPositionFrozen = true;

                            Wait(1000);
                        }

                        SearchDumpster(prop);

                        if (config.Fade >= 1)
                        {
                            Screen.FadeIn(config.Fade);
                            Game.Player.Character.IsPositionFrozen = false;
                        }

                        if (config.LootTimer > 0)
                        {
                            dumpsterTimeout[prop] = Game.GameTime + (config.LootTimer * 60 * 1000);
                            prop.IsPersistent = true;
                        }
                    }
                }
            }
        }

        foreach (Prop prop in dumpsterTimeout.Keys.ToList())
        {
            if (!prop.Exists() || dumpsterTimeout[prop] <= Game.GameTime)
            {
                dumpsterTimeout.Remove(prop);
                prop.IsPersistent = false;
            }
        }

        if (!found && notified)
        {
            Function.Call(Hash.CLEAR_ALL_HELP_MESSAGES);
            Function.Call(Hash.CLEAR_HELP, true);
            notified = false;
        }
    }

    #endregion
    
    #region Functions

    /// <summary>
    /// Adds a specific set of items for a dumpster.
    /// </summary>
    /// <param name="prop">The Dumpster prop to use.</param>
    /// <param name="items">The items to add.</param>
    public static void AddItemsForDumpster(Prop prop, params Item[] items)
    {
        if (nextItem.TryGetValue(prop, out List<Item> list))
        {
            list.AddRange(items);
        }
        else
        {
            nextItem.Add(prop, [..items]);
        }
    }
    
    #endregion
}
