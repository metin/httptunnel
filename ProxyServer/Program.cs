using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HTTPTunnel
{
    class Program
    {
        static void Main(string[] args)
        {
            ServerListener simpleHttpProxyServer = new ServerListener(9000);
            simpleHttpProxyServer.StartServer();
            while (true)
            {
                simpleHttpProxyServer.AcceptConnection();
            }
        }
    }
}
