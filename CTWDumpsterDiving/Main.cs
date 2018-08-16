using GTA;
using GTA.Math;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

public class DumpsterDiving : Script
{
    /// <summary>
    /// A list that contains models of dumpsters.
    /// </summary>
    public static List<Model> Dumpsters = new List<Model>
    {
        new Model("prop_dumpster_01a"),
        new Model("prop_dumpster_02a"),
        new Model("prop_dumpster_02b"),
        new Model("prop_dumpster_04a"),
        new Model("prop_dumpster_4b")
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

                    Vector3 Position = new Vector3(CurrentProp.Position.X, CurrentProp.Position.Y, CurrentProp.Position.Z + 2);
                    World.DrawMarker(MarkerType.UpsideDownCone, Position, Vector3.Zero, Vector3.Zero, new Vector3(0.5f, 0.5f, 0.5f), Color.YellowGreen);
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
}
