using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Net;

namespace HTTPTunnel
{
    class ClientConnection
    {
        private Socket clientSocket;

        public ClientConnection(Socket client)
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
            bool recvRequest = true;
            string EOL = "\r\n";

            string requestPayload = "";
            string requestTempLine = "";
            List<string> requestLines = new List<string>();
            byte[] requestBuffer = new byte[1];
            byte[] responseBuffer = new byte[1];

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

                this.clientSocket.Send(responseBuffer);

                //State 1: Rebuilding Request Information and Create Connection to Destination Server
                //string remoteHost = requestLines[0].Split(' ')[1].Replace("http://", "").Split('/')[0];
                //string requestFile = requestLines[0].Replace("http://", "").Replace(remoteHost, "");
                //requestLines[0] = requestFile;
                requestLines[5] = "Host: localhost";

                requestPayload = "";
                foreach (string line in requestLines)
                {
                    requestPayload += line;
                    requestPayload += EOL;
                }

                Socket destServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                destServerSocket.Connect("192.168.12.110", 80);
                //IPHostEntry ipHostInfo = Dns.GetHostEntry("google.com");
                //IPAddress ipAddress = ipHostInfo.AddressList[0];
                //destServerSocket.Connect("192.168.12.110", 80);

                //State 2: Sending New Request Information to Destination Server and Relay Response to Client
                destServerSocket.Send(ASCIIEncoding.ASCII.GetBytes(requestPayload));

                //Console.WriteLine("Begin Receiving Response...");
                while (destServerSocket.Receive(responseBuffer) != 0)
                {
                    //Console.Write(ASCIIEncoding.ASCII.GetString(responseBuffer));
                    this.clientSocket.Send(responseBuffer);
                }

                destServerSocket.Disconnect(false);
                destServerSocket.Dispose();
                this.clientSocket.Disconnect(false);
                this.clientSocket.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error Occured: " + e.Message);
                //Console.WriteLine(e.StackTrace);
            }
        }

    }
}
