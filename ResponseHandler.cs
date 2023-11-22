using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace myOwnWebServer
{
    internal class ResponseHandler
    {
        TcpClient client { get; set; }

        private string statusCode;

        public ResponseHandler(TcpClient serverClient, int status)
        {
            client = serverClient;
            statusCode = status.ToString();
        }

        public void ResponseSender()
        {
            NetworkStream stream = client.GetStream();

            string responseHTTP = "HTTP/1.1" + statusCode + "Not Found\r\n" +
                                   "Content-Type: text/html\r\n" +
                                   "Content-Length: 0\r\n" +
                                   "Server: myOwnWebserver\r\n" +
                                   "Date: " + DateTime.Now.ToString("r") + "\r\n" +
                                   "\r\n";

            byte[] httpResponseBytes = Encoding.UTF8.GetBytes(responseHTTP);

            stream.Write(httpResponseBytes, 0, httpResponseBytes.Length);

            stream.Close();
        }
    }
}
