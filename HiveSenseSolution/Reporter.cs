using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using GadgeteerApp1.Data;


namespace HiveSense
{

    public class Reporter
    {


        public readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0); 

        public double GetTimeStamp(DateTime dt)
        {
            TimeSpan span = (dt - epoch);
            return span.Ticks / (double)TimeSpan.TicksPerSecond;
        }

        public static UdpSocketClient client;
        private string API_KEY;

        public Reporter(String apiKey)
        {
            API_KEY = apiKey;
            client = new UdpSocketClient();
        }

        public bool ReportMetrics(TempAndHumidity[] metrics, String metric_prefix = "test_metric")
        {
            var value = 3;
            for (int i = 0; i < metrics.Length; i++)
            {
                var metric = metrics[i];
                var metric_type = "temp";

                // TODO  - Pass the actual date from the metric through
                SendMetric(metric_prefix, "temp", metric.Temperature, DateTime.UtcNow);
                SendMetric(metric_prefix, "humidity", metric.Humidity, DateTime.UtcNow);
            }

            return true;
        }

        private void SendMetric(string metric_prefix, string metric_type, double metric, DateTime dt)
        {
            String wireString = this.API_KEY + "." + metric_prefix + "." + metric_type + " " +
                                metric.ToString();
            if (dt != DateTime.MinValue)
            {
                wireString += " " + ((int)GetTimeStamp(dt)).ToString();
            }

            var bytes = Encoding.UTF8.GetBytes(wireString);
            client.Send(bytes);
        }
    }


    public class UdpSocketClient
    {
        public const int DEFAULT_CLIENT_PORT = 2003;
        public const String DEFAULT_CLIENT_HOST = "carbon.hostedgraphite.com";

        private int port;
        private Socket socket;

        public UdpSocketClient()
            : this(DEFAULT_CLIENT_PORT, DEFAULT_CLIENT_HOST)
        {
        }

        public UdpSocketClient(int port, String host)
        {
            try
            {
                this.port = port;
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                var endpoint = GetIPEndPointFromHostName(host, port, false);
                socket.Connect(endpoint);
            }
            catch (Exception)
            {
                return;
            }

        }

        public void Send(byte[] buffer)
        {
            try
            {
                socket.Send(buffer);
            }
            catch (Exception ex)
            {
                return;
            }
        }

        public IPEndPoint GetIPEndPointFromHostName(string hostName, int port, bool throwIfMoreThanOneIP)
        {
            try
            {
                var addresses = Dns.GetHostEntry(hostName);
                if (addresses.AddressList.Length == 0)
                {
                    throw new ArgumentException(
                        "Unable to retrieve address from specified host name.",
                        "hostName"
                        );
                }
                else if (throwIfMoreThanOneIP && addresses.AddressList.Length > 1)
                {
                    throw new ArgumentException(
                        "There is more that one IP address to the specified host.",
                        "hostName"
                        );
                }
                return new IPEndPoint(addresses.AddressList[0], port); // Port gets validated here.       
            }
            catch (Exception ex)
            {
                return null;

            }

        }
    }
}



