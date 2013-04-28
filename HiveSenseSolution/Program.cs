using System;
using GadgeteerApp1.Data;
using Microsoft.SPOT;
using GT = Gadgeteer;
using Gadgeteer.Modules.GHIElectronics;
using Gadgeteer.Modules.Seeed;
using GadgeteerApp1.Cellular;

namespace GadgeteerApp1
{
    public partial class Program
    {
        GT.Timer _timer = new GT.Timer(1000);
        int _numberTicks = 0;

        bool _pauseTemprature = false;
        int _pauseTempratureCount = 0;

        private IGpsSensor _gpsSensor;

        CellularObject _celluarObject;

        private TempAndHumidityAccess _tempAndHumidityAccess;

        // This method is run when the mainboard is powered up or reset.   
        void ProgramStarted()
        {
            Debug.Print("Program Started");

            // Change the instance required here when debugging.
            _gpsSensor = new GpsFake();
            //_gpsSensor = new GpsSensor(gps);

            _tempAndHumidityAccess = new TempAndHumidityAccess(sdCard, new DateTime(2013, 1, 1));

            button.ButtonPressed += Button_Method_Click;
            
            // Initially, wait till we have recieved a lock on the the GPS position before logging
            _gpsSensor.OnGpsSyncReceived += new GPS.PositionReceivedHandler(GPS_PositionReceived);
            _gpsSensor.Enable = true;

            //gps.PositionReceived += new GPS.PositionReceivedHandler(GPS_PositionReceived);
            //gps.Enabled = true;

            char_Display.Clear();
            char_Display.PrintString("GPS Syncing...");
           // BeginMonitoring();
        }

        private void GPS_PositionReceived(GPS sender, GPS.Position position)
        {
            // Unregister the position received event as we've now got a lock on
            _gpsSensor.OnGpsSyncReceived -= new GPS.PositionReceivedHandler(GPS_PositionReceived);
            //gps.PositionReceived -= new GPS.PositionReceivedHandler(GPS_PositionReceived);

            BeginMonitoring();
        }

        public void BeginMonitoring()
        {
            _timer.Tick += new GT.Timer.TickEventHandler(Timer_Tick);
            _timer.Start();

            temperatureHumidity.MeasurementComplete+=new TemperatureHumidity.MeasurementCompleteEventHandler(temperatureHumidity_MeasurementComplete);
            temperatureHumidity.StartContinuousMeasurements();
            InitialiseAccelerormeter();


            var wifi = new Wifi.Wifi(wifi_RS21, "", "");
            wifi.RaiseWifiConnected += new EventHandler(onWifiConnected);

        }

        void onWifiConnected(object sender, EventArgs e)
        {
            DisplayMessage("Connected to Wifi", String.Empty);

        }

        public void InitialiseAccelerormeter()
        {
            accelerometer.EnableThresholdDetection(3, false, true, false, false, false, true);
            accelerometer.ThresholdExceeded += new Accelerometer.ThresholdExceededEventHandler(accelerometer_ThresholdExceeded);
        }

        void accelerometer_ThresholdExceeded(Accelerometer sender)
        {
            DisplayMessage("Help! Fallen!", string.Empty);
            //Panic("Help! Fallen");

            _pauseTemprature = true;
        }

        private void temperatureHumidity_MeasurementComplete(TemperatureHumidity sender, double temperature, double relativeHumidity)
        {
            var str  = String.Concat("T:" , temperature.ToString("F2"), " H:", relativeHumidity.ToString("F2")); 
            var str2 = String.Concat( "Ts:" , _numberTicks.ToString());

            TempAndHumidity tempAndHumidity = new TempAndHumidity()
            {
                TimeSpan = DateTime.Now.Ticks,
                Temperature = temperature,
                Humidity = relativeHumidity
            };

            _tempAndHumidityAccess.Log(tempAndHumidity);

            if (!_pauseTemprature)
            {
                DisplayMessage(str, str2);
            }
            else
            {
                _pauseTempratureCount++;

                if (_pauseTempratureCount >= 2)
                {
                    _pauseTemprature = false;
                    _pauseTempratureCount = 0;
                }
            }
        }

        void DisplayMessage(string line1, string line2)
        {
            char_Display.Clear();
            char_Display.PrintString(line1);
            char_Display.SetCursor(1, 0);
            char_Display.PrintString(line2);
        }

        void Timer_Tick(GT.Timer timer)
        {
            _numberTicks++; 
        }
   
        private void Button_Method_Click(Button sender, Button.ButtonState state)
        {
            _timer.Stop();
            _numberTicks = 0; 
            _timer.Start();
            temperatureHumidity.StartContinuousMeasurements();

            GT.StorageDevice storageDevice = sdCard.GetStorageDevice();

            string[] directories = storageDevice.ListDirectories(@"\");
        }

        void Panic(string message)
        {
            if (message != null && !message.Equals(string.Empty))
            {
                if (_celluarObject == null)
                    _celluarObject = new CellularObject(cellularRadio);

                _celluarObject.SendSms(message);
            }
        }
    }

    #region Redundant Code

    //GPS.Position lastPosition = gps.LastPosition;
    //_pauseTemprature = true;

    //char_Display.Clear();
    //char_Display.PrintString(lastPosition.FixTimeUtc.ToLocalTime().ToString("HH:mm:ss"));

    #endregion
}
