using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace FarmClient
{
    class Program
    {

        static void Main(string[] args)
        {
            byte[] requestBuffer = new byte[1];
            byte[] responseBuffer = new byte[1];
            bool recvRequest = true;
            string EOL = "\r\n";

            string requestPayload = "";
            string requestTempLine = "";
            List<string> requestLines = new List<string>();

            Socket proxSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            proxSocket.Connect("192.168.12.110", 9000);

            try
            {

                proxSocket.Send(ASCIIEncoding.ASCII.GetBytes("FARM1\r\n\r\n"));
                proxSocket.Receive(responseBuffer); //Get ACK
                String resp = responseBuffer.ToString();
                Console.WriteLine("Begin Receiving Response...");
                while (true)
                {

                    while (recvRequest)
                    {
                        proxSocket.Receive(requestBuffer);
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

                    Console.WriteLine(requestPayload);
                }
            }
            catch { }

        }   
    }
}
