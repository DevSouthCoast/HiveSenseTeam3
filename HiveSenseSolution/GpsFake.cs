using System;
using Gadgeteer.Modules.Seeed;
using Microsoft.SPOT; 

namespace GadgeteerApp1
{
    public class GpsFake : IGpsSensor
    {
        

        public DateTime GetMeLastTime()
        {
            return new DateTime(2010, 2, 2); 
        }

        public event GPS.PositionReceivedHandler  OnGpsSyncReceived;

        public bool Enable
        {
            get
            {
                return false;
            }
            set
            {
                if (value == true)
                {
                    OnGpsSyncReceived.Invoke(null, null);
                }
            }
        }
    }
}
