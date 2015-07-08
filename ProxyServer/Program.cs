using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ProxyServer
{
    class Program
    {

        static void Main(string[] args)
        {
            Thread clientHandler = new Thread(ClientHandler);
            clientHandler.Priority = ThreadPriority.Normal;
            clientHandler.Start();

            Thread handler = new Thread(ProxyHandler);
            handler.Priority = ThreadPriority.Normal;
            handler.Start();
        }

        public static void ProxyHandler()
        {
            new ProxyHTTPServer().Begin();
        }

        public static void ClientHandler()
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
