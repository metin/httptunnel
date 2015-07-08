using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace ProxyServer
{
    class ClientRegistrar
    {

        private Dictionary<string, Socket> clients;
        private static volatile ClientRegistrar instance;
        private static object syncRoot = new Object();


        private ClientRegistrar()
        {
            clients = new Dictionary<string, Socket>();
        }

        public static ClientRegistrar Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new ClientRegistrar();
                    }
                }
                return instance;
            }
        }

        public void Register(String clientID, Socket socket)
        {
            Console.WriteLine("Registering Client ID: {0}", clientID);
            clients[clientID] = socket;
        }

        public Socket Find(string cid)
        {
            KeyValuePair<string, Socket> pair = clients.First();
            return pair.Value;
        }

        public void Register(Socket clientSocket)
        {

            byte[] buffer = new byte[1024];

            Console.WriteLine("Waiting for client ID");
            clientSocket.Receive(buffer);
            string clientID = ASCIIEncoding.ASCII.GetString(buffer);
            Register(clientID.Trim(), clientSocket);            
            clientSocket.Send(ASCIIEncoding.ASCII.GetBytes("OK: Registered!"));
            

        }
    }
}
