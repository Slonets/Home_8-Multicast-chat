using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class Connect
    {
        public string Name { get; set; }

        public IPEndPoint RemoteEndPoint { get; set; }

        public Connect(string name, IPEndPoint remoteEndPoint)
        {
            Name = name;
            RemoteEndPoint = remoteEndPoint;
        }
    }
}
