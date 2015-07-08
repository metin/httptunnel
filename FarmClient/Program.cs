using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;

namespace FarmClient
{
    class Program
    {

        static void Main(string[] args)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string s in args)
            {
                sb.Append(s + "\n\r");
            }
            Console.WriteLine("Args: \n\r");
            Console.WriteLine(sb.ToString());
            string serverIP = args[0];

            byte[] requestBuffer = new byte[1];
            byte[] responseBuffer = new byte[1024];
            bool recvRequest = true;
            string EOL = "\r\n";

            string requestPayload = "";
            string requestTempLine = "";
            List<string> requestLines = new List<string>();

            Socket proxSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            proxSocket.Connect(serverIP, 9000);

            try
            {

                string str = String.Format("FARM1: {0}", System.Guid.NewGuid().ToString());
                Console.WriteLine(str);
                proxSocket.Send(ASCIIEncoding.ASCII.GetBytes(str));
                

                proxSocket.Receive(responseBuffer); //Get ACK
                String resp = ASCIIEncoding.ASCII.GetString(responseBuffer);
                Console.WriteLine("OK: From Server: {0}", resp);
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
                    foreach (var s in requestLines)
                    {
                        Console.WriteLine(s);
                    }
                    Console.WriteLine(requestPayload);
                    Uri myuri = new Uri("http://localhost/" + requestLines[0]);
                    HttpWebRequest http = (HttpWebRequest)HttpWebRequest.Create(myuri);

                    HttpWebResponse re = http.GetResponse() as HttpWebResponse;

                    using (StreamReader sr = new StreamReader(re.GetResponseStream()))
                    {
                        string responseJson = sr.ReadToEnd();
                        Console.WriteLine(responseJson);
                        proxSocket.Send(ASCIIEncoding.ASCII.GetBytes(responseJson));
                        // more stuff
                    }
                    Console.WriteLine(requestPayload);


                    recvRequest = true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }   
    }
}
