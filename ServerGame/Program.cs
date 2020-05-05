using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ServerGame
{
    class Program
    {
      //  static int port = 8005;
        static void Main(string[] args)
        {
            TcpGameServer.StartListen();
        }
    }
}
