﻿using GTA;
using GTA.Math;
using GTA.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

public class DumpsterDiving : Script
{
    /// <summary>
    /// If the player has a dumpster that can be used.
    /// This changes every tick.
    /// </summary>
    private bool CanSearch = false;
    /// <summary>
    /// The items that the player can get in the dumpsters.
    /// </summary>
    private enum Items {
        Hotdog = 0,
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
        AR = 11,
        Shotgun = 12,
        SawnOffShotgun = 13,
        Grenades = 14,
        BZ = 15
    }
    /// <summary>
    /// A list that contains models of dumpsters.
    /// </summary>
    public static List<Model> Dumpsters = new List<Model>
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
    public static ScriptSettings ScriptConfig = ScriptSettings.Load("scripts\\DumpsterDiving.ini");
    /// <summary>
    /// Proximity between the player and the dumpster to show a Blip.
    /// </summary>
    public static float Proximity = ScriptConfig.GetValue("CWDD", "Proximity", 25f);

    public DumpsterDiving()
    {
        // Add our events
        Tick += OnTick;
        KeyDown += OnKeyDown;

        // Just an example message
        // TODO: Print the version and type of build
        UI.Notify("DumpsterDiving has been loaded!");
    }

    private void OnTick(object Sender, EventArgs Args)
    {
        // By default, the user can't search the dumpster if is not near it
        CanSearch = false;

        // Iterate over our Dumpster models
        foreach (Model PropModel in Dumpsters)
        {
            // Iterate over the props for the model
            foreach (Prop CurrentProp in World.GetAllProps(PropModel))
            {
                // Get the distance in units between the player and the Dumpster
                float Distance = World.GetDistance(Game.Player.Character.Position, CurrentProp.Position);
                // If the player is near and the dumpster is visible, enable the dumpster diving minigame on it
                if (CurrentProp.IsVisible && Distance <= Proximity)
                {
                    // Add a blip if the user wants to
                    if (!CurrentProp.CurrentBlip.Exists() && ScriptConfig.GetValue("CWDD", "Blips", false))
                    {
                        Blip PropBlip = CurrentProp.AddBlip();
                        PropBlip.Name = "Dumpster";
                        PropBlip.Sprite = BlipSprite.Devin;
                        PropBlip.Color = BlipColor.Yellow;
                    }

                    // Draw a marker that shows the dumpster position
                    Vector3 TopMarkerPos = new Vector3(CurrentProp.Position.X, CurrentProp.Position.Y, CurrentProp.Position.Z + 2);
                    World.DrawMarker(MarkerType.UpsideDownCone, TopMarkerPos, Vector3.Zero, Vector3.Zero, new Vector3(0.5f, 0.5f, 0.5f), Color.YellowGreen);
                    // Draw a marker that will trigger the dumpster diving
                    Vector3 Front = CurrentProp.GetOffsetInWorldCoords(new Vector3(0, -1f, 0));

                    // If the player is near the dumpster, allow it to search
                    if (Game.Player.Character.Position.DistanceTo(Front) <= 1.5)
                    {
                        UI.ShowSubtitle("Press " + ScriptConfig.GetValue("CWDD", "KeyInteract", Keys.None).ToString() + " to search dumpster", 1);
                        CanSearch = true;
                    }
                }
                // If the dumpster is far and has a blip attached, remove it
                if (Distance > 25f && CurrentProp.CurrentBlip.Exists())
                {
                    if (ScriptConfig.GetValue("CWDD", "Debug", false))
                    {
                        UI.Notify("Deleting blip: " + CurrentProp.CurrentBlip.GetHashCode().ToString());
                    }
                    CurrentProp.CurrentBlip.Remove();
                }
            }
        }
    }

