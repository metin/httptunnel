using ProxyServer;
using System;
using System.Net;
using System.Net.Sockets;

namespace ProxyServer
{
    public class ServerListener
    {
        private int listenPort;
        private TcpListener listener;

        public ServerListener(int port)
        {
            this.listenPort = port;
            this.listener = new TcpListener(IPAddress.Any, this.listenPort);
        }
 
        public void StartServer()
        {
            this.listener.Start();
        }
 
        public void AcceptConnection()
        {
            Console.WriteLine("Accepting connections...");
            Socket newClient = this.listener.AcceptSocket();
            ClientRegistrar.Instance.Register(newClient);           
            //ClientConnection client = new ClientConnection(newClient);
            //client.StartHandling();
        }
 
    }
}
