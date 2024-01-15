/*
* FILE				: ResponseHandler.cs
* PROJECT			: PROG 2001 - Assignment 05 (myOwnWebServer)
* PROGRAMMERS		: Cody Glanville ID: 8864645
* FIRST VERSION		: November 19, 2023
* DESCRIPTION		:
*	This file is used in order to hold the class that contains the logic for sending out responses to clients.
*	As such, this file contains methods relating to sending out error messages or proper responses that the 
*	client requested for. This all depends on if the "200" status code was determined or if an error was 
*	found at any point in the validation for checking the recieved request.
*/

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
    /*
    * INTERNAL CLASS        : ResponseHandler
    * DESCRIPTION	        :
    *	This class is used in order to properly handle a response that will be sent to a client. As such, this class holds the methods
    *	used in order to build a proper HTTP header to output and also determine the content to add after the header. Responses will either 
    *	be an error message or a valid resource to show to the user.
    */
    internal class ResponseHandler
    {
        TcpClient client { get; set; }

        private string statusCode;

        private string file;

        private string contentType;

        private string fileDirectory;

        private long usedLength;

        /*
        *	CONSTRUCTOR     : ResponseHandler(TcpClient serverClient, string resource, string status, string content, string directory)
        *	DESCRIPTION		:
        *		This constructor is used to construct a RequestHandler object. This class is used in order to properly create a 
        *		response for clients who have requested the services of our server. As such, the methods used in this class relate
        *		to properly creating an HTTP header for a response and also deal with encoding this response to send to a client.
        *   PARAMETERS      :
        *		TcpClient serverClient     :    This is the client that the server will be sending a response to
        *		string resource            :    This is the resource that the client will be accessing (or not)
        *		string status              :    This is the status code that will have been created from reading the request
        *		string content             :    This is the content-type that will be used for the response
        *		string directory           :    This is the directory that resources are held in for the server
        *	RETURNS			:
        *		void                       :    Void is used as there no return values
        */
        public ResponseHandler(TcpClient serverClient, string resource, string status, string content, string directory)
        {
            client = serverClient;
            statusCode = status.ToString();
            file = resource;  
            contentType = content;
            fileDirectory = directory;
        }

        /*
        *	METHOD          : ResponseSender()
        *	DESCRIPTION		:
        *		This method is used in order to either send out the proper error message to a client or the resouce they wish to access.
        *		There are two paths that can be taken, the error path or the success path depending on the status code recieved from the
        *		client request. In each case, a proper HTTP response is sent back to the client.
        *   PARAMETERS      :
        *		void               :    Void is used as there are no parameters for this method
        *	RETURNS			:
        *		void               :    Void is used as there are no return values for this method
        */
        public void ResponseSender()
        {
            NetworkStream stream = client.GetStream();

            string errorResponseHTTP;
            string responseHTTP;

            if (statusCode != Constants.kOutputDecider)                                // We have an error, so we use the proper method to create a string
            {
                NetworkStream errorStream = client.GetStream();

                errorResponseHTTP = ErrorResponseString();

                byte[] errorResponseBytes = Encoding.UTF8.GetBytes(errorResponseHTTP);

                errorStream.Write(errorResponseBytes, 0, errorResponseBytes.Length);

                errorStream.Close();

                FileOperations.ResponseLogError(statusCode);                          // Write the response to the server's log after the response has been sent

            }
            else
            {
                // The request has been deemed successful through validation, now we need to get the resource and display its contents

                NetworkStream goodStream = client.GetStream();

                string fullPath = fileDirectory + file;                               // Create the full path to the resouce

                byte[] fileContents = File.ReadAllBytes(fullPath);

                responseHTTP = CorrectResponseString();

                byte[] httpResponseBytes = Encoding.UTF8.GetBytes(responseHTTP);

                byte[] fullResponse = new byte[httpResponseBytes.Length + fileContents.Length];

                Array.Copy(httpResponseBytes, fullResponse, httpResponseBytes.Length);
                Array.Copy(fileContents, 0, fullResponse, httpResponseBytes.Length, fileContents.Length);

                stream.Write(fullResponse, 0, fullResponse.Length);                  // We combine our response into a single response to the client

                stream.Close();

                FileOperations.ResponseLogOK(contentType, usedLength);               // Output to the server log that the response has been sent
            }
        }

        /*
        *	METHOD          : ErrorResponseString()
        *	DESCRIPTION		:
        *		This method is used in order to generate a proper error response for a client who requested a service from our server.
        *		However, in this case, an error was genereated and as such it needs to be returned to the client. The method will take
        *		the status code and make the proper response to send to the user and also properly set the byte size of the error to be 
        *		displayed for the user as well.
        *   PARAMETERS      :
        *       void                    :    Void is used as there are no parameters
        *	RETURNS			:
        *		string response         :    This is the string containing the response to the client
        */
        public string ErrorResponseString()
        {
            // Access the error type through the dictionary value
            // Content-length is decided upon based on the size of the status codes dictionary value

            string errorOutput = statusCode + " " + Constants.HTTPDictionary[statusCode];

            int byteSize = Encoding.UTF8.GetByteCount(errorOutput);

            string response = "HTTP/1.1 " + statusCode + " " + Constants.HTTPDictionary[statusCode] +"\r\n" +
                                   "Content-Type: text/html\r\n" +
                                   "Content-Length: "+ byteSize + "\r\n" +
                                   "Server: myOwnWebserver\r\n" +
                                   "Date: " + DateTime.Now.ToString("r") + "\r\n" +
                                   "\r\n" + errorOutput;

            return response;
        }

        /*
        *	METHOD          : CorrectResponseString()
        *	DESCRIPTION		:
        *		This method is used in order to generate a proper response to a user in order to display the content they requested
        *		for. As such, we need to access the resouce they requested for, get its size in bytes, and then generate the proper 
        *		response header for the client to recieve. The HTTP response header is returned as a string.
        *   PARAMETERS      :
        *		void                :    Void is used as there are no parameters
        *	RETURNS			:
        *		string response     :    This is the string containing the response to the client
        */
        public string CorrectResponseString()
        {
            string fullPath = fileDirectory + file;                 // Get the full path of our validated file

            FileInfo fileInfo = new FileInfo(fullPath);             // File exists, so we can get its info and its size to put into the response header
            long fileSize = fileInfo.Length;

            usedLength = fileSize;                                  // Set the file size to a property to be able to properly write to our log once sent

            string response = "HTTP/1.1 " + statusCode + " " + Constants.HTTPDictionary[statusCode] + "\r\n" +
                                   "Content-Type: "+ contentType +"\r\n" +
                                   "Content-Length: "+ fileInfo.ToString() +"\r\n" +
                                   "Server: myOwnWebserver\r\n" +
                                   "Date: " + DateTime.Now.ToString("r") + "\r\n" +
                                   "\r\n";

            return response;
        }
    }
}
