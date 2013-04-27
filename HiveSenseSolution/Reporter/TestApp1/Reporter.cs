using System;
using Reporter;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace HiveSense
{

    public class Reporter
    {

        public static UdpSocketClient client;
        private string API_KEY;

        public Reporter(String apiKey)
        {
            API_KEY = apiKey;
            client = new UdpSocketClient();
        }

        public bool ReportMetrics(HiveSenseMetric[] metrics, HiveMetric metricType)
        {
            var metric_name = "test_metric";
            var value = 0.17;
            String wireString = this.API_KEY + "." + metric_name + " " + value.ToString();
            var bytes = Encoding.UTF8.GetBytes(wireString);
            client.Send(bytes);
            return true;
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
            this.port = port;
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            var endpoint = GetIPEndPointFromHostName(host, port, false);
            socket.Connect(endpoint);
        }

        public void Send(byte[] buffer)
        {
            socket.Send(buffer);
        }

        public IPEndPoint GetIPEndPointFromHostName(string hostName, int port, bool throwIfMoreThanOneIP)
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
    }
}



