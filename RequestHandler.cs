using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace myOwnWebServer
{
    internal class RequestHandler
    {
        private string serverIP;
        private string serverFolder;
        private string serverPort;

        public int StatusCode { get; set; }

        public string usableFile { get; set; }

        public string contentType { get; set; }

        public RequestHandler(string iP, string directory, string port)
        {
            serverIP = iP;
            serverFolder = directory;
            serverPort = port;
        }

        public void RequestParser(TcpClient client)
        {
            // First create a new stream

            NetworkStream stream = client.GetStream();

            // Next we setup our buffer, read from the stream, and decode what was sent

            byte[] buffer = new byte[1024];
            int bytesToRead = stream.Read(buffer, 0, buffer.Length);
            string requestString = Encoding.UTF8.GetString(buffer, 0, bytesToRead);

            // Now we need to parse the request to get our separate pieces and remove the empty entries to only get our needed values to help in creating a response


            // IMPLEMENT TRY CATCH WITH AN OBJECT OR SOMETHING HERE -=============================================================


            string[] parsedRequest = requestString.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            string[] firstLineParts = parsedRequest[0].Split(' ');  // Used to break the first line of the parsed request into its separate parts

            string hostLineString = parsedRequest[1];               // Used to get the line that should hold the host on it

            // CHECK IF RESOURCE EXISTS

            string verb = firstLineParts[0];                        // Gather each individual part of the first line to check for errors

            string resource = firstLineParts[1].Replace('/', '\\'); // Get our proper file path

            resource = Uri.UnescapeDataString(resource);            // Recieve the actual path if there were any spaces in it (they're treated differently in encoding URL)

            string version = firstLineParts[2];                     // Grab the version number

            FileOperations.RequestLog(verb, resource);              // Complete our log entry now with our parsed values

            string host = "";

            if (hostLineString.StartsWith("HOST: ") || hostLineString.StartsWith("Host: "))               // Determine if the second request line contains a host
            {
                host = hostLineString.Substring(6).Trim();
            }
            else
            {
                StatusCode = Constants.kBadRequest;
                return;
            }

            // ERROR CHECK EACH REQUEST (CHECK FOR IF ITS GET, PROPER FILE EXTENSION, VALID HOST, AND VERSION NUMBER


            if (!ValidateVerb(verb))
            {
                StatusCode = Constants.kMethodNotAloud;
                return;
            }

            if (!ValidateResource(resource))
            {
                StatusCode = Constants.kForbidden;
                return;
            }

            if (!ValidateVersion(version))
            {
                StatusCode = Constants.kInvalidHTTPVersion;
                return;
            }

            if (!ValidateRequestIP(host))
            {
                StatusCode = Constants.kBadGateway;
                return;
            }

            // All the validation was passed, so we set the status code at the end to be OK and also our resource
            StatusCode = 200;
            usableFile = resource;
        }

        private bool ValidateVerb(string verb)
        {
            if (verb != Constants.kProperVerb)
            {
                return false;
            }
            return true;
        }

        private bool ValidateResource(string resource)
        {
            string extension = Path.GetExtension(resource);

            if (string.IsNullOrEmpty(extension))
            {
                return false;
            }

            string fullPath = serverFolder + resource;

            if (!File.Exists(fullPath))
            {
                return false;
            }


            foreach (var usableExtension in Constants.kHTMLFamily)
            {
                if (extension == usableExtension)
                {
                    contentType = Constants.kContentTypeHTML;
                    return true;
                }
            }

            foreach (var usableExtension in Constants.KJPGFamily)
            {
                if (extension == usableExtension)
                {
                    contentType = Constants.kContentTypeJPG;
                    return true;
                }
            }

            if (extension == Constants.kGifExt)
            {
                contentType = Constants.kContentTypeGIF;
                return true;
            }

            if (extension == Constants.kTextExt)
            {
                contentType = Constants.kContentTypeText;
                return true;
            }
            return false;
        }

        private bool ValidateVersion(string version)
        {
            if (version != Constants.kVersion)
            {
                return false;
            }
            return true;
        }

        private bool ValidateRequestIP(string requestIP)
        {
            // Need to split the two parts up (IP : Host)
            // IP Can be the servers IP it is using OR Localhost
            // Port must be the port we are using


            string parsedIP;
            string parsedPort;

            int colonIndex = requestIP.IndexOf(':');

            if (requestIP == "localhost")
            {
                return true;
            }


            if (colonIndex != -1)
            {
                parsedIP = requestIP.Substring(0, colonIndex);
                parsedPort = requestIP.Substring(colonIndex + 1);

                if (parsedIP != serverIP)
                {
                    return false;
                }

                if (parsedPort != serverPort)
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
            return true;
        }
    }
}
