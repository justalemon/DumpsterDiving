using GTA;
using System;
using System.Collections.Generic;

public class DumpsterDiving : Script
{
    /// <summary>
    /// A list that contains models of dumpsters.
    /// </summary>
    public static List<Model> Dumpsters = new List<Model>
    {
        new Model("prop_dumpster_02b"),
        new Model("prop_dumpster_04a")
    };
    /// <summary>
    /// A list with the map blips that we have created.
    /// </summary>
    public static List<Blip> Blips = new List<Blip>();

    public DumpsterDiving()
    {
        // Add our events
        Tick += OnTick;

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
                // Check that the Prop is visible and is near 15 units to the player
                if (CurrentProp.IsVisible && World.GetDistance(Game.Player.Character.Position, CurrentProp.Position) <= 15f)
                {
                    // TODO: Do something
                }
            }
        }
    }
}
