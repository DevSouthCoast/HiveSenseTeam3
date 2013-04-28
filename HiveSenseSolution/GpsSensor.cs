using System;
using Gadgeteer.Modules.Seeed;

namespace GadgeteerApp1
{
    public class GpsSensor : IGpsSensor
    {
        private readonly GPS _gps;

        public GpsSensor(GPS gps)
        {
            _gps = gps;
        }

        public DateTime GetMeLastTime()
        {
            return _gps.LastPosition.FixTimeUtc;
        }

        public event GPS.PositionReceivedHandler OnGpsSyncReceived;

        public bool Enable
        {
            get { return _gps.Enabled; }
            set { _gps.Enabled = value; }
        }
    }
}
