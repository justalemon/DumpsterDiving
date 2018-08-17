using GTA;
using GTA.Math;
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
        Random random = new Random();
        int randomNumber = random.Next(1, 15);
        Items Item = (Items)randomNumber;
        UI.Notify("You found a " + Item.ToString() + "!");
    }
}
