using GTA;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

public class DumpsterDiving : Script
{
    /// <summary>
    /// A list that contains models of dumpsters.
    /// </summary>
    public static List<Model> Dumpsters = new List<Model>
    {
        new Model("prop_dumpster_02b"),
        new Model("prop_dumpster_04a"),
        new Model("prop_dumpster_01a")
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
        UI.Notify(@"Dumpster Diving V has been loaded successfully!

Version: Alpha 1.1
Authors: Derpy-Canadian, Lemon
Build Type: Debug|Any CPU");
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
                    // If the dumpster is far and has a blip attached, remove it
                    if (Distance > 25f && CurrentProp.CurrentBlip.Exists())
                    {
                        UI.Notify("Deleting Prop" + CurrentProp.GetHashCode().ToString());
                        CurrentProp.CurrentBlip.Remove();
                    }
                }
            }
        }
    }

    private void OnKeyDown(object Sender, KeyEventArgs Args)
    {
        // In the case of pressing Page Down
        if (Args.KeyCode == Keys.PageDown)
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
