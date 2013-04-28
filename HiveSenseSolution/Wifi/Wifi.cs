using System;
using Microsoft.SPOT;

namespace GadgeteerApp1.Wifi
{
    class Wifi
    {

        public Wifi()
        {
                
        }

        public delegate void WifiConnectedEventHandler(object sender, EventArgs e);

        public event EventHandler RaiseWifiConnected;


        protected virtual void OnRaiseWifiConnected(EventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of 
            // a race condition if the last subscriber unsubscribes 
            // immediately after the null check and before the event is raised.
            EventHandler handler = RaiseWifiConnected;

            // Event will be null if there are no subscribers 
            if (handler != null)
            {
                // Use the () operator to raise the event.
                handler(this, e);
            }
        }

        public void InitialiseWifi()
        {
            RaiseWifiConnected(this, new EventArgs());
        }
    }
}