    private void OnKeyDown(object Sender, KeyEventArgs Args)
    {
        // If the player preses E and is a dumpster available, "loot it"
        if (Args.KeyCode == ScriptConfig.GetValue("CWDD", "KeyInteract", Keys.None) && CanSearch)
        {
            Game.FadeScreenOut(1000);
            Wait(1000);
            SearchDumpster();
            Wait(1000);
            Game.FadeScreenIn(1000);
        }
        // In the case of pressing Page Down
        if (Args.KeyCode == ScriptConfig.GetValue("CWDD", "KeyBlipRemoval", Keys.None))
        {
            // Iterate over the map blips
            foreach (Blip CurrentBlip in World.GetActiveBlips())
            {
                // If is a D blip
                if (CurrentBlip.Sprite == BlipSprite.Devin)
                {
                    // Remove it
                    CurrentBlip.Remove();
                }
            }
        }
    }

    private void SearchDumpster()
    {
        // Temporary variables to know if the player has found a weapon
        WeaponHash Weapon = WeaponHash.Pistol;
        bool WeaponFound = false;

        // Get a random item from the enum at the top
        Random Generator = new Random();
        int Number = Generator.Next(0, Enum.GetValues(typeof(Items)).Length);
        Items Item = (Items)Number;

        // Hotdog's and Hamburgers restore the entire health bar
        if (Item == Items.Hamburger)
        {
            int MaxHealth = Function.Call<int>(Hash.GET_ENTITY_MAX_HEALTH, Game.Player.Character);
            Function.Call(Hash.SET_ENTITY_HEALTH, Game.Player.Character, MaxHealth);
            UI.Notify("You found a Hamburger!");
        }
        else if (Item == Items.Hotdog)
        {
            int MaxHealth = Function.Call<int>(Hash.GET_ENTITY_MAX_HEALTH, Game.Player.Character);
            Function.Call(Hash.SET_ENTITY_HEALTH, Game.Player.Character, MaxHealth);
            UI.Notify("You found a Hotdog!");
        }
        // Money gives a random ammount between 10 and 100
        else if (Item == Items.Money)
        {
            int amountofmoney = Generator.Next(10, 100);
            Game.Player.Money += amountofmoney;
            UI.Notify("You found $" + amountofmoney + "!");
        }
        // Now, the whole list of weapons
        else if (Item == Items.Pistol)
        {
            Weapon = WeaponHash.Pistol;
            WeaponFound = true;
            UI.Notify("You found a Pistol!");
        }
        else if (Item == Items.MicroSMG)
        {
            Weapon = WeaponHash.MicroSMG;
            WeaponFound = true;
            UI.Notify("You found a Micro SMG!");
        }
        else if (Item == Items.AR)
        {
            Weapon = WeaponHash.AssaultRifle;
            WeaponFound = true;
            UI.Notify("You found an Assault Rifle!");
        }
        else if (Item == Items.Shotgun)
        {
            Weapon = WeaponHash.PumpShotgun;
            WeaponFound = true;
            UI.Notify("You found a Pump Shotgun!");
        }
        else if (Item == Items.SawnOffShotgun)
        {
            Weapon = WeaponHash.SawnOffShotgun;
            WeaponFound = true;
            UI.Notify("You found a Sawn Off Shotgun!");
        }
        else if (Item == Items.Grenades)
        {
            Weapon = WeaponHash.Grenade;
            WeaponFound = true;
            UI.Notify("You found a Grenade!");
        }
        else if (Item == Items.BZ)
        {
            Weapon = WeaponHash.BZGas;
            WeaponFound = true;
            UI.Notify("You found BZ Gas!");
        }
        else if (Item == Items.Condom)
        {
            UI.Notify("You found a Condom.");
        }
        else if (Item == Items.Boot)
        {
            UI.Notify("You found a Boot.");
        }
        else if (Item == Items.Fish)
        {
            UI.Notify("You found a fish carcass.");
        }
        else if (Item == Items.Dildo)
        {
            UI.Notify("You found a Dildo.");
        }
        else if (Item == Items.MoldyHamburger)
        {
            UI.Notify("You found a Moldy Hamburger.");
        }
        else if (Item == Items.MoldyHotDog)
        {
            UI.Notify("You found a Moldy Hotdog.");
        }

        // Give a weapon or ammo
        if (WeaponFound)
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
