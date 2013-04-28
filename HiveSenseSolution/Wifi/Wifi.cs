using System;
using GHI.Premium.Net;
using Gadgeteer.Modules.GHIElectronics;
using Microsoft.SPOT;

namespace GadgeteerApp1.Wifi
{
    class Wifi
    {
        private readonly WiFi_RS21 wifi_RS21;
        private readonly string _ssid;
        private readonly string _password;

        public Wifi(WiFi_RS21 wifiRs21, String SSID, String password)
        {
            wifi_RS21 = wifiRs21;
            _ssid = SSID;
            _password = password;
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

            if (!wifi_RS21.Interface.IsOpen)
                wifi_RS21.Interface.Open();

            NetworkInterfaceExtension.AssignNetworkingStackTo(wifi_RS21.Interface);

            if (!wifi_RS21.Interface.NetworkInterface.IsDhcpEnabled)
                wifi_RS21.Interface.NetworkInterface.EnableDhcp();



            wifi_RS21.Interface.WirelessConnectivityChanged += new WiFiRS9110.WirelessConnectivityChangedEventHandler(Interface_WirelessConnectivityChanged);
            wifi_RS21.Interface.NetworkAddressChanged += new NetworkInterfaceExtension.NetworkAddressChangedEventHandler(Interface_NetworkAddressChanged);


            //Here you have to write the name (SSID) of your local WLAN AP
            WiFiNetworkInfo[] ScanResp = wifi_RS21.Interface.Scan(this._ssid);

            foreach (WiFiNetworkInfo result in ScanResp)
            {
                Debug.Print("****" + result.SSID + "****");
                Debug.Print("ChannelNumber = " + result.ChannelNumber);
                Debug.Print("networkType = " + result.networkType);
                //Debug.Print("PhysicalAddress = " + result.GetMACAddress(result.PhysicalAddress));
                Debug.Print("RSSI = " + result.RSSI);
                Debug.Print("SecMode = " + result.SecMode);
            }

            if (ScanResp != null && ScanResp.Length > 0)
            {
                try
                {
                    var info = ScanResp[0];
                    // Include the password of your WLAN
                    wifi_RS21.Interface.Join(ScanResp[0], this._password);
                }
                catch (Exception ex)
                {
                    return;
                }
            }
        }

        void Interface_WirelessConnectivityChanged(object sender, WiFiRS9110.WirelessConnectivityEventArgs e)
        {
            Debug.Print("WirelessConnectivityChanged event!");
            Debug.Print("Connected: " + e.IsConnected);
            Debug.Print("IP Address : " + wifi_RS21.Interface.NetworkInterface.IPAddress);
            wifi_RS21.Interface.NetworkInterface.RenewDhcpLease();
        }

        void Interface_NetworkAddressChanged(object sender, EventArgs e)
        {
            try
            {
                Debug.Print("NetworkAddressChanged event!");
                Debug.Print("IP Address : " + wifi_RS21.Interface.NetworkInterface.IPAddress);
                Debug.Print("Default Getway: " + wifi_RS21.Interface.NetworkInterface.GatewayAddress);

                for (int j = 0; j < wifi_RS21.Interface.NetworkInterface.DnsAddresses.Length; j++)
                {
                    Debug.Print("DNS Server: " + wifi_RS21.Interface.NetworkInterface.DnsAddresses[j]);
                }
            }
            catch (Exception ex)
            {
                return;
            }
            RaiseWifiConnected(this, new EventArgs());
        }
    }
}
