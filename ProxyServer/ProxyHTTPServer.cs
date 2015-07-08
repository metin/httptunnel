using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ProxyServer
{
    class ProxyHTTPServer
    {
        public ProxyHTTPServer()
        {

        }

        public void Begin()
        {
            HttpListener listener = new HttpListener();
            //listener.Prefixes.Add("http://localhost:8080/");
            listener.Prefixes.Add("http://+:8080/");
            listener.Start();

            while (true)
            {
                Console.WriteLine("Listening");

                HttpListenerContext ctx = listener.GetContext();
                Console.WriteLine("Received this request");
                Console.WriteLine(ctx.Request.ToString());
                Console.WriteLine(ctx.Request.ContentType);
                Console.WriteLine(ctx.Request.UserAgent);

                //Console.WriteLine(ctx.Request.InputStream.Read());
                Console.WriteLine("------END------");
                //    HttpWebRequest newRequest = (HttpWebRequest)WebRequest.Create("http://192.168.58.164");
                //newRequest.Headers = ctx.Request.Headers.;
                //newRequest.ContentType = ctx.Request.ContentType;
                //newRequest.UserAgent = ctx.Request.UserAgent;
                //byte[] originalStream = ReadToByteArray(ctx.Request.InputStream, 1024);

                //Stream reqStream = newRequest.GetRequestStream();
                //reqStream.Write(originalStream, 0, originalStream.Length);
                //reqStream.Close();

                Socket s = ClientRegistrar.Instance.Find("clientID");
                s.Send(ASCIIEncoding.ASCII.GetBytes(ctx.Request.RawUrl + "\r\n\r\n"));

                byte[] rbuffer = new byte[20480];
                s.Receive(rbuffer);
                string fromByte = ASCIIEncoding.ASCII.GetString(rbuffer);
                HttpListenerResponse response = ctx.Response;
                // Construct a response. 
                string responseString = string.Format("<HTML><BODY> Hello world! <br/> Time Now: {0} </BODY></HTML>", DateTime.Now.ToString());
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(fromByte);
                // Get a response stream and write the response to it.
                response.ContentLength64 = buffer.Length;
                System.IO.Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                // You must close the output stream.
                output.Close();
            }


        }
    }
}
