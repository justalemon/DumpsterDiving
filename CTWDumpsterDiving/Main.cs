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
        // If the user wants blips, work on it
        if (ScriptConfig.GetValue("CWDD", "Blips", false))
        {
            // Iterate over our Dumpster models
            foreach (Model PropModel in Dumpsters)
            {
                // Iterate over the props for the model
                foreach (Prop CurrentProp in World.GetAllProps(PropModel))
                {
                    // Get the distance in units between the player and the Dumpster
                    float Distance = World.GetDistance(Game.Player.Character.Position, CurrentProp.Position);
                    // Check that the Prop is visible, is near 15 units to the player and it does not have a blip attached
                    if (CurrentProp.IsVisible && Distance <= 25f && !CurrentProp.CurrentBlip.Exists())
                    {
                        Blip PropBlip = CurrentProp.AddBlip();
                        PropBlip.Name = "Dumpster";
                        PropBlip.Sprite = BlipSprite.Devin;
                        PropBlip.Color = BlipColor.Green;
                    }
                    if (CurrentProp.IsVisible && Distance <= 25f)
                    {
                        float X = CurrentProp.Position.X;
                        float Y = CurrentProp.Position.Y;
                        float Z = CurrentProp.Position.Z + 2;
                        World.DrawMarker(MarkerType.UpsideDownCone, new Vector3(X, Y, Z), Vector3.Zero, Vector3.Zero, new Vector3(0.5f, 0.5f, 0.5f), Color.Red);
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
