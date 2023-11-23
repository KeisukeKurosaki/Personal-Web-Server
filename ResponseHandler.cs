using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using Microsoft.SqlServer.Server;
using System.Security.Cryptography;

namespace myOwnWebServer
{
    internal class ResponseHandler
    {
        TcpClient client { get; set; }

        private string statusCode;

        private string file;

        private string contentType;

        private string fileDirectory;

        private long usedLength;

        public ResponseHandler(TcpClient serverClient, string resource, int status, string content, string directory)
        {
            client = serverClient;
            statusCode = status.ToString();
            file = resource;  
            contentType = content;
            fileDirectory = directory;
        }

        public void ResponseSender()
        {
            NetworkStream stream = client.GetStream();

            string errorResponseHTTP;
            string responseHTTP;

            if (statusCode != "200")                                // We have an error, so we use the proper method to create a string
            {
                NetworkStream errorStream = client.GetStream();

                errorResponseHTTP = ErrorResponseString(statusCode);

                byte[] errorResponseBytes = Encoding.UTF8.GetBytes(errorResponseHTTP);

                errorStream.Write(errorResponseBytes, 0, errorResponseBytes.Length);

                errorStream.Close();

                // Write the response to the server's log after the response has been sent

                FileOperations.ResponseLogError(statusCode);
            }
            else
            {
                // Need the file extension to open the file and output its contents

                NetworkStream goodStream = client.GetStream();

                string fullPath = fileDirectory + file;

                byte[] fileContents = File.ReadAllBytes(fullPath);

                responseHTTP = CorrectResponseString(statusCode, file, fileDirectory, contentType);

                byte[] httpResponseBytes = Encoding.UTF8.GetBytes(responseHTTP);

                byte[] fullResponse = new byte[httpResponseBytes.Length + fileContents.Length];

                Array.Copy(httpResponseBytes, fullResponse, httpResponseBytes.Length);
                Array.Copy(fileContents, 0, fullResponse, httpResponseBytes.Length, fileContents.Length);

                stream.Write(fullResponse, 0, fullResponse.Length);

                stream.Close();

                FileOperations.ResponseLogOK(contentType, usedLength);
            }
        }

        public string ErrorResponseString(string code)
        {
            // Access the error type through the dictionary value
            // Content-length is not needed here due to it being an error

            string response = "HTTP/1.1 " + code + " " + Constants.HTTPDictionary[code] +"\r\n" +
                                   "Content-Type: text/html\r\n" +
                                   "Content-Length: 0\r\n" +
                                   "Server: myOwnWebserver\r\n" +
                                   "Date: " + DateTime.Now.ToString("r") + "\r\n" +
                                   "\r\n";

            return response;
        }

        public string CorrectResponseString(string code, string resource, string directory, string type)
        {
            // Need the file directory and specific file in order to read its size 
            // Then, output the header

            // Combine both strings, then get the file info of the combined path instead lol
            string fullPath = directory + resource;

            FileInfo fileInfo = new FileInfo(fullPath);             // File exists, so we can get its info and its size to put into the response header
            long fileSize = fileInfo.Length;

            usedLength = fileSize;                                  // Setup the file size to a property to be able to properly write to our log once sent

            string response = "HTTP/1.1 " + code + " " + Constants.HTTPDictionary[code] + "\r\n" +
                                   "Content-Type: "+ type +"\r\n" +
                                   "Content-Length: "+ fileInfo.ToString() +"\r\n" +
                                   "Server: myOwnWebserver\r\n" +
                                   "Date: " + DateTime.Now.ToString("r") + "\r\n" +
                                   "\r\n";

            return response;
        }
    }
}
