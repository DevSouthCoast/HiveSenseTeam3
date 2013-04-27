using System;
using System.Collections;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Touch;

using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;
using Gadgeteer.Modules.Seeed;

namespace GadgeteerApp1
{
    public partial class Program
    {
        GT.Timer _timer = new GT.Timer(1000);
        int _numberTicks = 0; 

        // This method is run when the mainboard is powered up or reset.   
        void ProgramStarted()
        {
            /*******************************************************************************************
            Modules added in the Program.gadgeteer designer view are used by typing 
            their name followed by a period, e.g.  button.  or  camera.
            Many modules generate useful events. Type +=<tab><tab> to add a handler to an event, e.g.:
                button.ButtonPressed +=<tab><tab>
            If you want to do something periodically, use a GT.Timer and handle its Tick event, e.g.:
                GT.Timer timer = new GT.Timer(1000); // every second (1000ms)
                timer.Tick +=<tab><tab>
                timer.Start();
            *******************************************************************************************/
            // Use Debug.Print to show messages in Visual Studio's "Output" window during debugging.
            Debug.Print("Program Started");

            button.ButtonPressed += Button_Method_Click;

            _timer.Tick += new GT.Timer.TickEventHandler(Timer_Tick);
            _timer.Start();
            temperatureHumidity.MeasurementComplete+=new TemperatureHumidity.MeasurementCompleteEventHandler(temperatureHumidity_MeasurementComplete);
            temperatureHumidity.StartContinuousMeasurements(); 
        }
  
        private void temperatureHumidity_MeasurementComplete(TemperatureHumidity sender, double temperature, double relativeHumidity)
        {
            var str  = String.Concat("T:" , temperature.ToString("F2"), " H:", relativeHumidity.ToString("F2")); 
            var str2 = String.Concat( "Ts:" , _numberTicks.ToString());

            char_Display.Clear();
            char_Display.PrintString(str); 
            char_Display.SetCursor(1, 0);
            char_Display.PrintString( str2);
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
        }
  
        
    }
}
