using System;

namespace Data
{
    public class TempAndHumidity
    {
        public long TimeSpan { get; set; }

        public double Temperature { get; set; }

        public double Humidity { get; set; }

        internal string ToCsvString()
        {
            // TODO: Clean this up later!
            string value = this.TimeSpan.ToString() + "," + this.Temperature.ToString("F2") + "," + this.Humidity.ToString("F2");

            return value;
        }
    }
}
