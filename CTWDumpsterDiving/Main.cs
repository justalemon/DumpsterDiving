using GTA;
using System;

public class DumpsterDiving : Script
{

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

    }
}
