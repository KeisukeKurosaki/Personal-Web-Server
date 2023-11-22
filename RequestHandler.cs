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

        public int StatusCode { get; set; }

        public RequestHandler(string iP, string directory)
        {
            serverIP = iP;
            serverFolder = directory;
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

            string[] parsedRequest = requestString.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            string[] firstLineParts = parsedRequest[0].Split(' ');  // Used to break the first line of the parsed request into its separate parts

            string hostLineString = parsedRequest[1];               // Used to get the line that should hold the host on it

            if (parsedRequest.Length != 2)                          // Cant handle requests with any more arguments
            {
                StatusCode = Constants.kBadRequest;
                return;
            }

            string host = "";

            if (hostLineString.StartsWith("HOST: "))                // Determine if the second request line contains a host
            {
                host = hostLineString.Substring(6).Trim();
            }
            else
            {
                StatusCode = Constants.kBadRequest;
                return;
            }

            // CHECK IF RESOURCE EXISTS

            string verb = firstLineParts[0];                        // Gather each individual part of the first line to check for errors
            string resource = firstLineParts[1];
            string version = firstLineParts[2];

            // ERROR CHECK EACH REQUEST (CHECK FOR IF ITS GET, PROPER FILE EXTENSION, VALID HOST, AND VERSION NUMBER
            string outGoingError = null;

            if (!ValidateVerb(verb))
            {
                outGoingError = "< Method used that is not aloud >";
                StatusCode = Constants.kMethodNotAloud;
            }

            if (!ValidateResource(resource))
            {
                outGoingError = "< Resource used is not supported >";
                StatusCode = Constants.kForbidden;
                ;
            }

            if (!ValidateVersion(version))
            {
                outGoingError = "< Invalid HTTP version requested >";
                StatusCode = Constants.kInvalidHTTPVersion;
            }

            if (!ValidateRequestIP(host))
            {
                outGoingError = "< Incorrect IP address was requested >";
                StatusCode = Constants.kBadGateway;
            }

            if (outGoingError == null)              // If the outGoingError is still null, nothing wrong was detected with the values requested, so continue
            {
                StatusCode = 200;
            }
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

            if (!string.IsNullOrEmpty(extension))
            {
                return false;
            }

            string fullPath = Path.Combine(serverFolder, resource);

            if (!File.Exists(fullPath))
            {
                return false;
            }


            foreach (var usableExtension in Constants.kHTMLFamily)
            {
                if (extension == usableExtension)
                {
                    return true;
                }
            }

            foreach (var usableExtension in Constants.KJPGFamily)
            {
                if (extension == usableExtension)
                {
                    return true;
                }
            }

            if (extension == Constants.kGifExt || extension == Constants.kTextExt)
            {
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
            if (requestIP != serverIP)
            {
                return false;
            }
            return true;
        }
    }
}
