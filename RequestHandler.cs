/*
* FILE				: RequestHanlder.cs
* PROJECT			: PROG 2001 - Assignment 05 (myOwnWebServer)
* PROGRAMMERS		: Cody Glanville ID: 8864645
* FIRST VERSION		: November 19, 2023
* DESCRIPTION		:
*	This file is used in order to parse the incoming requests that will be made to our created server. As such, this file contains the
*	class that will be used to handle each request and methods that will help validate the values that clients will be sending. As such, 
*	methods relate to receiving and validating the IP address, the port, the resource, and the version that clients will be sending to 
*	us. 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics.Eventing.Reader;

namespace myOwnWebServer
{
    /*
    * INTERNAL CLASS        : RequestHandler
    * DESCRIPTION	        :
    *	This class is used in order to properly handle a request from a client. As such, methods will relate to validation for
    *	the parsed request strings that will be incoming to the server. Also within this class is the determination of an outgoing
    *	resource or an outgoing error code to the client. 
    */
    internal class RequestHandler
    {
        private string serverIP;
        private string serverFolder;
        private string serverPort;

        private string verb;
        private string resource;
        private string host;
        private string version;

        public int StatusCode { get; set; }

        public string usableFile { get; set; }

        public string contentType { get; set; }

        /*
        *	CONSTRUCTOR     : RequestHandler(string iP, string directory, string port)
        *	DESCRIPTION		:
        *		This constructor is used to construct a RequestHandler object. This class is used in order to properly parse an
        *		incoming request to the created server. Upon instantiation, the private variables will be filled relating to the
        *		values passed into the constructor which relate to the server's IP address, port, and the directory the server uses 
        *		to draw resources from.
        *   PARAMETERS      :
        *		string iP                  :   This is the IP address that will be needed to connect to the server 
        *		string directory:          :   This is the directory that the server uses to hold its resources
        *		string port                :   This is the port that will be used to connect to our server
        *	RETURNS			:
        *		void                       :   Void is used as there no return values
        */
        public RequestHandler(string iP, string directory, string port)
        {
            serverIP = iP;
            serverFolder = directory;
            serverPort = port;
        }

        /*
        *	METHOD          : RequestParser(TcpClient client)
        *	DESCRIPTION		:
        *		This method acts in order to parse an incoming client request for the server which will ultimately determine the response that will
        *		be sent back to the client through a validation process used to determine a status code. Eventually, this status code will be used 
        *		in a separate class that is used to send the proper response to a client (Error or resource).
        *   PARAMETERS      :
        *		TcpClient client        :   This is the client whose request we will be processing
        *	RETURNS			:
        *		void                    :   Void is used as there are no return values for this method
        */
        public void RequestParser(TcpClient client)
        {
            NetworkStream stream = client.GetStream();                     // First create a new stream

            byte[] buffer = new byte[1024];                                // Next we setup our buffer, read from the stream, and decode what was sent
            int bytesToRead = stream.Read(buffer, 0, buffer.Length);
            string requestString = Encoding.UTF8.GetString(buffer, 0, bytesToRead);

            // This block will parse our request string and put the parsed values into private variables that can be used for further validation

            try
            {
                bool canContinue = ValidateSyntax(requestString);

                if (!canContinue)
                {
                    StatusCode = Constants.kBadRequest;                      // There was no Host found in the request string, so we must have gotten a bad request
                    FileOperations.RequestLog("Unavailable", "Unavailable"); // Output to the log, except with 'unavailable' as the verb and resource may be flawed
                    return;
                }
            }
            catch (Exception)
            {
                StatusCode = Constants.kBadRequest;                          // Error happened in validation, request syntax must be invalid
                FileOperations.RequestLog("Unavailable", "Unavailable");     // Output to the log, except with unavailable as the verb and resource as they may be flawed
                return;
            }

            FileOperations.RequestLog(verb, resource);         // Complete our log entry now with our parsed values (verb and resource)

            // Validation begins to check for any specific errors

            if (!ValidateVerb())                               // Will set status code to an error code if anything occurred in verb validation
            {
                return;
            }

            if (!ValidateResource())                           // Can set multiple different StatusCodes for if a file does not exists or an improper extension was used
            {
                return;
            }

            if (!ValidateVersion())                            // Will check the version of HTTP that is being used (we only want HTTP/1.1)
            {
                return;
            }

            if (!ValidateRequestIP())                          // Validates if the IP was properly used
            {
                return;
            }

            StatusCode = Constants.kOK;                        // If the previous errors were not called, we must have a valid request, so the response is OK
            usableFile = resource;                             // Set our file property to be the resource entered
        }

        /*
        *	METHOD          : ValidateVerb()
        *	DESCRIPTION		:
        *		This method is used in order to validate the verb the client has sent to the server. In the case of our server, we will only be
        *		accepting requests that use the GET method. As such, we determine if the parsed value is equal to GET and return a bool based on
        *		the outcome.
        *   PARAMETERS      :
        *		void                    :   Void is used as there are no parameters for this method
        *	RETURNS			:
        *		bool true	            :   True is returned if GET was requested for
        *		bool false              :   False is returned if anything other than GET was used
        */
        private bool ValidateVerb()
        {
            if (verb != Constants.kProperVerb)
            {
                StatusCode = Constants.kMethodNotAloud;
                return false;
            }
            return true;
        }

        /*
        *	METHOD          : ValidateResource()
        *	DESCRIPTION		:
        *		This method is used in order to validate the resource that the server was requesting for. As such, the resource is combined
        *		with the directory the server draws its resources from in order to determine if the file exists and also determine its file 
        *		extension (which will end up determining the content-type of the file if the resource can be used).
        *   PARAMETERS      :
        *		void                    :   Void is used as there are no parameters for this method
        *	RETURNS			:
        *		bool true	            :   True is returned if the file is valid and the extension is usable
        *		bool false              :   False is returned if the file is invalid
        */
        private bool ValidateResource()
        {
            string extension = Path.GetExtension(resource);

            if (string.IsNullOrEmpty(extension))
            {
                // No extension, syntax error

                StatusCode = Constants.kBadRequest;
                return false;
            }

            string fullPath = serverFolder + resource;

            if (!File.Exists(fullPath))
            {
                // The file does not exist, not found error

                StatusCode = Constants.kNotFound;
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

            // We reached the end, so an invalid media type must have been used at this point, therefore we can use the forbidden error

            StatusCode = Constants.kForbidden;
            return false;
        }

        /*
        *	METHOD          : ValidateVersion()
        *	DESCRIPTION		:
        *		This method is used to validate the HTTP version that was sent to the server. Since our server only uses HTTP/1.1, we must validate
        *		that the version sent to us is the one that we accept. If the version is anything other than HTTP/1.1, false will be returned at which
        *		point the proper status code will be decided for the response from our server.
        *   PARAMETERS      :
        *		void                    :   Void is used as there are no parameters for this method
        *	RETURNS			:
        *		bool true	            :   True is returned if the version was HTTP/1.1
        *		bool false              :   False is returned if the version was NOT HTTP/1.1
        */
        private bool ValidateVersion()
        {
            if (version != Constants.kVersion)
            {
                StatusCode = Constants.kInvalidHTTPVersion;
                return false;
            }
            return true;
        }

        /*
        *	METHOD          : ValidateRequestIP()
        *	DESCRIPTION		:
        *		This method is used in order to validate the IP address that was used in the request to our server. As such, we need to validate this
        *		value to ensure it is the same as the IP that our server is running and listening on. Due to this, we will validate that the IP and Port 
        *		both match our server, otherwise a false value is returned.
        *   PARAMETERS      :
        *		void                      :   Void is used as there are no parameters for this method
        *	RETURNS			:
        *		bool true	              :   True is returned if the requested IP and Port was the same as the server's IP address port
        *		bool false                :   False is returned if the IP and Port was NOT the same as the server's IP address and port
        */
        private bool ValidateRequestIP()
        {
            // Need to split the two parts up (IP : Host)
            // IP Can be the servers IP it is using OR Localhost
            // Port must be the port we are using

            string parsedIP;
            string parsedPort;

            int colonIndex = host.IndexOf(':');

            if (colonIndex != -1)
            {
                parsedIP = host.Substring(0, colonIndex);
                parsedPort = host.Substring(colonIndex + 1);

                if (parsedIP != serverIP && parsedIP.ToLower() != "localhost")
                {
                    StatusCode = Constants.kBadGateway;
                    return false;
                }

                if (parsedPort != serverPort)
                {
                    StatusCode = Constants.kBadGateway;
                    return false;
                }
            }
            else
            {
                StatusCode = Constants.kBadGateway;
                return false;
            }
            return true;
        }

        /*
        *	METHOD          : ValidateSyntax(string request)
        *	DESCRIPTION		:
        *		This method is used in order to validate the syntax of the client request for the server. As such, it will go through a variety 
        *		of checks to determine if we can properly parse the first line of the requests including the method, the resource, and the version, 
        *		and it will also check if a host line was included anywhere in the response as well. If it was, it will parse the string accordingly 
        *		and set property values to be used for further validation.
        *   PARAMETERS      :
        *		string request            :   This is the string containing the request that a client has sent to the server
        *	RETURNS			:
        *		bool true	              :   True is returned if the request can be properly parsed indicating proper syntax of the request
        *		bool false                :   False is returned if the request was deemed invalid and bad syntax was used
        */
        private bool ValidateSyntax(string request)
        {
            string[] parsedRequest = request.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

            string[] firstLineParts = parsedRequest[0].Split(' ');  // Used to break the first line of the parsed request into its separate parts

            foreach (string line in parsedRequest)
            {
                if (line.ToUpper().StartsWith("HOST: "))            // Need to find a line that includes the host
                {
                    host = line.Substring(6);
                    break;
                }
            }

            if (host == null)                                       // No host was found
            {
                return false;
            }

            // Set our private data members

            verb = firstLineParts[0];                               // Gather each individual part of the first line to check for errors

            resource = firstLineParts[1].Replace('/', '\\');        // Get our proper file path

            resource = Uri.UnescapeDataString(resource);            // Recieve the actual path if there were any spaces in it (they're treated differently in encoding URL)

            version = firstLineParts[2];                            // Grab the version number

            return true;
        }
    }
}
