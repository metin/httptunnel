using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace ProxyServer
{
    class ServiceCaller
    {
        private Socket clientSocket;

        public ServiceCaller(Socket client)
        {
            this.clientSocket = client;
        }

        public void StartHandling()
        {
            Thread handler = new Thread(Handler);
            handler.Priority = ThreadPriority.Normal;
            handler.Start();
        }

        private void Handler()
        {
            // USE HTTP client instead of raw socket
            bool recvRequest = true;
            string EOL = "\r\n";

            string requestPayload = "";
            string requestTempLine = "";
            List<string> requestLines = new List<string>();
            byte[] requestBuffer = new byte[1];
            byte[] responseBuffer = new byte[1];
            Socket destServerSocket = null;
            requestLines.Clear();

            try
            {
                //State 0: Handle Request from Client
                while (recvRequest)
                {
                    this.clientSocket.Receive(requestBuffer);
                    string fromByte = ASCIIEncoding.ASCII.GetString(requestBuffer);
                    requestPayload += fromByte;
                    requestTempLine += fromByte;

                    if (requestTempLine.EndsWith(EOL))
                    {
                        requestLines.Add(requestTempLine.Trim());
                        requestTempLine = "";
                    }

                    if (requestPayload.EndsWith(EOL + EOL))
                    {
                        recvRequest = false;
                    }
                }
                Console.WriteLine("Raw Request Received...");
                Console.WriteLine(requestPayload);



            }
            catch (Exception e)
            {
                Console.WriteLine("Error Occured: " + e.Message);
                //Console.WriteLine(e.StackTrace);
            }
            finally
            {
                Console.WriteLine("Close all");
                if (destServerSocket != null)
                {
                    destServerSocket.Disconnect(false);
                    destServerSocket.Dispose();
                }

                this.clientSocket.Disconnect(false);
                this.clientSocket.Dispose();
            }
        }

    }
}
