using System;
using Gadgeteer.Modules.Seeed;

namespace GadgeteerApp1
{
    public interface IGpsSensor
    {
        DateTime GetMeLastTime();

        // public delegate void PositionReceivedHandler(Gadgeteer.Modules.Seeed.GPS sender, Gadgeteer.Modules.Seeed.GPS.Position position);
        event GPS.PositionReceivedHandler OnGpsSyncReceived;

        bool Enable { get; set; }        
    }
}

