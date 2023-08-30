using System.Net.Sockets;
using System.Net;
using System.Text;
using System;

namespace Server
{
    internal class Program
    {
        static private UdpClient udpClient = new UdpClient();
        static private IPEndPoint remoteEndPoint;
        static void Main(string[] args)
        {
            
            IPAddress multicastAddress = IPAddress.Parse("239.0.0.1");
            
            int port = 8001;
            remoteEndPoint = new IPEndPoint(multicastAddress, port);

            while (true)
            {                

                Console.Write($"Input message = ");
                string message = Console.ReadLine();

                SendMessage(message);
            }

        }

        public static async void SendMessage(string message)
        {

            byte[] data = Encoding.Unicode.GetBytes(message);
                        
            await udpClient.SendAsync(data, remoteEndPoint);            
        }

    }    
}